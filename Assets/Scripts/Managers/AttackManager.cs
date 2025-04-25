using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#region Manager: AttackManager

/// <summary>
/// ��������� �������������� ����������� ���� ����� ��� ������ � ��.
/// ��� ��������� ����� ������������� ������� ����� �� �����.
/// ������������ ������������ ����, ���� ��� ����.
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
    /// ��������� ���� �����: ��� ����� ��������� ������� ������������� ������� �����.
    /// </summary>
    /// <param name="isPlayerTurn">true � ����� ������, false � ����� ��</param>
    public void ResolveAttackPhase(bool isPlayerTurn)
    {
        // �������� ������ ��������� � ������������
        var attackers = isPlayerTurn
            ? TurnManager.Instance.PlayerCards
            : TurnManager.Instance.EnemyCards;

        var defenders = isPlayerTurn
            ? TurnManager.Instance.EnemyCards
            : TurnManager.Instance.PlayerCards;

        if (attackers.Count == 0 || defenders.Count == 0)
            return;

        // ������� ������������ ���� � ������ ����� ����� � ������������ ������������
        var priorityTarget = defenders.FirstOrDefault(c =>
            c.cardData.ability != null &&
            c.cardData.ability.isPriorityTarget &&
            c.IsAlive);

        // ��� ������� ����������, � �������� loyalty > 0
        foreach (var attacker in attackers)
        {
            if (!attacker.CanAct || !attacker.IsAlive)
                continue;

            // �������� ����: ���� ������������, ���� ������ �����
            var target = (priorityTarget != null && priorityTarget.IsAlive)
                ? priorityTarget
                : defenders.FirstOrDefault(c => c.IsAlive);

            if (target == null)
                break; // ��� ���� ����������

            // ������� ����
            ResolveOneAttack(attacker, target);

            // ���� ������������ ���� ������� � ������� �
            if (priorityTarget != null && !priorityTarget.IsAlive)
            {
                priorityTarget = null;
            }
        }

        // ������� �������� ����� � ��������� UI
        CleanupDeadUnits(isPlayerTurn);
        UIManager.Instance.RefreshBattlefield();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// ������� ���� ����� ����� �������: ��������� vs ����.
    /// </summary>
    private void ResolveOneAttack(CardInstance attacker, CardInstance defender)
    {
        defender.TakeDamage(attacker.cardData.attack);
        // ��������� ����
        attacker.TakeDamage(defender.cardData.attack);
    }

    /// <summary>
    /// ������� �������� ����� �� ������� TurnManager.
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
