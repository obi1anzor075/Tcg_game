using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Manager: TurnManager

/// <summary>
/// Отвечает за "преданность", розыгрыш карт и хранение карт на поле.
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
    private int _totalEnemyLoyalty;

    #endregion

    #region Properties

    /// <summary>Копии списка карт игрока (readonly).</summary>
    public IReadOnlyList<CardInstance> PlayerCards => _playerCards;
    /// <summary>Копии списка карт ИИ (readonly).</summary>
    public IReadOnlyList<CardInstance> EnemyCards => _enemyCards;
    /// <summary>Текущий ресурс лояльности игрока.</summary>
    public int TotalPlayerLoyalty => _totalPlayerLoyalty;
    /// <summary>Текущий ресурс лояльности ИИ.</summary>
    public int TotalEnemyLoyalty => _totalEnemyLoyalty;

    #endregion

    #region Turn Management

    /// <summary>Начало хода игрока: сброс и суммирование лояльности.</summary>
    public void StartPlayerTurn()
    {
        _totalPlayerLoyalty = _playerCards
            .Select(card => { card.ResetLoyalty(); return card.currentLoyalty; })
            .Where(l => l > 0)
            .Sum();
        Debug.Log($"[TurnManager] Player turn. Total Loyalty = {_totalPlayerLoyalty}");
    }

    /// <summary>Начало хода ИИ: сброс и суммирование лояльности.</summary>
    public void StartEnemyTurn()
    {
        _totalEnemyLoyalty = _enemyCards
            .Select(card => { card.ResetLoyalty(); return card.currentLoyalty; })
            .Where(l => l > 0)
            .Sum();
        Debug.Log($"[TurnManager] Enemy turn. Total Loyalty = {_totalEnemyLoyalty}");
    }

    #endregion

    #region Play Card Methods

    public CardInstance TryPlayPlayerCard(CardData data)
    {
        int cost = data.type == CardType.Minion
            ? data.baseLoyalty
            : data.loyaltyCost;
        if (cost > _totalPlayerLoyalty) return null;
        _totalPlayerLoyalty -= cost;

        if (data.type == CardType.Minion)
        {
            var inst = new CardInstance(data);
            _playerCards.Add(inst);
            // триггер OnPlay для миньона
            inst.OnPlay();
            return inst;
        }
        else
        {
            ApplyCardEffect(data);
            return null;
        }
    }

    public CardInstance TryPlayEnemyCard(CardData data)
    {
        int cost = data.type == CardType.Minion
            ? data.baseLoyalty
            : data.loyaltyCost;
        if (cost > _totalEnemyLoyalty) return null;
        _totalEnemyLoyalty -= cost;

        if (data.type == CardType.Minion)
        {
            var inst = new CardInstance(data);
            _enemyCards.Add(inst);
            inst.OnPlay();
            return inst;
        }
        else
        {
            ApplyCardEffect(data);
            return null;
        }
    }

    #endregion

    #region Field Management

    public void AddPlayerCard(CardInstance inst) => _playerCards.Add(inst);
    public void AddEnemyCard(CardInstance inst) => _enemyCards.Add(inst);

    public void RemoveCardsFromField(IEnumerable<CardInstance> toRemove)
    {
        _playerCards.RemoveAll(c => toRemove.Contains(c));
        _enemyCards.RemoveAll(c => toRemove.Contains(c));
    }

    public void RemoveDeadCards()
    {
        _playerCards.RemoveAll(c => !c.IsAlive);
        _enemyCards.RemoveAll(c => !c.IsAlive);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Применяет мгновенные эффекты способностей (trigger == OnPlay или Passive) для спеллов и героических умений.
    /// </summary>
    private void ApplyCardEffect(CardData data)
    {
        // Для каждой способности у карты, срабатывающей OnPlay или Passive
        foreach (var ab in data.abilities.Where(a => a.trigger == AbilityTrigger.OnPlay || a.trigger == AbilityTrigger.Passive))
        {
            Debug.Log($"[TurnManager] Applying ability {ab.abilityName}");
            foreach (var eff in ab.effects)
            {
                Debug.Log($"    Effect {eff.type}: value={eff.value}, targetOwn={eff.targetsOwnPlayer}");
                // Здесь можно сразу применять эффекты без таргетинга:
                // например, если eff.targetsOwnPlayer → применять к герою игрока/ИИ
                // иначе — к противоположному герою.
            }
        }
    }

    #endregion
}

#endregion
