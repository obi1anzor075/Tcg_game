using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Manager: HandManager

/// <summary>
/// �������� �� ������, ���� ������ � ��, � ����� �� ����� � �������� ���� �� ����.
/// </summary>
public class HandManager : MonoBehaviour
{
    #region Singleton

    public static HandManager Instance { get; private set; }

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

    #endregion

    #region Inspector Fields

    [Header("Hand Settings")]
    [Tooltip("������ ��������� ���� ��� ������� ������")]
    [SerializeField] private int _startHandSize = 5;

    [Header("Decks (assign in Inspector)")]
    [Tooltip("������ ������")]
    [SerializeField] private List<CardData> _playerDeck = new();
    [Tooltip("������ ��")]
    [SerializeField] private List<CardData> _enemyDeck = new();

    #endregion

    #region Public State

    /// <summary>������� ���� ������.</summary>
    public List<CardData> playerHand { get; private set; } = new();

    /// <summary>������� ���� ��.</summary>
    public List<CardData> enemyHand { get; private set; } = new();

    /// <summary>�������: �����/�� �������� ����� (��� UI � ������).</summary>
    public event Action<CardData> OnCardPlayed;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // ��� ������ ����� ������������� ������� ��������� ����
        // ��� �������� DealStartHands() �� GameManager.Setup()
    }

    #endregion

    #region Public API

    /// <summary>
    /// �������������� ������ � ������ �� _startHandSize ���� ������ � ��.
    /// �������� ���� ��� � Setup.
    /// </summary>
    public void DealStartHands()
    {
        Shuffle(_playerDeck);
        Shuffle(_enemyDeck);

        for (int i = 0; i < _startHandSize; i++)
        {
            DrawCard(_playerDeck, playerHand);
            DrawCard(_enemyDeck, enemyHand);
        }
    }

    /// <summary>
    /// �������� �� ����� ����� ��� ������ ��� �� �� ������ ����.
    /// </summary>
    /// <param name="isPlayer">true � �����, false � ��</param>
    public void DrawTurnCards(bool isPlayer)
    {
        if (isPlayer)
            DrawCard(_playerDeck, playerHand);
        else
            DrawCard(_enemyDeck, enemyHand);
    }

    /// <summary>
    /// �������� ��������� ����� �� ����.
    /// ������� � �� ������ � ���������� ������� OnCardPlayed.
    /// </summary>
    /// <param name="cardData">������ �����</param>
    /// <param name="isPlayer">true � ��� ������, false � ��� ��</param>
    /// <returns>true, ���� ����� ���� � ���� � �������</returns>
    public bool TryPlayCard(CardData cardData, bool isPlayer)
    {
        var hand = isPlayer ? playerHand : enemyHand;
        if (!hand.Contains(cardData))
            return false;

        hand.Remove(cardData);
        OnCardPlayed?.Invoke(cardData);
        return true;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// ���������� ������� ����� �� deck � hand.
    /// </summary>
    private void DrawCard(List<CardData> deck, List<CardData> hand)
    {
        if (deck.Count == 0)
            return;

        var top = deck[0];
        deck.RemoveAt(0);
        hand.Add(top);
    }

    /// <summary>
    /// ������������ ������ ���� � ��������� �������.
    /// </summary>
    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, list.Count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    /// <summary>
    /// ��������� ������ ����� ���� Minion �� ������ ������ � ���������� �.
    /// </summary>
    public CardData DrawFirstMinionFromPlayerDeck()
    {
        var minion = _playerDeck.FirstOrDefault(c => c.type == CardType.Minion);
        if (minion != null) _playerDeck.Remove(minion);
        return minion;
    }

    /// <summary>
    /// ������ ��� ��.
    /// </summary>
    public CardData DrawFirstMinionFromEnemyDeck()
    {
        var minion = _enemyDeck.FirstOrDefault(c => c.type == CardType.Minion);
        if (minion != null) _enemyDeck.Remove(minion);
        return minion;
    }

    #endregion
}

#endregion
