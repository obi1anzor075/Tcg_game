using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }
    [SerializeField]public CardData chosenHeroData = null;

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
            // Ход игрока
            State = GameState.PlayerTurn;
            // Раздать карту и обновить UI
            HandManager.Instance.DrawTurnCards(true);
            UIManager.Instance.RefreshHands();
            TurnManager.Instance.StartTurn();
            yield return StartCoroutine(PlayerTurn());

            // Проверка на победу/поражение
            if (CheckGameOver()) break;

            // Ход AI
            State = GameState.EnemyTurn;
            // Раздать карту и обновить UI
            HandManager.Instance.DrawTurnCards(false);
            UIManager.Instance.RefreshHands();
            TurnManager.Instance.StartTurn();   // можно разделить лояльность/условия для врага
            yield return StartCoroutine(EnemyTurn());

            if (CheckGameOver()) break;
        }

        State = GameState.GameOver;
        HandleGameOver();
    }

    private IEnumerator Setup()
    {
        // TODO: добавить героя (через CardInstance с type=Hero) в TurnManager.Instance.playerCards
        // Раздать стартовую руку и обновить UI
        // 1) Добавляем выбранного героя на поле
        var heroInst = new CardInstance(chosenHeroData);
        TurnManager.Instance.AddPlayerCard(heroInst);

        // 2) Рисуем поле с героем
        UIManager.Instance.RefreshBattlefield();

        // 3) Раздаём стартовые руки
        HandManager.Instance.DealStartHands();
        UIManager.Instance.RefreshHands();


        Debug.Log("Game Setup...");
        yield return null; // здесь можно подождать анимации
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


    private void HandleGameOver()
    {
        Debug.Log("Game Over!");
        // TODO: показать экран победы/поражения
    }
}