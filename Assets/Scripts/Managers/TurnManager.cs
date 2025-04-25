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
            // сброс и получение лояльности из каждой карты
            card.ResetLoyalty();
            if (card.currentLoyalty > 0)
                _totalPlayerLoyalty += card.currentLoyalty;
        }

        Debug.Log($"[TurnManager] New turn. Total Loyalty = {_totalPlayerLoyalty}");
    }


    /// <summary>
    /// Пытается разыграть карту. Если это миньон — добавляет на поле и возвращает экземпляр; 
    /// если способность — применяет её. В обоих случаях уменьшает ресурс лояльности.
    /// </summary>
    public CardInstance TryPlayCard(CardData cardData)
    {
        // стоимость карты:
        int cost = cardData.type == CardType.Minion
            ? cardData.baseLoyalty
            : cardData.loyaltyCost;

        if (cost > _totalPlayerLoyalty)
            return null; // недостаточно преданности

        // уменьшаем ресурс
        _totalPlayerLoyalty -= cost;

        if (cardData.type == CardType.Minion)
        {
            var inst = new CardInstance(cardData);
            _playerCards.Add(inst);
            return inst;
        }
        else
        {
            // способности
            ApplyCardEffect(cardData);
            return null;
        }
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

    /// <summary>Списывает указанную преданность (без создания CardInstance).</summary>
    public bool TrySpendLoyalty(int amount)
    {
        if (amount > _totalPlayerLoyalty) return false;
        _totalPlayerLoyalty -= amount;
        return true;
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
