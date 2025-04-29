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

    private int _playerLoyalty;
    private int _enemyLoyalty;

    #endregion

    #region Properties

    /// <summary>Копии списка карт игрока (readonly).</summary>
    public IReadOnlyList<CardInstance> PlayerCards => _playerCards;
    /// <summary>Копии списка карт ИИ (readonly).</summary>
    public IReadOnlyList<CardInstance> EnemyCards => _enemyCards;
    /// <summary>Текущий ресурс преданности игрока.</summary>
    public int PlayerLoyalty => _playerLoyalty;
    /// <summary>Текущий ресурс преданности ИИ.</summary>
    public int EnemyLoyalty => _enemyLoyalty;

    #endregion

    #region Turn Management

    /// <summary>Начало хода игрока: сброс и суммирование лояльности.</summary>
    /// <summary>
    /// Начало хода: сброс и суммирование преданности для игрока или ИИ.
    /// </summary>
    public void StartTurn(bool isPlayer)
    {
        if (isPlayer)
        {
            _playerLoyalty = _playerCards
                .Select(c => { c.ResetLoyalty(); return c.currentLoyalty; })
                .Where(l => l > 0).Sum();
            Debug.Log($"[TurnManager] Player turn. Loyalty = {_playerLoyalty}");
        }
        else
        {
            _enemyLoyalty = _enemyCards
                .Select(c => { c.ResetLoyalty(); return c.currentLoyalty; })
                .Where(l => l > 0).Sum();
            Debug.Log($"[TurnManager] Enemy turn. Loyalty = {_enemyLoyalty}");
        }
    }


    #endregion

    #region Play Card Methods

    /// <summary>
    /// Пытается разыграть карту (миньон или спелл) для игрока/ИИ из общей «преданности» данного.
    /// </summary>
    /// <returns>Если это миньон – новый экземпляр, иначе null (спелл).</returns>
    public CardInstance TryPlayCard(CardData data, bool isPlayer)
    {
        int cost = data.type == CardType.Minion ? data.baseLoyalty : data.loyaltyCost;
        ref int pool = ref (isPlayer ? ref _playerLoyalty : ref _enemyLoyalty);

        if (cost > pool) return null;
        pool -= cost;

        if (data.type == CardType.Minion)
        {
            var inst = new CardInstance(data);
            inst.OnPlay();
            return inst;
        }
        else
        {
            ApplyCardEffect(data, isPlayer);
            return null;
        }
    }


    #endregion

    #region Field Management

    public void AddPlayerCard(CardInstance inst) => _playerCards.Add(inst);
    public void AddEnemyCard(CardInstance inst) => _enemyCards.Add(inst);


    /// <summary>Убирает из поля указанные экземпляры (часть EndTurnCleanup).</summary>
    public void RemoveCardsFromField(IEnumerable<CardInstance> toRemove)
    {
        _playerCards.RemoveAll(c => toRemove.Contains(c));
        _enemyCards.RemoveAll(c => toRemove.Contains(c));
    }

    /// <summary>Удаляет убитых (IsAlive==false) после боя или атак.</summary>
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
    private void ApplyCardEffect(CardData data, bool isPlayer)
    {
        // Тут сразу подставьте свою логику мгновенных эффектов (OnPlay / Passive)
        foreach (var ab in data.abilities.Where(a =>
                     a.trigger == AbilityTrigger.OnPlay || a.trigger == AbilityTrigger.Passive))
        {
            foreach (var eff in ab.effects)
            {
                Debug.Log($"[TurnManager] Apply {eff.type}={eff.value} ({ab.abilityName})");
                // Например:
                // if (eff.targetsOwnPlayer) ... healing own hero
                // else ... damage enemy hero
            }
        }
    }

    #endregion
}

#endregion
