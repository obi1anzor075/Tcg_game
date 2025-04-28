using System.Linq;
using UnityEngine;

#region Manager: AttackManager

/// <summary>
/// �������� �� �������������� ���� �����:
/// ��� ��������� ����� � loyalty > 0 ������������� ������� ���� �� �����.
/// ������ ��������� ����������� ������������� ����� � �������� OnAttack/OnDamaged.
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
    /// ��������� ���� �����: ��� ���� ����� � loyalty > 0 ������������� ���� �� �����.
    /// </summary>
    /// <param name="isPlayerTurn">true � ��� ������, false � ��� ��</param>
    public void ResolveAttackPhase(bool isPlayerTurn)
    {
        var attackers = isPlayerTurn
            ? TurnManager.Instance.PlayerCards
            : TurnManager.Instance.EnemyCards;

        var defenders = isPlayerTurn
            ? TurnManager.Instance.EnemyCards
            : TurnManager.Instance.PlayerCards;

        // ��� ��������� ��� ��� ����� � �������
        if (attackers.Count == 0 || defenders.Count == 0)
            return;

        // ���� ������������ ���� ����� �����, � ������� ���� ��������� AbilityData.isPriorityTarget
        CardInstance priority = defenders
            .FirstOrDefault(d =>
                d.IsAlive
                && d.cardData.abilities != null
                && d.cardData.abilities.Any(ab => ab.isPriorityTarget)
            );

        // ������ ��������� � CanAct
        foreach (var atk in attackers)
        {
            if (!atk.IsAlive || !atk.CanAct)
                continue;

            // ��������� OnAttack
            atk.OnAttack();

            // �������� ����: ������� ���������, ����� ����� ����� ������, ����� �����
            CardInstance target = null;

            if (priority != null && priority.IsAlive)
            {
                target = priority;
            }
            else
            {
                // ����� �������
                target = defenders.FirstOrDefault(d => d.IsAlive && d.cardData.type == CardType.Minion)
                      ?? defenders.FirstOrDefault(d => d.IsAlive && d.cardData.type == CardType.Hero);
            }

            if (target == null)
                break; // ��� ���� ������

            // ������� ����: ������� ������, ����� HP
            target.TakeDamage(atk.cardData.attack);
            // ��������� OnDamaged � ����
            target.OnDamaged();

            // ��������, ���������� ���� ���� ���������� ����� TakeDamage � target,
            // �� ���� �����, ����� ��������:
            atk.TakeDamage(target.cardData.attack);
            atk.OnDamaged();

            // ���� priority ����� � ����������
            if (priority != null && !priority.IsAlive)
                priority = null;
        }

        // ������� �������� �� �������
        TurnManager.Instance.RemoveDeadCards();

        // ��������� UI
        UIManager.Instance.RefreshBattlefield();
    }

    #endregion
}

#endregion
