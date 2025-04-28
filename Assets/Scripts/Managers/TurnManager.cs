using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Manager: TurnManager

/// <summary>
/// Отвечает за хранение карт на поле, ресурс преданности и розыгрыш карт.
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

    /// <summary>Текущие карты игрока на поле.</summary>
    public IReadOnlyList<CardInstance> PlayerCards => _playerCards;

    /// <summary>Текущие карты ИИ на поле.</summary>
    public IReadOnlyList<CardInstance> EnemyCards => _enemyCards;

    /// <summary>Текущий ресурс преданности игрока.</summary>
    public int PlayerLoyalty => _playerLoyalty;

    /// <summary>Текущий ресурс преданности ИИ.</summary>
    public int EnemyLoyalty => _enemyLoyalty;

    #endregion

    #region Turn Management

    /// <summary>
    /// Начало хода: сброс и подсчет преданности.
    /// </summary>
    /// <param name="isPlayer">true — игрок, false — ИИ</param>
    public void StartTurn(bool isPlayer)
    {
        if (isPlayer)
        {
            _playerLoyalty = _playerCards
                .Select(c => { c.ResetLoyalty(); return c.currentLoyalty; })
                .Where(l => l > 0)
                .Sum();
            Debug.Log($"[TurnManager] Player turn started. Loyalty = {_playerLoyalty}");
        }
        else
        {
            _enemyLoyalty = _enemyCards
                .Select(c => { c.ResetLoyalty(); return c.currentLoyalty; })
                .Where(l => l > 0)
                .Sum();
            Debug.Log($"[TurnManager] Enemy turn started. Loyalty = {_enemyLoyalty}");
        }
    }

    #endregion

    #region Play Card

    /// <summary>
    /// Пытается разыграть карту для игрока или ИИ.
    /// Миньон выходит на поле, спеллы/геро. способности применяются сразу.
    /// </summary>
    /// <param name="data">Данные карты.</param>
    /// <param name="isPlayer">true — ход игрока, false — ход ИИ.</param>
    /// <returns>Экземпляр миньона, если это Minion; иначе null.</returns>
    public CardInstance TryPlayCard(CardData data, bool isPlayer)
    {
        // выбираем нужный пул преданности
        ref int loyalty = ref (isPlayer ? ref _playerLoyalty : ref _enemyLoyalty);

        int cost = data.type == CardType.Minion
            ? data.baseLoyalty
            : data.loyaltyCost;

        if (cost > loyalty)
            return null;

        loyalty -= cost;

        if (data.type == CardType.Minion)
        {
            var inst = new CardInstance(data);
            if (isPlayer) _playerCards.Add(inst);
            else _enemyCards.Add(inst);
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

    /// <summary>Adds a pre-created card instance to the field for player.</summary>
    public void AddPlayerCard(CardInstance inst) => _playerCards.Add(inst);

    /// <summary>Adds a pre-created card instance to the field for AI.</summary>
    public void AddEnemyCard(CardInstance inst) => _enemyCards.Add(inst);

    /// <summary>
    /// Удаляет указанные карты с поля.
    /// </summary>
    public void RemoveCardsFromField(IEnumerable<CardInstance> toRemove)
    {
        _playerCards.RemoveAll(c => toRemove.Contains(c));
        _enemyCards.RemoveAll(c => toRemove.Contains(c));
    }

    /// <summary>
    /// Удаляет погибшие (HP <= 0) карты с поля.
    /// </summary>
    public void RemoveDeadCards()
    {
        _playerCards.RemoveAll(c => !c.IsAlive);
        _enemyCards.RemoveAll(c => !c.IsAlive);
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Применяет мгновенные эффекты (OnPlay, Passive) для спеллов и героических умений.
    /// </summary>
    private void ApplyCardEffect(CardData data, bool isPlayer)
    {
        // Проходим по всем способностям, которые сработают при розыгрыше
        var toApply = data.abilities
            .Where(a => a.trigger == AbilityTrigger.OnPlay || a.trigger == AbilityTrigger.Passive);

        foreach (var ab in toApply)
        {
            Debug.Log($"[TurnManager] Applying ability {ab.abilityName}");
            foreach (var eff in ab.effects)
            {
                Debug.Log($"    Effect {eff.type}, value={eff.value}");
                // TODO: в зависимости от eff.targetsOwnPlayer
                // применить к героям или миньонам нужной стороны
            }
        }
    }

    #endregion
}

#endregion
