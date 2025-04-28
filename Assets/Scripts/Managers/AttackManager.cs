using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the attack phase of each turn, handling how cards attack each other.
/// </summary>
public class AttackManager : MonoBehaviour
{
    #region Singleton

    public static AttackManager Instance { get; private set; }

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

    [Header("Attack Settings")]
    [SerializeField] private float _attackAnimationDelay = 0.3f;
    [SerializeField] private bool _showDebugLogs = true;

    /// <summary>
    /// Resolves the attack phase for the current player's turn.
    /// </summary>
    /// <param name="isPlayer">True if player's turn, false if AI's turn</param>
    public void ResolveAttackPhase(bool isPlayer)
    {
        LogMessage("Starting attack phase");

        // Get attacking and defending card collections
        List<CardInstance> attackers = isPlayer
            ? TurnManager.Instance.PlayerCards.Where(c => c.IsAlive && c.currentAttack > 0).ToList()
            : TurnManager.Instance.EnemyCards.Where(c => c.IsAlive && c.currentAttack > 0).ToList();

        List<CardInstance> defenders = isPlayer
            ? TurnManager.Instance.EnemyCards.Where(c => c.IsAlive).ToList()
            : TurnManager.Instance.PlayerCards.Where(c => c.IsAlive).ToList();

        if (attackers.Count == 0 || defenders.Count == 0)
        {
            LogMessage("No attackers or defenders available. Skipping attack phase.");
            return;
        }

        LogMessage($"Found {attackers.Count} attackers and {defenders.Count} defenders");

        // Process each attacker individually
        foreach (var attacker in attackers)
        {
            // Skip cards that can't attack (0 attack power)
            if (attacker.currentAttack <= 0)
            {
                LogMessage($"{attacker.cardData.cardName} can't attack (0 attack power)");
                continue;
            }

            // Find an appropriate target for this attacker
            CardInstance target = SelectTargetFor(attacker, defenders);

            if (target != null)
            {
                // Execute the attack and apply damage
                ExecuteAttack(attacker, target);

                // Check if target was defeated and update defenders list if needed
                if (!target.IsAlive)
                {
                    LogMessage($"{target.cardData.cardName} was defeated and will be removed from defenders");
                    defenders.Remove(target);
                    if (defenders.Count == 0)
                    {
                        LogMessage("No more defenders left. Ending attack phase.");
                        break;
                    }
                }
            }
        }

        // Remove any cards that were killed during the attack phase
        TurnManager.Instance.RemoveDeadCards();

        LogMessage("Attack phase completed");
    }

    /// <summary>
    /// Selects a target for the attacker following targeting rules.
    /// </summary>
    private CardInstance SelectTargetFor(CardInstance attacker, List<CardInstance> possibleTargets)
    {
        if (possibleTargets.Count == 0)
        {
            LogMessage($"No targets available for {attacker.cardData.cardName}");
            return null;
        }

        // Priority 1: For heroes, prioritize attacking enemy hero if possible
        if (attacker.cardData.type == CardType.Hero)
        {
            var enemyHero = possibleTargets.FirstOrDefault(c => c.cardData.type == CardType.Hero);
            if (enemyHero != null)
            {
                LogMessage($"{attacker.cardData.cardName} targets enemy hero {enemyHero.cardData.cardName}");
                return enemyHero;
            }
        }

        // Priority 2: For minions, prioritize attacking minions with lowest defense first
        var minions = possibleTargets.Where(c => c.cardData.type == CardType.Minion).ToList();
        if (minions.Count > 0)
        {
            // Target minion with lowest defense first
            var target = minions.OrderBy(m => m.CurrentDefense).ThenBy(m => m.currentHealth).FirstOrDefault();
            LogMessage($"{attacker.cardData.cardName} targets minion {target.cardData.cardName}");
            return target;
        }

        // Priority 3: If no minions, attack the hero
        var hero = possibleTargets.FirstOrDefault(c => c.cardData.type == CardType.Hero);
        if (hero != null)
        {
            LogMessage($"{attacker.cardData.cardName} targets hero {hero.cardData.cardName} (no minions left)");
        }
        return hero;
    }

    /// <summary>
    /// Executes an attack from attacker to defender.
    /// </summary>
    private void ExecuteAttack(CardInstance attacker, CardInstance defender)
    {
        int damage = attacker.currentAttack;

        LogMessage($"{attacker.cardData.cardName} attacks {defender.cardData.cardName} for {damage} damage!");

        // Apply damage to defender - our updated TakeDamage method will handle defense properly
        defender.TakeDamage(damage);

        // Check if defender is still alive and can counter-attack
        // Heroes don't counter-attack, but minions do if they have attack value
        if (defender.IsAlive && defender.cardData.type == CardType.Minion && defender.currentAttack > 0)
        {
            int counterDamage = defender.currentAttack;
            LogMessage($"{defender.cardData.cardName} counter-attacks for {counterDamage} damage!");
            attacker.TakeDamage(counterDamage);
        }
    }

    /// <summary>
    /// Helper method for logging debug messages.
    /// </summary>
    private void LogMessage(string message)
    {
        if (_showDebugLogs)
        {
            Debug.Log($"[AttackManager] {message}");
        }
    }
}