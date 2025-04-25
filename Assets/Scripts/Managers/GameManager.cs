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

    [SerializeField] private CardData _chosenHeroData = null;
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
            // Ход игрока
            State = GameState.PlayerTurn;

            // 2.1. Сброс и восстановление лояльности
            TurnManager.Instance.StartTurn();

            // 2.2. Добор карты
            HandManager.Instance.DrawTurnCards(true);
            UIManager.Instance.RefreshHands();

            // 2.3. Корутин игрока
            yield return StartCoroutine(PlayerTurn());

            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();
            GameManager.Instance.ClearRoundPlayed();

            // 2.4. Переносим сыгранные карты в поле и очищаем PlayedCards
            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();

            // 2.5. Проверка конца игры
            if (CheckGameOver()) break;

            // Ход ИИ
            State = GameState.EnemyTurn;

            // 2.6. Сброс и восстановление лояльности для ИИ
            TurnManager.Instance.StartTurn();

            // 2.7. Добор карты ИИ и обновление руки
            HandManager.Instance.DrawTurnCards(false);
            UIManager.Instance.RefreshHands();

            // 2.8. Корутин ИИ
            yield return StartCoroutine(EnemyTurn());

            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();
            GameManager.Instance.ClearRoundPlayed();


            // 2.9. Перенос ИИ-карт и очистка PlayedCards
            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();
            ClearRoundPlayed();

            // 2.10. Проверка конца игры
            if (CheckGameOver()) break;
        }

        State = GameState.GameOver;
        HandleGameOver();
    }

    private IEnumerator Setup()
    {
        // 1) Добавляем выбранного героя на поле
        var heroInst = new CardInstance(_chosenHeroData);
        TurnManager.Instance.AddPlayerCard(heroInst);

        // 2) Добавляем _initialMinionCount верных сразу на поле
        int added = 0;
        // Берём из колоды первые по типу Minion
        foreach (var minionData in HandManager.Instance.playerDeck
                                        .Where(c => c.type == CardType.Minion)
                                        .ToList())
        {
            if (added >= _initialMinionCount)
                break;

            // Удаляем эту карту из колоды
            HandManager.Instance.playerDeck.Remove(minionData);

            // Создаём экземпляр и добавляем на поле
            TurnManager.Instance.AddPlayerCard(new CardInstance(minionData));
            added++;
        }

        // 3) Отрисовываем поле (герой + стартовые верные)
        UIManager.Instance.RefreshBattlefield();

        // 4) Раздаём стартовые руки
        HandManager.Instance.DealStartHands();
        UIManager.Instance.RefreshHands();

        Debug.Log($"Game Setup complete: Hero + {added} minions placed, {HandManager.Instance.playerHand.Count} cards in hand.");
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