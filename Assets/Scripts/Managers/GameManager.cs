using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }

    // ������ ����, ��������� � ������� ������
    private readonly List<CardInstance> _roundPlayed = new();

    [SerializeField] private CardData _chosenHeroData = null;
    [Header("��������� ����")]
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

        // 2. �������� ���� �����
        while (State != GameState.GameOver)
        {
            // ��� ������
            State = GameState.PlayerTurn;

            // 2.1. ����� � �������������� ����������
            TurnManager.Instance.StartTurn();

            // 2.2. ����� �����
            HandManager.Instance.DrawTurnCards(true);
            UIManager.Instance.RefreshHands();

            // 2.3. ������� ������
            yield return StartCoroutine(PlayerTurn());

            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();
            GameManager.Instance.ClearRoundPlayed();

            // 2.4. ��������� ��������� ����� � ���� � ������� PlayedCards
            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();

            // 2.5. �������� ����� ����
            if (CheckGameOver()) break;

            // ��� ��
            State = GameState.EnemyTurn;

            // 2.6. ����� � �������������� ���������� ��� ��
            TurnManager.Instance.StartTurn();

            // 2.7. ����� ����� �� � ���������� ����
            HandManager.Instance.DrawTurnCards(false);
            UIManager.Instance.RefreshHands();

            // 2.8. ������� ��
            yield return StartCoroutine(EnemyTurn());

            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();
            GameManager.Instance.ClearRoundPlayed();


            // 2.9. ������� ��-���� � ������� PlayedCards
            UIManager.Instance.RefreshBattlefield();
            UIManager.Instance.ClearPlayedArea();
            ClearRoundPlayed();

            // 2.10. �������� ����� ����
            if (CheckGameOver()) break;
        }

        State = GameState.GameOver;
        HandleGameOver();
    }

    private IEnumerator Setup()
    {
        // 1) ��������� ���������� ����� �� ����
        var heroInst = new CardInstance(_chosenHeroData);
        TurnManager.Instance.AddPlayerCard(heroInst);

        // 2) ��������� _initialMinionCount ������ ����� �� ����
        int added = 0;
        // ���� �� ������ ������ �� ���� Minion
        foreach (var minionData in HandManager.Instance.playerDeck
                                        .Where(c => c.type == CardType.Minion)
                                        .ToList())
        {
            if (added >= _initialMinionCount)
                break;

            // ������� ��� ����� �� ������
            HandManager.Instance.playerDeck.Remove(minionData);

            // ������ ��������� � ��������� �� ����
            TurnManager.Instance.AddPlayerCard(new CardInstance(minionData));
            added++;
        }

        // 3) ������������ ���� (����� + ��������� ������)
        UIManager.Instance.RefreshBattlefield();

        // 4) ������ ��������� ����
        HandManager.Instance.DealStartHands();
        UIManager.Instance.RefreshHands();

        Debug.Log($"Game Setup complete: Hero + {added} minions placed, {HandManager.Instance.playerHand.Count} cards in hand.");
        yield return null; // ����� ����� ��������� ��������
    }


    private IEnumerator PlayerTurn()
    {
        Debug.Log("Player's Turn");
        // ���, ���� ����� ����� ������ EndTurn
        bool ended = false;
        UIManager.Instance.OnEndTurnClicked += () => ended = true;
        while (!ended)
            yield return null;
        UIManager.Instance.OnEndTurnClicked -= () => ended = true;
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy's Turn");
        // �������� ��
        AIManager.Instance.PlayTurn();
        // ��������� ���� ������, ����� �������� ���� �����
        yield return new WaitForSeconds(1f);
    }

    private bool CheckGameOver()
    {
        // ���� ���� �� ���� ����� ����
        bool playerHeroDead = TurnManager.Instance.PlayerCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        bool enemyHeroDead = TurnManager.Instance.EnemyCards
            .Any(c => c.cardData.type == CardType.Hero && !c.IsAlive);

        // ��� ���� ��� ����� ������ ������
        bool allPlayerMinionsDead = TurnManager.Instance.PlayerCards
            .Where(c => c.cardData.type == CardType.Minion)
            .All(c => !c.IsAlive);

        // ��� ���� ��� ����� ����� ������
        bool allEnemyMinionsDead = TurnManager.Instance.EnemyCards
            .Where(c => c.cardData.type == CardType.Minion)
            .All(c => !c.IsAlive);

        return (playerHeroDead  && allPlayerMinionsDead) || (enemyHeroDead && allEnemyMinionsDead);
    }

    /// <summary>��������� ��������� ��������� ����� �������� ������.</summary>
    public void RecordPlayed(CardInstance inst)
    {
        _roundPlayed.Add(inst);
    }

    /// <summary>������� ������ ��������� �� ����� ����.</summary>
    public void ClearRoundPlayed()
    {
        _roundPlayed.Clear();
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over!");
        // TODO: �������� ����� ������/���������
    }

}