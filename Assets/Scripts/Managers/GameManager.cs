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

        // 2. �������� ���� �����
        while (State != GameState.GameOver)
        {
            // ��� ������
            State = GameState.PlayerTurn;
            // ������� ����� � �������� UI
            HandManager.Instance.DrawTurnCards(true);
            UIManager.Instance.RefreshHands();
            TurnManager.Instance.StartTurn();
            yield return StartCoroutine(PlayerTurn());

            // �������� �� ������/���������
            if (CheckGameOver()) break;

            // ��� AI
            State = GameState.EnemyTurn;
            // ������� ����� � �������� UI
            HandManager.Instance.DrawTurnCards(false);
            UIManager.Instance.RefreshHands();
            TurnManager.Instance.StartTurn();   // ����� ��������� ����������/������� ��� �����
            yield return StartCoroutine(EnemyTurn());

            if (CheckGameOver()) break;
        }

        State = GameState.GameOver;
        HandleGameOver();
    }

    private IEnumerator Setup()
    {
        // TODO: �������� ����� (����� CardInstance � type=Hero) � TurnManager.Instance.playerCards
        // ������� ��������� ���� � �������� UI
        // 1) ��������� ���������� ����� �� ����
        var heroInst = new CardInstance(chosenHeroData);
        TurnManager.Instance.AddPlayerCard(heroInst);

        // 2) ������ ���� � ������
        UIManager.Instance.RefreshBattlefield();

        // 3) ������ ��������� ����
        HandManager.Instance.DealStartHands();
        UIManager.Instance.RefreshHands();


        Debug.Log("Game Setup...");
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


    private void HandleGameOver()
    {
        Debug.Log("Game Over!");
        // TODO: �������� ����� ������/���������
    }
}