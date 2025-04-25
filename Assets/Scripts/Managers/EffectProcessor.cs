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
    /// Вызывается при розыгрыше способности. Сохраняем CardData для таргетинга.
    /// </summary>
    public void BeginTargeting(CardData data)
    {
        // просто храним карту
    }

    /// <summary>
    /// Применяем все эффекты карты к выбранному экземпляру.
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

        //  если карта HeroAbility — применить к себе (герою) автоматически без таргета
        // И после применения убирать карту из боя/руки, если нужно.
    }
}
