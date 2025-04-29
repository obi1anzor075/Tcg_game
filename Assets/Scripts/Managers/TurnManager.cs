<<<<<<< HEAD
﻿using System.Collections.Generic;
=======
﻿using System;
using System.Collections.Generic;
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
using System.Linq;
using UnityEngine;

/// <summary>
<<<<<<< HEAD
/// Отвечает за "преданность", розыгрыш карт и хранение карт на поле.
=======
/// Отвечает за хранение карт на поле, ресурс преданности и розыгрыш карт.
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
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
<<<<<<< HEAD
=======
    private int _currentTurn = 0;
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

    #endregion

    #region Properties

<<<<<<< HEAD
    /// <summary>Копии списка карт игрока (readonly).</summary>
    public IReadOnlyList<CardInstance> PlayerCards => _playerCards;
    /// <summary>Копии списка карт ИИ (readonly).</summary>
    public IReadOnlyList<CardInstance> EnemyCards => _enemyCards;
    /// <summary>Текущий ресурс преданности игрока.</summary>
    public int PlayerLoyalty => _playerLoyalty;
    /// <summary>Текущий ресурс преданности ИИ.</summary>
    public int EnemyLoyalty => _enemyLoyalty;
=======
    /// <summary>Текущие карты игрока на поле.</summary>
    public IReadOnlyList<CardInstance> PlayerCards => _playerCards;

    /// <summary>Текущие карты ИИ на поле.</summary>
    public IReadOnlyList<CardInstance> EnemyCards => _enemyCards;

    /// <summary>Текущий ресурс преданности игрока.</summary>
    public int PlayerLoyalty => _playerLoyalty;

    /// <summary>Текущий ресурс преданности ИИ.</summary>
    public int EnemyLoyalty => _enemyLoyalty;

    /// <summary>Текущий номер хода (начиная с 1).</summary>
    public int CurrentTurn => _currentTurn;
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

    #endregion

    #region Turn Management

    /// <summary>Начало хода игрока: сброс и суммирование лояльности.</summary>
    /// <summary>
<<<<<<< HEAD
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
=======
    /// Начало хода: сброс и подсчет преданности.
    /// </summary>
    /// <param name="isPlayer">true — игрок, false — ИИ</param>
    public void StartTurn(bool isPlayer)
    {
        // Увеличиваем счетчик ходов в начале хода игрока
        if (isPlayer)
        {
            _currentTurn++;
        }

        if (isPlayer)
        {
            _playerLoyalty = _playerCards
                .Select(c => { c.ResetLoyalty(); return c.currentLoyalty; })
                .Where(l => l > 0)
                .Sum();
            Debug.Log($"[TurnManager] Player turn {_currentTurn} started. Loyalty = {_playerLoyalty}");
        }
        else
        {
            _enemyLoyalty = _enemyCards
                .Select(c => { c.ResetLoyalty(); return c.currentLoyalty; })
                .Where(l => l > 0)
                .Sum();
            Debug.Log($"[TurnManager] Enemy turn {_currentTurn} started. Loyalty = {_enemyLoyalty}");
        }
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    }

    #endregion

    #region Play Card

    #endregion

    #region Play Card Methods

    /// <summary>
<<<<<<< HEAD
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
=======
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
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
            return inst;
        }
        else
        {
            ApplyCardEffect(data, isPlayer);
            return null;
        }
    }

<<<<<<< HEAD

=======
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    #endregion

    #region Field Management

<<<<<<< HEAD
    public void AddPlayerCard(CardInstance inst) => _playerCards.Add(inst);
    public void AddEnemyCard(CardInstance inst) => _enemyCards.Add(inst);


    /// <summary>Убирает из поля указанные экземпляры (часть EndTurnCleanup).</summary>
=======
    /// <summary>Adds a pre-created card instance to the field for player.</summary>
    public void AddPlayerCard(CardInstance inst) => _playerCards.Add(inst);

    /// <summary>Adds a pre-created card instance to the field for AI.</summary>
    public void AddEnemyCard(CardInstance inst) => _enemyCards.Add(inst);

    /// <summary>
    /// Удаляет указанные карты с поля.
    /// </summary>
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public void RemoveCardsFromField(IEnumerable<CardInstance> toRemove)
    {
        _playerCards.RemoveAll(c => toRemove.Contains(c));
        _enemyCards.RemoveAll(c => toRemove.Contains(c));
    }

<<<<<<< HEAD
    /// <summary>Удаляет убитых (IsAlive==false) после боя или атак.</summary>
=======
    /// <summary>
    /// Удаляет погибшие (HP <= 0) карты с поля.
    /// </summary>
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public void RemoveDeadCards()
    {
        int playerDeadCount = _playerCards.Count(c => !c.IsAlive);
        int enemyDeadCount = _enemyCards.Count(c => !c.IsAlive);

        if (playerDeadCount > 0 || enemyDeadCount > 0)
        {
            Debug.Log($"[TurnManager] Removing dead cards: {playerDeadCount} player cards, {enemyDeadCount} enemy cards");
        }

        _playerCards.RemoveAll(c => !c.IsAlive);
        _enemyCards.RemoveAll(c => !c.IsAlive);
    }

    #endregion

    #region Private Helpers

    /// <summary>
<<<<<<< HEAD
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
=======
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
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
            }
        }
    }

    #endregion
}