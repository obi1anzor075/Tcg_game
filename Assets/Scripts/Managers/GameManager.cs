using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }

    // Список карт, сыгранных в текущем раунде
    private readonly List<CardInstance> _roundPlayed = new();

    [Header("Герои")]
    [SerializeField] private CardData _chosenHeroData = null;
    [SerializeField] private CardData _chosenAIHeroData = null;

    [Header("Начальное поле")]
    [SerializeField] private int _initialMinionCount = 3;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(RunGameLoop());
    }

    private IEnumerator RunGameLoop()
    {
        // 1. Setup
        State = GameState.Setup;
        yield return StartCoroutine(Setup());

        // 2. Основной цикл ходов
        while (State != GameState.GameOver)
        {
            // --- Ход игрока ---
            State = GameState.PlayerTurn;

            // 2.1. Начало хода: сброс и восстановление лояльности
            TurnManager.Instance.StartPlayerTurn();

            // 2.2. Добор карты и обновление руки
            HandManager.Instance.DrawTurnCards(true);
            UIManager.Instance.RefreshHands();

            // 2.3. Ждём EndTurn
            yield return StartCoroutine(PlayerTurn());

            // 2.4. После хода: перенос сыгранных карт в поле и очистка played-панели
            TurnManager.Instance.RemoveCardsFromField(_roundPlayed);
            UIManager.Instance.RefreshBattlefield();

            // ── ВСТАВЛЯЕМ ФАЗУ АТАКИ ИГРОКА ──
            AttackManager.Instance.ResolveAttackPhase(true);
            UIManager.Instance.RefreshBattlefield();
            // ──────────────────────────────────

            UIManager.Instance.ClearPlayedAreas();
            ClearRoundPlayed();

            // 2.5. Проверка Game Over
            if (CheckGameOver()) break;


            // --- Ход ИИ ---
            State = GameState.EnemyTurn;

            // 2.6. Начало хода ИИ
            TurnManager.Instance.StartEnemyTurn();

            // 2.7. Добор карты ИИ и обновление руки
            HandManager.Instance.DrawTurnCards(false);
            UIManager.Instance.RefreshHands();

            // 2.8. Ход ИИ
            yield return StartCoroutine(EnemyTurn());

            // 2.9. После хода ИИ: перенос сыгранных карт и очистка played-панели
            TurnManager.Instance.RemoveCardsFromField(_roundPlayed);
            UIManager.Instance.RefreshBattlefield();

            // ── ВСТАВЛЯЕМ ФАЗУ АТАКИ ИИ ──
            AttackManager.Instance.ResolveAttackPhase(false);
            UIManager.Instance.RefreshBattlefield();
            // ─────────────────────────────

            UIManager.Instance.ClearPlayedAreas();
            ClearRoundPlayed();

            // 2.10. Проверка Game Over
            if (CheckGameOver()) break;
        }

        State = GameState.GameOver;
        HandleGameOver();
    }


    private IEnumerator Setup()
    {
        // ---- Игрок ----

        // 1. Добавляем героя игрока
        var heroInst = new CardInstance(_chosenHeroData);
        TurnManager.Instance.AddPlayerCard(heroInst);

        // 2. Сразу выставляем _initialMinionCount верных игрока
        int added = 0;
        foreach (var minionData in HandManager.Instance.playerDeck
                                        .Where(c => c.type == CardType.Minion)
                                        .ToList())
        {
            if (added >= _initialMinionCount) break;
            HandManager.Instance.playerDeck.Remove(minionData);
            TurnManager.Instance.AddPlayerCard(new CardInstance(minionData));
            added++;
        }

        // ---- ИИ ----

        // 3. Добавляем героя ИИ
        var aiHeroInst = new CardInstance(_chosenAIHeroData);
        TurnManager.Instance.AddEnemyCard(aiHeroInst);

        // 4. Сразу выставляем _initialMinionCount верных ИИ
        int aiAdded = 0;
        foreach (var minionData in HandManager.Instance.enemyDeck
                                        .Where(c => c.type == CardType.Minion)
                                        .ToList())
        {
            if (aiAdded >= _initialMinionCount) break;
            HandManager.Instance.enemyDeck.Remove(minionData);
            TurnManager.Instance.AddEnemyCard(new CardInstance(minionData));
            aiAdded++;
        }

        // 5. Рисуем поле (герои + начальные верные обеих сторон)
        UIManager.Instance.RefreshBattlefield();

        // 6. Раздаём стартовые руки
        HandManager.Instance.DealStartHands();
        UIManager.Instance.RefreshHands();

        Debug.Log($"Game Setup complete:\n" +
                  $"- Player: Hero + {added} minions\n" +
                  $"- AI: Hero + {aiAdded} minions\n" +
                  $"- Player hand: {HandManager.Instance.playerHand.Count} cards");
        yield return null;
    }


    private IEnumerator PlayerTurn()
    {
        Debug.Log("Player's Turn");
        // Ждём, пока игрок нажмёт кнопку EndTurn
        bool ended = false;
        UIManager.Instance.OnEndTurnClicked += () => ended = true;
        while (!ended)
            yield return null;
        UIManager.Instance.OnEndTurnClicked -= () => ended = true;
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy's Turn");
        // Вызываем ИИ
        AIManager.Instance.PlayTurn();
        // Подождать пару секунд, чтобы показать ходы врага
        yield return new WaitForSeconds(1f);
    }

    private bool CheckGameOver()
    {
        // Если хотя бы один герой убит
        bool playerHeroDead = TurnManager.Instance.PlayerCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        bool enemyHeroDead = TurnManager.Instance.EnemyCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        // Или если все герои игрока мертвы
        bool allPlayerMinionsDead = TurnManager.Instance.PlayerCards
            .Where(c => c.cardData.type == CardType.Minion)
            .All(c => !c.IsAlive);

        // Или если все герои врага мертвы
        bool allEnemyMinionsDead = TurnManager.Instance.EnemyCards
            .Where(c => c.cardData.type == CardType.Minion)
            .All(c => !c.IsAlive);

        return (playerHeroDead  && allPlayerMinionsDead) || (enemyHeroDead && allEnemyMinionsDead);
    }

    /// <summary>Сохраняет экземпляр сыгранной карты текущего раунда.</summary>
    public void RecordPlayed(CardInstance inst)
    {
        _roundPlayed.Add(inst);
    }

    /// <summary>Очищает список сыгранных за раунд карт.</summary>
    public void ClearRoundPlayed()
    {
        _roundPlayed.Clear();
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over!");
        // TODO: показать экран победы/поражения
    }

}