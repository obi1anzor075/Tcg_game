using System;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    [Header("��������� ����")]
    public int startHandSize = 5;
    public int drawPerTurn = 1;

    [Header("������ ������� (� ����������)")]
    public List<CardData> playerDeck = new();
    public List<CardData> enemyDeck = new();

    public List<CardData> playerHand { get; private set; } = new();
    public List<CardData> enemyHand { get; private set; } = new();

    // �������: ����� ��������� (��� UI)
    public event Action<CardData> OnCardPlayed;

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

    /// <summary>
    /// ���������� � Setup() GameManager: ������ ��������� ����.
    /// </summary>
    public void DealStartHands()
    {
        Shuffle(playerDeck);
        Shuffle(enemyDeck);

        for (int i = 0; i < startHandSize; i++)
        {
            DrawCard(playerDeck, playerHand);
            DrawCard(enemyDeck, enemyHand);
        }
    }

    /// <summary>
    /// �������� ����� � ������ ����.
    /// </summary>
    public void DrawTurnCards(bool isPlayer)
    {
        if (isPlayer) DrawCard(playerDeck, playerHand);
        else DrawCard(enemyDeck, enemyHand);
    }

    private void DrawCard(List<CardData> deck, List<CardData> hand)
    {
        if (deck.Count == 0) return;
        var top = deck[0];
        deck.RemoveAt(0);
        hand.Add(top);
    }

    /// <summary>
    /// �������� ��������� ����� �� ����.
    /// </summary>
    public bool TryPlayCard(CardData cardData, bool isPlayer)
    {
        var hand = isPlayer ? playerHand : enemyHand;
        if (!hand.Contains(cardData)) return false;
        hand.Remove(cardData);
        OnCardPlayed?.Invoke(cardData);
        return true;
    }

    /// <summary>
    /// ��������� � ������� �� playerDeck ������ ����� ���� Minion.
    /// </summary>
    public CardData DrawFirstMinionFromPlayerDeck()
    {
        var m = playerDeck.Find(c => c.type == CardType.Minion);
        if (m != null) playerDeck.Remove(m);
        return m;
    }

    /// <summary>
    /// ��������� � ������� �� enemyDeck ������ ����� ���� Minion.
    /// </summary>
    public CardData DrawFirstMinionFromEnemyDeck()
    {
        var m = enemyDeck.Find(c => c.type == CardType.Minion);
        if (m != null) enemyDeck.Remove(m);
        return m;
    }

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
}
