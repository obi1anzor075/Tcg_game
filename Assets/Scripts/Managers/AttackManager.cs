using System.Linq;
using UnityEngine;

#region Manager: AttackManager

/// <summary>
/// Отвечает за автоматическую фазу атаки:
/// все атакующие карты с loyalty > 0 автоматически наносят урон по целям.
/// Учтены пассивные способности «приоритетная цель» и триггеры OnAttack/OnDamaged.
/// </summary>
public class AttackManager : MonoBehaviour
{
    #region Singleton

    public static AttackManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// Разрешает фазу атаки: все свои карты с loyalty > 0 автоматически бьют по врагу.
    /// </summary>
    /// <param name="isPlayerTurn">true — ход игрока, false — ход ИИ</param>
    public void ResolveAttackPhase(bool isPlayerTurn)
    {
        var attackers = isPlayerTurn
            ? TurnManager.Instance.PlayerCards
            : TurnManager.Instance.EnemyCards;

        var defenders = isPlayerTurn
            ? TurnManager.Instance.EnemyCards
            : TurnManager.Instance.PlayerCards;

        // нет атакующих или нет целей — выходим
        if (attackers.Count == 0 || defenders.Count == 0)
            return;

        // ищем приоритетную цель среди живых, у которых есть пассивная AbilityData.isPriorityTarget
        CardInstance priority = defenders
            .FirstOrDefault(d =>
                d.IsAlive
                && d.cardData.abilities != null
                && d.cardData.abilities.Any(ab => ab.isPriorityTarget)
            );

        // каждый атакующий с CanAct
        foreach (var atk in attackers)
        {
            if (!atk.IsAlive || !atk.CanAct)
                continue;

            // триггерим OnAttack
            atk.OnAttack();

            // выбираем цель: сначала приоритет, потом любой живой миньон, потом герой
            CardInstance target = null;

            if (priority != null && priority.IsAlive)
            {
                target = priority;
            }
            else
            {
                // живые миньоны
                target = defenders.FirstOrDefault(d => d.IsAlive && d.cardData.type == CardType.Minion)
                      ?? defenders.FirstOrDefault(d => d.IsAlive && d.cardData.type == CardType.Hero);
            }

            if (target == null)
                break; // все цели мертвы

            // наносим урон: сначала защиту, затем HP
            target.TakeDamage(atk.cardData.attack);
            // триггерим OnDamaged у цели
            target.OnDamaged();

            // если priority погиб — сбрасываем
            if (priority != null && !priority.IsAlive)
                priority = null;
        }

        // убираем погибших из списков
        TurnManager.Instance.RemoveDeadCards();

        // обновляем UI
        UIManager.Instance.RefreshBattlefield();
    }

    #endregion
}

#endregion
