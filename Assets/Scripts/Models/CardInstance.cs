<<<<<<< HEAD
using System.Linq;
using UnityEngine;

#region Model: CardInstance

/// <summary>
/// Экземпляр карты в игре: хранит текущее HP, защиту, преданность и способности,
/// а также логику получения урона, сброса преданности и срабатывания способностей.
/// </summary>
public class CardInstance
{
    #region Fields & Properties

=======
using System;
using UnityEngine;

/// <summary>
/// Represents an instance of a card in the game with its own state.
/// </summary>
public class CardInstance
{
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public readonly CardData cardData;

    // Current stats
    public int currentHealth;
    public int currentAttack;
    public int currentLoyalty;
<<<<<<< HEAD
    private int _currentDefense;

    public bool IsAlive => currentHP > 0;
    public bool CanAct => currentLoyalty > 0;
    public int CurrentDefense => _currentDefense;

    #endregion

    #region Constructor
=======
    public int currentDefense;

    // Properties
    public bool IsAlive => currentHealth > 0;
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

    /// <summary>
    /// Current defense value, used by UI and game logic.
    /// </summary>
    public int CurrentDefense => currentDefense;

    /// <summary>
    /// Current HP value, used by UI and game logic.
    /// </summary>
    public int currentHP => currentHealth;

    /// <summary>
    /// Creates a new instance of a card from its data.
    /// </summary>
    public CardInstance(CardData data)
    {
        cardData = data;
        currentHealth = data.baseHealth;
        currentAttack = data.baseAttack;
        currentLoyalty = data.baseLoyalty;
<<<<<<< HEAD
        _currentDefense = data.baseDefense;
    }

    #endregion

    #region Loyalty & Defense

    public void ResetLoyalty() => currentLoyalty = cardData.baseLoyalty;
    public void ModifyLoyalty(int delta) => currentLoyalty += delta;
    public void ModifyDefense(int delta) => _currentDefense = Mathf.Max(0, _currentDefense + delta);

    #endregion

    #region Damage

    /// <summary>
    /// Получает урон: сначала вычитаем из защиты, остаток из HP.
    /// </summary>
    public void TakeDamage(int amount)
    {
        int absorbed = Mathf.Min(_currentDefense, amount);
        _currentDefense -= absorbed;

        int leftover = amount - absorbed;
        currentHP = Mathf.Max(0, currentHP - leftover);

        // Триггерим способность при получении урона
        ProcessAbilities(AbilityTrigger.OnDamaged);
    }

    #endregion

    #region Ability Triggers

    #region Ability Triggers

    /// <summary>Вызывается при выходе карты на поле.</summary>
    public void OnPlay() => ProcessAbilities(AbilityTrigger.OnPlay);

    /// <summary>Вызывается в начале хода владельца карты.</summary>
    public void OnTurnStart() => ProcessAbilities(AbilityTrigger.OnTurnStart);

    /// <summary>Вызывается в момент, когда карта атакует.</summary>
    public void OnAttack() => ProcessAbilities(AbilityTrigger.OnAttack);

    /// <summary>Вызывается после того, как карта получает урон.</summary>
    public void OnDamaged() => ProcessAbilities(AbilityTrigger.OnDamaged);

    #endregion


    #endregion

    #region Ability Processing

    private void ProcessAbilities(AbilityTrigger trigger)
    {
        // Проходим по всем способностям, заданным в CardData
        foreach (var ab in cardData.abilities)
        {
            // пассивные способности всегда срабатывают
            if (ab.trigger == AbilityTrigger.Passive || ab.trigger == trigger)
                ApplyEffects(ab.effects);
        }
    }

    private void ApplyEffects(EffectData[] effects)
    {
        foreach (var eff in effects)
        {
            switch (eff.type)
            {
                case EffectType.Damage:
                    // бьем по противнику (нужен внешний вызов с целью)
                    break;
                case EffectType.Heal:
                    currentHP = Mathf.Min(cardData.maxHP, currentHP + eff.value);
                    break;
                case EffectType.ModifyLoyalty:
                    ModifyLoyalty(eff.value);
                    break;
                case EffectType.ModifyDefense:
                    ModifyDefense(eff.value);
                    break;
                    // другие эффекты…
            }
        }
    }

    #endregion
}

#endregion
=======
        currentDefense = data.baseDefense;
    }

    /// <summary>
    /// Apply damage to this card, reducing its health.
    /// First reduces defense if available, then applies remaining damage to health.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        // First, damage is applied to defense
        if (currentDefense > 0)
        {
            int absorbedByDefense = Mathf.Min(currentDefense, amount);
            currentDefense -= absorbedByDefense;
            amount -= absorbedByDefense;

            Debug.Log($"[CardInstance] {cardData.cardName} absorbed {absorbedByDefense} damage with defense. Defense now: {currentDefense}");

            // If defense absorbed all damage, we're done
            if (amount <= 0) return;
        }

        // Apply remaining damage to health
        currentHealth -= amount;

        // Ensure health doesn't go below 0
        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log($"[CardInstance] {cardData.cardName} took {amount} damage to health. Health now: {currentHealth}");
    }

    /// <summary>
    /// Modifies the card's loyalty value.
    /// </summary>
    public void ModifyLoyalty(int amount)
    {
        currentLoyalty = Mathf.Max(0, currentLoyalty + amount);
        Debug.Log($"[CardInstance] {cardData.cardName} loyalty changed by {amount}. New loyalty: {currentLoyalty}");
    }

    /// <summary>
    /// Modifies the card's defense value.
    /// </summary>
    public void ModifyDefense(int amount)
    {
        currentDefense = Mathf.Max(0, currentDefense + amount);
        Debug.Log($"[CardInstance] {cardData.cardName} defense changed by {amount}. New defense: {currentDefense}");
    }

    /// <summary>
    /// Resets loyalty to its base value at the start of a turn.
    /// </summary>
    public int ResetLoyalty()
    {
        if (cardData.type == CardType.Minion || cardData.type == CardType.Hero)
        {
            currentLoyalty = cardData.baseLoyalty;
        }
        return currentLoyalty;
    }
}
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
