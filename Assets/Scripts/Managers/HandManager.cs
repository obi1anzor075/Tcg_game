using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Manager: HandManager

/// <summary>
/// Отвечает за колоды, руку игрока и ИИ, а также за добор и розыгрыш карт из руки.
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
    [Tooltip("Размер стартовой руки для каждого игрока")]
    [SerializeField] private int _startHandSize = 5;

    [Header("Decks (assign in Inspector)")]
    [Tooltip("Колода игрока")]
    [SerializeField] private List<CardData> _playerDeck = new();
    [Tooltip("Колода ИИ")]
    [SerializeField] private List<CardData> _enemyDeck = new();

    #endregion

    #region Public State

    /// <summary>Текущая рука игрока.</summary>
    public List<CardData> playerHand { get; private set; } = new();

    /// <summary>Текущая рука ИИ.</summary>
    public List<CardData> enemyHand { get; private set; } = new();

    /// <summary>Событие: игрок/ИИ разыграл карту (для UI и логики).</summary>
    public event Action<CardData> OnCardPlayed;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // При старте можно автоматически раздать стартовые руки
        // или вызывать DealStartHands() из GameManager.Setup()
    }

    #endregion

    #region Public API

    /// <summary>
    /// Перетасовывает колоды и раздаёт по _startHandSize карт игроку и ИИ.
    /// Вызывать один раз в Setup.
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
    /// Добирает по одной карте для игрока или ИИ на начало хода.
    /// </summary>
    /// <param name="isPlayer">true — игрок, false — ИИ</param>
    public void DrawTurnCards(bool isPlayer)
    {
        if (isPlayer)
            DrawCard(_playerDeck, playerHand);
        else
            DrawCard(_enemyDeck, enemyHand);
    }

    /// <summary>
    /// Пытается разыграть карту из руки.
    /// Убирает её из списка и генерирует событие OnCardPlayed.
    /// </summary>
    /// <param name="cardData">Данные карты</param>
    /// <param name="isPlayer">true — ход игрока, false — ход ИИ</param>
    /// <returns>true, если карта была в руке и удалена</returns>
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
    /// Вытягивает верхнюю карту из deck в hand.
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
    /// Перемешивает список карт в случайном порядке.
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
    /// Извлекает первую карту типа Minion из колоды игрока и возвращает её.
    /// </summary>
    public CardData DrawFirstMinionFromPlayerDeck()
    {
        var minion = _playerDeck.FirstOrDefault(c => c.type == CardType.Minion);
        if (minion != null) _playerDeck.Remove(minion);
        return minion;
    }

    /// <summary>
    /// Аналог для ИИ.
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
