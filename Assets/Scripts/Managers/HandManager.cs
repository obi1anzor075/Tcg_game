using System;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    [Header("Настройки руки")]
    public int startHandSize = 5;
    public int drawPerTurn = 1;

    [Header("Колоды игроков (в инспекторе)")]
    public List<CardData> playerDeck = new();
    public List<CardData> enemyDeck = new();

    public List<CardData> playerHand { get; private set; } = new();
    public List<CardData> enemyHand { get; private set; } = new();

    // Событие: карта разыграна (для UI)
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
    /// Вызывается в Setup() GameManager: раздаёт стартовые руки.
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
    /// Вызывается в StartTurn() GameManager: добираем карту.
    /// </summary>
    public void DrawTurnCards(bool isPlayer)
    {
        if (isPlayer)
            DrawCard(playerDeck, playerHand);
        else
            DrawCard(enemyDeck, enemyHand);
    }

    private void DrawCard(List<CardData> deck, List<CardData> hand)
    {
        if (deck.Count == 0) return;
        var card = deck[0];
        deck.RemoveAt(0);
        hand.Add(card);
        Debug.Log($"{(deck == playerDeck ? "Player" : "AI")} draws {card.cardName}");
    }

    /// <summary>
    /// Вызываем, когда хотим разыграть карту из руки.
    /// </summary>
    public bool TryPlayCard(CardData cardData, bool isPlayer)
    {
        if (isPlayer && playerHand.Contains(cardData))
        {
            OnCardPlayed?.Invoke(cardData);
            playerHand.Remove(cardData);
            return true;
        }
        else if (!isPlayer && enemyHand.Contains(cardData))
        {
            OnCardPlayed?.Invoke(cardData);
            enemyHand.Remove(cardData);
            return true;
        }
        return false;
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
