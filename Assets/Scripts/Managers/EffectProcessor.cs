using UnityEngine;

public class EffectProcessor : MonoBehaviour
{
    public static EffectProcessor Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    /// <summary>
    /// ���������� ��� ��������� �����������. ��������� CardData ��� ����������.
    /// </summary>
    public void BeginTargeting(CardData data)
    {
        // ������ ������ �����
    }

    /// <summary>
    /// ��������� ��� ������� ����� � ���������� ����������.
    /// </summary>
    public void ApplyEffectTo(CardInstance target, CardData effectCard)
    {
        foreach (var eff in effectCard.effects)
        {
            if (eff.damage != 0)
                target.TakeDamage(eff.damage);
            if (eff.heal != 0)
                target.currentHP = Mathf.Min(target.cardData.maxHP, target.currentHP + eff.heal);
            if (eff.loyaltyDelta != 0)
                target.ModifyLoyalty(eff.loyaltyDelta);
        }

        //  ���� ����� HeroAbility � ��������� � ���� (�����) ������������� ��� �������
        // � ����� ���������� ������� ����� �� ���/����, ���� �����.
    }
}
