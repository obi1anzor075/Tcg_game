using System;
using UnityEngine;

/// <summary>
/// Represents an instance of a card in the game with its own state.
/// </summary>
public class CardInstance
{
    public readonly CardData cardData;

    // Current stats
    public int currentHealth;
    public int currentAttack;
    public int currentLoyalty;
    public int currentDefense;

    // Properties
    public bool IsAlive => currentHealth > 0;

    /// <summary>
    /// Current defense value, used by UI and game logic.
    /// </summary>
    public int CurrentDefense => currentDefense;

    /// <summary>
    /// Current HP value, used by UI and game logic.
    /// </summary>
    public int currentHP => currentHealth;

    /// <summary>
    /// Creates a new instance of a card from its data.
    /// </summary>
    public CardInstance(CardData data)
    {
        cardData = data;
        currentHealth = data.baseHealth;
        currentAttack = data.baseAttack;
        currentLoyalty = data.baseLoyalty;
        currentDefense = data.baseDefense;
    }

    /// <summary>
    /// Apply damage to this card, reducing its health.
    /// First reduces defense if available, then applies remaining damage to health.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        // First, damage is applied to defense
        if (currentDefense > 0)
        {
            int absorbedByDefense = Mathf.Min(currentDefense, amount);
            currentDefense -= absorbedByDefense;
            amount -= absorbedByDefense;

            Debug.Log($"[CardInstance] {cardData.cardName} absorbed {absorbedByDefense} damage with defense. Defense now: {currentDefense}");

            // If defense absorbed all damage, we're done
            if (amount <= 0) return;
        }

        // Apply remaining damage to health
        currentHealth -= amount;

        // Ensure health doesn't go below 0
        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log($"[CardInstance] {cardData.cardName} took {amount} damage to health. Health now: {currentHealth}");
    }

    /// <summary>
    /// Modifies the card's loyalty value.
    /// </summary>
    public void ModifyLoyalty(int amount)
    {
        currentLoyalty = Mathf.Max(0, currentLoyalty + amount);
        Debug.Log($"[CardInstance] {cardData.cardName} loyalty changed by {amount}. New loyalty: {currentLoyalty}");
    }

    /// <summary>
    /// Modifies the card's defense value.
    /// </summary>
    public void ModifyDefense(int amount)
    {
        currentDefense = Mathf.Max(0, currentDefense + amount);
        Debug.Log($"[CardInstance] {cardData.cardName} defense changed by {amount}. New defense: {currentDefense}");
    }

    /// <summary>
    /// Resets loyalty to its base value at the start of a turn.
    /// </summary>
    public int ResetLoyalty()
    {
        if (cardData.type == CardType.Minion || cardData.type == CardType.Hero)
        {
            currentLoyalty = cardData.baseLoyalty;
        }
        return currentLoyalty;
    }
}