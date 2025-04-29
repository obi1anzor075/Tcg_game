using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
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

            TurnManager.Instance.StartTurn(isPlayer: true);
            HandManager.Instance.DrawTurnCards(true);
            UIManager.Instance.RefreshHands();

            yield return StartCoroutine(PlayerTurn());

            EndTurnCleanup(isPlayer: true);
            if (CheckGameOver()) break;

            // --- Ход ИИ ---
            State = GameState.EnemyTurn;

            TurnManager.Instance.StartTurn(isPlayer: false);
            HandManager.Instance.DrawTurnCards(false);
            UIManager.Instance.RefreshHands();

            yield return StartCoroutine(EnemyTurn());

            EndTurnCleanup(isPlayer: false);
            if (CheckGameOver()) break;
        }

        State = GameState.GameOver;
        HandleGameOver();
    }

    private IEnumerator Setup()
    {
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

        yield return null;
    }

    private IEnumerator PlayerTurn()
    {
        bool ended = false;
        UIManager.Instance.OnEndTurnClicked += () => ended = true;
        while (!ended) yield return null;
        UIManager.Instance.OnEndTurnClicked -= () => ended = true;
    }

    private IEnumerator EnemyTurn()
    {
        AIManager.Instance.PlayTurn();
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
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

    private void HandleGameOver()
    {
        Debug.Log("Game Over!");
        // TODO: показать экран победы/поражения
    }
}
