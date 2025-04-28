using UnityEngine;

#region Manager: EffectProcessor

/// <summary>
/// Обрабатывает применение эффектов карт (способности, заклинания) к целевым экземплярам.
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
    /// Сохраняем не всю карту, а именно ту способность, по которой кликнули.
    /// </summary>
    private AbilityData _pendingAbility;

    #endregion

    #region Public API

    /// <summary>
    /// Начинаем таргетинг; сохраняем выбранную способность (AbilityData), а не всю карту.
    /// </summary>
    public void BeginTargeting(AbilityData ability)
    {
        _pendingAbility = ability;
    }

    /// <summary>
    /// Применяем все эффекты сохранённой способности к указанному экземпляру цели.
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
                    // добавить другие типы эффектов по необходимости
            }
        }

        // Сбросили таргетинг
        _pendingAbility = null;

        // Обновляем поле, чтобы UI показал новые HP/щит/преданность
        UIManager.Instance.RefreshBattlefield();
    }

    #endregion
}

#endregion
