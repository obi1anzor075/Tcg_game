using UnityEngine;

#region Manager: EffectProcessor

/// <summary>
/// ������������ ���������� �������� ���� (�����������, ����������) � ������� �����������.
/// </summary>
public class EffectProcessor : MonoBehaviour
{
    #region Singleton

    public static EffectProcessor Instance { get; private set; }

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

    #region State

    /// <summary>
    /// ��������� �� ��� �����, � ������ �� �����������, �� ������� ��������.
    /// </summary>
    private AbilityData _pendingAbility;

    #endregion

    #region Public API

    /// <summary>
    /// �������� ���������; ��������� ��������� ����������� (AbilityData), � �� ��� �����.
    /// </summary>
    public void BeginTargeting(AbilityData ability)
    {
        _pendingAbility = ability;
    }

    /// <summary>
    /// ��������� ��� ������� ���������� ����������� � ���������� ���������� ����.
    /// </summary>
    public void ApplyEffectTo(CardInstance target)
    {
        if (_pendingAbility == null || target == null)
            return;

        foreach (var eff in _pendingAbility.effects)
        {
            switch (eff.type)
            {
                case EffectType.Damage:
                    target.TakeDamage(eff.value);
                    break;
                case EffectType.Heal:
                    target.currentHP = Mathf.Min(target.cardData.maxHP, target.currentHP + eff.value);
                    break;
                case EffectType.ModifyLoyalty:
                    target.ModifyLoyalty(eff.value);
                    break;
                case EffectType.ModifyDefense:
                    target.ModifyDefense(eff.value);
                    break;
                    // �������� ������ ���� �������� �� �������������
            }
        }

        // �������� ���������
        _pendingAbility = null;

        // ��������� ����, ����� UI ������� ����� HP/���/�����������
        UIManager.Instance.RefreshBattlefield();
    }

    #endregion
}

#endregion
