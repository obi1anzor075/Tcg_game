using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#region Manager: AttackManager

/// <summary>
/// Управляет автоматическим разрешением фазы атаки для игрока и ИИ.
/// Все атакующие карты автоматически наносят удары по целям.
/// Поддерживает приоритетные цели, пока они живы.
/// </summary>
public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance { get; private set; }

    #region Unity Methods

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

    #region Public API

    /// <summary>
    /// Разрешает фазу атаки: все карты атакующей стороны автоматически наносят удары.
    /// </summary>
    /// <param name="isPlayerTurn">true — атака игрока, false — атака ИИ</param>
    public void ResolveAttackPhase(bool isPlayerTurn)
    {
        // выбираем списки атакующих и защищающихся
        var attackers = isPlayerTurn
            ? TurnManager.Instance.PlayerCards
            : TurnManager.Instance.EnemyCards;

        var defenders = isPlayerTurn
            ? TurnManager.Instance.EnemyCards
            : TurnManager.Instance.PlayerCards;

        if (attackers.Count == 0 || defenders.Count == 0)
            return;

        // Находим приоритетную цель — первая живая карта с приоритетной способностью
        var priorityTarget = defenders.FirstOrDefault(c =>
            c.cardData.ability != null &&
            c.cardData.ability.isPriorityTarget &&
            c.IsAlive);

        // Для каждого атакующего, у которого loyalty > 0
        foreach (var attacker in attackers)
        {
            if (!attacker.CanAct || !attacker.IsAlive)
                continue;

            // выбираем цель: либо приоритетная, либо первая живая
            var target = (priorityTarget != null && priorityTarget.IsAlive)
                ? priorityTarget
                : defenders.FirstOrDefault(c => c.IsAlive);

            if (target == null)
                break; // все цели уничтожены

            // Наносим урон
            ResolveOneAttack(attacker, target);

            // Если приоритетная цель погибла — сбросим её
            if (priorityTarget != null && !priorityTarget.IsAlive)
            {
                priorityTarget = null;
            }
        }

        // Удаляем погибшие карты и обновляем UI
        CleanupDeadUnits(isPlayerTurn);
        UIManager.Instance.RefreshBattlefield();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Наносит урон между двумя картами: атакующий vs цель.
    /// </summary>
    private void ResolveOneAttack(CardInstance attacker, CardInstance defender)
    {
        defender.TakeDamage(attacker.cardData.attack);
        // отражённый урон
        attacker.TakeDamage(defender.cardData.attack);
    }

    /// <summary>
    /// Удаляет погибшие карты из списков TurnManager.
    /// </summary>
    private void CleanupDeadUnits(bool wasPlayerAttacking)
    {
        var ownList = wasPlayerAttacking
            ? TurnManager.Instance.PlayerCards
            : TurnManager.Instance.EnemyCards;
        var enemyList = wasPlayerAttacking
            ? TurnManager.Instance.EnemyCards
            : TurnManager.Instance.PlayerCards;

        TurnManager.Instance.RemoveDeadCards();
    }

    #endregion
}

#endregion
