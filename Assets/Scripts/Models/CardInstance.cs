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

    public readonly CardData cardData;

    public int currentHP;
    public int currentLoyalty;
    private int _currentDefense;

    public bool IsAlive => currentHP > 0;
    public bool CanAct => currentLoyalty > 0;
    public int CurrentDefense => _currentDefense;

    #endregion

    #region Constructor

    public CardInstance(CardData data)
    {
        cardData = data;
        currentHP = data.maxHP;
        currentLoyalty = data.baseLoyalty;
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
