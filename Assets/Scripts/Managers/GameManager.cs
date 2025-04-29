using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; }

<<<<<<< HEAD
    // Список карт, сыгранных в текущем раунде
    private readonly List<CardInstance> _roundPlayed = new();

    [Header("Герои")]
    [SerializeField] private CardData _chosenHeroData = null;
    [SerializeField] private CardData _chosenAIHeroData = null;

    [Header("Начальное поле")]
    [SerializeField] private int _initialMinionCount = 3;
=======
    // Список экземпляров карт, сыгранных в текущем раунде
    private readonly List<CardInstance> _roundPlayed = new();

    [Header("Герои")]
    [SerializeField] private CardData _chosenHeroData;
    [SerializeField] private CardData _chosenAIHeroData;

    [Header("Начальные миньоны")]
    [SerializeField] private int _initialMinionCount = 3;

    [Header("Game Settings")]
    [SerializeField] private bool _skipFirstAttackPhase = true;
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    private void Start()
    {
        StartCoroutine(RunGameLoop());
    }

    private IEnumerator RunGameLoop()
    {
        // 1) Setup
        State = GameState.Setup;
        yield return StartCoroutine(Setup());

<<<<<<< HEAD
        // 2. Основной цикл ходов
=======
        // 2) Основной цикл
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        while (State != GameState.GameOver)
        {
            // --- Ход игрока ---
            State = GameState.PlayerTurn;

<<<<<<< HEAD
            TurnManager.Instance.StartTurn(isPlayer: true);
            HandManager.Instance.DrawTurnCards(true);
            UIManager.Instance.RefreshHands();

            yield return StartCoroutine(PlayerTurn());

            EndTurnCleanup(isPlayer: true);
            if (CheckGameOver()) break;
=======
            // 2.1 Начало хода игрока
            TurnManager.Instance.StartTurn(isPlayer: true);
            UIManager.Instance.UpdateLoyaltyDisplay();

            // 2.2 Добор и UI руки
            HandManager.Instance.DrawTurnCards(isPlayer: true);
            UIManager.Instance.RefreshHands();

            // 2.3 Ждём EndTurn
            yield return StartCoroutine(PlayerTurn());

            // 2.4 Общая пост-ходовая логика для игрока
            bool skipAttackPhase = (_skipFirstAttackPhase && TurnManager.Instance.CurrentTurn == 1);
            EndTurnCleanup(isPlayer: true, skipAttackPhase);

            // Check if game is over AFTER player's turn
            if (CheckGameOver())
            {
                Debug.Log("Game over after player's turn");
                break;
            }
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

            // --- Ход ИИ ---
            State = GameState.EnemyTurn;

<<<<<<< HEAD
            TurnManager.Instance.StartTurn(isPlayer: false);
            HandManager.Instance.DrawTurnCards(false);
            UIManager.Instance.RefreshHands();

            yield return StartCoroutine(EnemyTurn());

            EndTurnCleanup(isPlayer: false);
            if (CheckGameOver()) break;
=======
            // 2.5 Начало хода ИИ
            TurnManager.Instance.StartTurn(isPlayer: false);
            // (можно выводить в HUD ИИ, но обычно скрыто)

            // 2.6 Добор и UI руки ИИ
            HandManager.Instance.DrawTurnCards(isPlayer: false);
            UIManager.Instance.RefreshHands();

            // 2.7 Ход ИИ
            yield return StartCoroutine(EnemyTurn());

            // 2.8 Общая пост-ходовая логика для ИИ
            EndTurnCleanup(isPlayer: false, false);

            // Check if game is over AFTER AI's turn
            if (CheckGameOver())
            {
                Debug.Log("Game over after AI's turn");
                break;
            }
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        }

        State = GameState.GameOver;
        HandleGameOver();
    }

    private IEnumerator Setup()
    {
<<<<<<< HEAD
        // Игрок
        TurnManager.Instance.AddPlayerCard(new CardInstance(_chosenHeroData));
        for (int i = 0; i < _initialMinionCount; i++)
        {
            var m = HandManager.Instance.DrawFirstMinionFromPlayerDeck();
            if (m == null) break;
            TurnManager.Instance.AddPlayerCard(new CardInstance(m));
        }

        // ИИ
        TurnManager.Instance.AddEnemyCard(new CardInstance(_chosenAIHeroData));
        for (int i = 0; i < _initialMinionCount; i++)
        {
            var m = HandManager.Instance.DrawFirstMinionFromEnemyDeck();
            if (m == null) break;
            TurnManager.Instance.AddEnemyCard(new CardInstance(m));
        }

        UIManager.Instance.RefreshBattlefield();
        HandManager.Instance.DealStartHands();
        UIManager.Instance.RefreshHands();

        // Устанавливаем лояльность перед первым ходом
        TurnManager.Instance.StartTurn(isPlayer: true);

=======
        // — Игрок —
        // 1) Герой
        TurnManager.Instance.AddPlayerCard(new CardInstance(_chosenHeroData));

        // 2) Начальные миньоны игрока
        int added = 0;
        for (int i = 0; i < _initialMinionCount; i++)
        {
            var mData = HandManager.Instance.DrawFirstMinionFromPlayerDeck();
            if (mData == null) break;
            TurnManager.Instance.AddPlayerCard(new CardInstance(mData));
            added++;
        }

        // — ИИ —
        // 3) Герой ИИ
        TurnManager.Instance.AddEnemyCard(new CardInstance(_chosenAIHeroData));

        // 4) Начальные миньоны ИИ
        int aiAdded = 0;
        for (int i = 0; i < _initialMinionCount; i++)
        {
            var mData = HandManager.Instance.DrawFirstMinionFromEnemyDeck();
            if (mData == null) break;
            TurnManager.Instance.AddEnemyCard(new CardInstance(mData));
            aiAdded++;
        }

        // 5) Первичный UI поля
        UIManager.Instance.RefreshBattlefield();

        // 6) Раздача стартовых рук
        HandManager.Instance.DealStartHands();
        UIManager.Instance.RefreshHands();

        // 7) Задаём преданность перед первым ходом
        TurnManager.Instance.StartTurn(isPlayer: true);
        UIManager.Instance.UpdateLoyaltyDisplay();

        Debug.Log($"Setup complete: Player minions={added}, AI minions={aiAdded}, Player hand={HandManager.Instance.playerHand.Count}");
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        yield return null;
    }

    private IEnumerator PlayerTurn()
    {
<<<<<<< HEAD
=======
        Debug.Log("Player's Turn");
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        bool ended = false;
        UIManager.Instance.OnEndTurnClicked += () => ended = true;
        while (!ended) yield return null;
        UIManager.Instance.OnEndTurnClicked -= () => ended = true;
    }

    private IEnumerator EnemyTurn()
    {
<<<<<<< HEAD
=======
        Debug.Log("Enemy's Turn");
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        AIManager.Instance.PlayTurn();
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
<<<<<<< HEAD
    /// Общая логика после каждого хода: очистка played, удаление карт, атака, перерисовка.
    /// </summary>
    private void EndTurnCleanup(bool isPlayer)
    {
        // 1) Удаляем сыгранные карты из постоянного поля
        TurnManager.Instance.RemoveCardsFromField(_roundPlayed);

        // 2) Очищаем UI зоны «сыгранных» карт
        UIManager.Instance.ClearPlayedAreas();

        // 3) Перерисовываем поле (теперь уже без этих карт)
        UIManager.Instance.RefreshBattlefield();

        // 4) Фаза автоматической атаки
        AttackManager.Instance.ResolveAttackPhase(isPlayer);

        // 5) И снова обновляем поле после атак
        UIManager.Instance.RefreshBattlefield();

        // 6) Сбрасываем список сыгранных за раунд
        ClearRoundPlayed();
    }



    private bool CheckGameOver()
    {
        bool pDead = TurnManager.Instance.PlayerCards.Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);
        bool eDead = TurnManager.Instance.EnemyCards.Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        bool allPM = TurnManager.Instance.PlayerCards.Where(c => c.cardData.type == CardType.Minion).All(c => !c.IsAlive);
        bool allEM = TurnManager.Instance.EnemyCards.Where(c => c.cardData.type == CardType.Minion).All(c => !c.IsAlive);

        return (pDead && allPM) || (eDead && allEM);
    }

    public void RecordPlayed(CardInstance inst) => _roundPlayed.Add(inst);
    public void ClearRoundPlayed() => _roundPlayed.Clear();
=======
    /// Общая логика после хода: 
    /// перенос карт, фаза атаки, очистка played-панели.
    /// </summary>
    private void EndTurnCleanup(bool isPlayer, bool skipAttackPhase = false)
    {
        // Переносим сыгранные в постоянное поле
        TurnManager.Instance.RemoveCardsFromField(_roundPlayed);

        // Обновляем и показываем поле
        UIManager.Instance.RefreshBattlefield();

        // Фаза атаки (можно пропустить в первом ходу)
        if (!skipAttackPhase)
        {
            AttackManager.Instance.ResolveAttackPhase(isPlayer);
        }
        else
        {
            Debug.Log("Skipping first attack phase");
        }

        // Ещё раз обновляем поле после атак
        UIManager.Instance.RefreshBattlefield();

        // Очищаем панель сыгранных карт
        UIManager.Instance.ClearPlayedAreas();

        // Очищаем внутренний список
        ClearRoundPlayed();
    }

    private bool CheckGameOver()
    {
        // Check if player hero is dead
        bool playerHeroDead = TurnManager.Instance.PlayerCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        // Check if AI hero is dead
        bool enemyHeroDead = TurnManager.Instance.EnemyCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        // Check if all player minions are dead
        bool allPlayerMinionsDead = !TurnManager.Instance.PlayerCards
            .Any(c => c.cardData.type == CardType.Minion && c.IsAlive);

        // Check if all enemy minions are dead
        bool allEnemyMinionsDead = !TurnManager.Instance.EnemyCards
            .Any(c => c.cardData.type == CardType.Minion && c.IsAlive);

        // Game over conditions:
        // 1. Player hero is dead AND all player minions are dead
        // 2. Enemy hero is dead AND all enemy minions are dead
        bool gameOver = (playerHeroDead && allPlayerMinionsDead) || (enemyHeroDead && allEnemyMinionsDead);

        if (gameOver)
        {
            Debug.Log($"Game over check: Player hero dead: {playerHeroDead}, All player minions dead: {allPlayerMinionsDead}");
            Debug.Log($"Game over check: Enemy hero dead: {enemyHeroDead}, All enemy minions dead: {allEnemyMinionsDead}");
        }

        return gameOver;
    }

    public void RecordPlayed(CardInstance inst)
    {
        _roundPlayed.Add(inst);
    }

    public void ClearRoundPlayed()
    {
        _roundPlayed.Clear();
    }
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

    private void HandleGameOver()
    {
        Debug.Log("Game Over!");
<<<<<<< HEAD
        // TODO: показать экран победы/поражения
    }
}
=======
        // Тут можно вызвать UIManager.ShowGameOver(...)
    }
}
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
