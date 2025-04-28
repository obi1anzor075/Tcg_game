using System.Linq;
using UnityEngine;

#region Model: CardInstance

/// <summary>
/// ��������� ����� � ����: ������ ������� HP, ������, ����������� � �����������,
/// � ����� ������ ��������� �����, ������ ����������� � ������������ ������������.
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
    /// �������� ����: ������� �������� �� ������, ������� �� HP.
    /// </summary>
    public void TakeDamage(int amount)
    {
        int absorbed = Mathf.Min(_currentDefense, amount);
        _currentDefense -= absorbed;

        int leftover = amount - absorbed;
        currentHP = Mathf.Max(0, currentHP - leftover);

        // ��������� ����������� ��� ��������� �����
        ProcessAbilities(AbilityTrigger.OnDamaged);
    }

    #endregion

    #region Ability Triggers

    #region Ability Triggers

    /// <summary>���������� ��� ������ ����� �� ����.</summary>
    public void OnPlay() => ProcessAbilities(AbilityTrigger.OnPlay);

    /// <summary>���������� � ������ ���� ��������� �����.</summary>
    public void OnTurnStart() => ProcessAbilities(AbilityTrigger.OnTurnStart);

    /// <summary>���������� � ������, ����� ����� �������.</summary>
    public void OnAttack() => ProcessAbilities(AbilityTrigger.OnAttack);

    /// <summary>���������� ����� ����, ��� ����� �������� ����.</summary>
    public void OnDamaged() => ProcessAbilities(AbilityTrigger.OnDamaged);

    #endregion


    #endregion

    #region Ability Processing

    private void ProcessAbilities(AbilityTrigger trigger)
    {
        // �������� �� ���� ������������, �������� � CardData
        foreach (var ab in cardData.abilities)
        {
            // ��������� ����������� ������ �����������
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
                    // ���� �� ���������� (����� ������� ����� � �����)
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
                    // ������ ��������
            }
        }
    }

    #endregion
}

#endregion
