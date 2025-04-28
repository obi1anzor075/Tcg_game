using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }

    // Список экземпляров карт, сыгранных в текущем раунде
    private readonly List<CardInstance> _roundPlayed = new();

    [Header("Герои")]
    [SerializeField] private CardData _chosenHeroData;
    [SerializeField] private CardData _chosenAIHeroData;

    [Header("Начальные миньоны")]
    [SerializeField] private int _initialMinionCount = 3;

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

        // 2) Основной цикл
        while (State != GameState.GameOver)
        {
            // --- Ход игрока ---
            State = GameState.PlayerTurn;

            // 2.1 Начало хода игрока
            TurnManager.Instance.StartTurn(isPlayer: true);
            UIManager.Instance.UpdateLoyaltyDisplay();

            // 2.2 Добор и UI руки
            HandManager.Instance.DrawTurnCards(isPlayer: true);
            UIManager.Instance.RefreshHands();

            // 2.3 Ждём EndTurn
            yield return StartCoroutine(PlayerTurn());

            // 2.4 Общая пост-ходовая логика для игрока
            EndTurnCleanup(isPlayer: true);
            if (CheckGameOver()) break;

            // --- Ход ИИ ---
            State = GameState.EnemyTurn;

            // 2.5 Начало хода ИИ
            TurnManager.Instance.StartTurn(isPlayer: false);
            // (можно выводить в HUD ИИ, но обычно скрыто)

            // 2.6 Добор и UI руки ИИ
            HandManager.Instance.DrawTurnCards(isPlayer: false);
            UIManager.Instance.RefreshHands();

            // 2.7 Ход ИИ
            yield return StartCoroutine(EnemyTurn());

            // 2.8 Общая пост-ходовая логика для ИИ
            EndTurnCleanup(isPlayer: false);
            if (CheckGameOver()) break;
        }

        State = GameState.GameOver;
        HandleGameOver();
    }

    private IEnumerator Setup()
    {
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
        yield return null;
    }

    private IEnumerator PlayerTurn()
    {
        Debug.Log("Player's Turn");
        bool ended = false;
        UIManager.Instance.OnEndTurnClicked += () => ended = true;
        while (!ended) yield return null;
        UIManager.Instance.OnEndTurnClicked -= () => ended = true;
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy's Turn");
        AIManager.Instance.PlayTurn();
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Общая логика после хода: 
    /// перенос карт, фаза атаки, очистка played-панели.
    /// </summary>
    private void EndTurnCleanup(bool isPlayer)
    {
        // Переносим сыгранные в постоянное поле
        TurnManager.Instance.RemoveCardsFromField(_roundPlayed);

        // Обновляем и показываем поле
        UIManager.Instance.RefreshBattlefield();

        // Фаза атаки
        AttackManager.Instance.ResolveAttackPhase(isPlayer);

        // Ещё раз обновляем поле после атак
        UIManager.Instance.RefreshBattlefield();

        // Очищаем панель сыгранных карт
        UIManager.Instance.ClearPlayedAreas();

        // Очищаем внутренний список
        ClearRoundPlayed();
    }

    private bool CheckGameOver()
    {
        bool pDead = TurnManager.Instance.PlayerCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);
        bool eDead = TurnManager.Instance.EnemyCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        bool allPM = TurnManager.Instance.PlayerCards
            .Where(c => c.cardData.type == CardType.Minion).All(c => !c.IsAlive);
        bool allEM = TurnManager.Instance.EnemyCards
            .Where(c => c.cardData.type == CardType.Minion).All(c => !c.IsAlive);

        return (pDead && allPM) || (eDead && allEM);
    }

    public void RecordPlayed(CardInstance inst)
    {
        _roundPlayed.Add(inst);
    }

    public void ClearRoundPlayed()
    {
        _roundPlayed.Clear();
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over!");
        // Тут можно вызвать UIManager.ShowGameOver(...)
    }
}
