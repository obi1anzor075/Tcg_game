using System.Collections.Generic;
using UnityEngine;

#region Manager: TurnManager

/// <summary>
/// Отвечает за ресурс "лояльность", розыгрыш карт и хранение карт на поле.
/// </summary>
public class TurnManager : MonoBehaviour
{
    #region Singleton

    public static TurnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Fields

    [Header("Карты на поле")]
    [SerializeField] private List<CardInstance> _playerCards = new();
    [SerializeField] private List<CardInstance> _enemyCards = new();

    private int _totalPlayerLoyalty;

    #endregion

    #region Properties

    /// <summary>Список карт игрока на поле.</summary>
    public IReadOnlyList<CardInstance> PlayerCards => _playerCards;

    /// <summary>Список карт ИИ на поле.</summary>
    public IReadOnlyList<CardInstance> EnemyCards => _enemyCards;

    /// <summary>Текущий ресурс лояльности игрока.</summary>
    public int TotalPlayerLoyalty => _totalPlayerLoyalty;

    #endregion

    #region Public Methods

    /// <summary>
    /// Начало хода: сбрасывает и суммирует лояльность у всех карт игрока.
    /// </summary>
    public void StartTurn()
    {
        _totalPlayerLoyalty = 0;

        foreach (var card in _playerCards)
        {
            card.ResetLoyalty();
            if (card.currentLoyalty > 0)
                _totalPlayerLoyalty += card.currentLoyalty;
        }

        Debug.Log($"[TurnManager] StartTurn — Total Loyalty: {_totalPlayerLoyalty}");
    }

    /// <summary>
    /// Разыгрывает карту: миньон выходит на поле, способность применяется.
    /// </summary>
    /// <returns>true если карта сыграна, иначе false.</returns>
    public bool PlayCard(CardData cardData)
    {
        if (cardData.type == CardType.Minion &&
            cardData.baseLoyalty <= _totalPlayerLoyalty)
        {
            var newCard = new CardInstance(cardData);
            _playerCards.Add(newCard);
            _totalPlayerLoyalty -= cardData.baseLoyalty;
            return true;
        }
        else if ((cardData.type == CardType.Spell || cardData.type == CardType.HeroAbility) &&
                  cardData.loyaltyCost <= _totalPlayerLoyalty)
        {
            ApplyCardEffect(cardData);
            _totalPlayerLoyalty -= cardData.loyaltyCost;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Добавляет карту на поле
    /// </summary>
    public void AddPlayerCard(CardInstance instance)
    {
        _playerCards.Add(instance);
    }

    /// <summary>
    /// Удаляет данную карту с поля (игрока или ИИ).
    /// </summary>
    /// <returns>true если карта была найдена и удалена.</returns>
    public void RemoveDeadCards()
    {
        _playerCards.RemoveAll(c => !c.IsAlive);
        _enemyCards.RemoveAll(c => !c.IsAlive);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Применяет эффекты урона, лечения и изменения лояльности одной карты.
    /// </summary>
    private void ApplyCardEffect(CardData cardData)
    {
        // TODO: внедрить таргетинг; пока просто логирование
        foreach (var eff in cardData.effects)
        {
            Debug.Log($"[TurnManager] Applying effect {eff.abilityName} (damage {eff.damage}, heal {eff.heal}, loyalty {eff.loyaltyDelta})");
            // пример: target.ModifyLoyalty(eff.loyaltyDelta);
        }
    }

    #endregion
}

#endregion
