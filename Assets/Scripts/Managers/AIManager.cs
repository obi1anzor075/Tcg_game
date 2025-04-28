using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Manager: AIManager

/// <summary>
/// Управляет логикой хода ИИ: сброс и расчёт преданности, 
/// последующий розыгрыш миньонов и способностей из руки.
/// </summary>
public class AIManager : MonoBehaviour
{
    #region Singleton

    public static AIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    #endregion

    #region Public API

    /// <summary>
    /// Выполняет ход ИИ: обновляет ресурс, пробует сыграть миньоны, затем спеллы.
    /// </summary>
    public void PlayTurn()
    {
        // 1. Начало хода ИИ: сброс и восстановление лояльности
        TurnManager.Instance.StartEnemyTurn();
        int totalLoyalty = TurnManager.Instance.TotalEnemyLoyalty;
        Debug.Log($"[AIManager] AI start turn. Total Loyalty = {totalLoyalty}");

        // 2. Копируем руку, чтобы безопасно итерироваться и модифицировать её
        var handCopy = HandManager.Instance.enemyHand.ToList();

        // 3. Разыгрываем миньонов
        foreach (var cardData in handCopy.Where(c => c.type == CardType.Minion).ToList())
        {
            if (cardData.baseLoyalty <= totalLoyalty)
            {
                var inst = TurnManager.Instance.TryPlayEnemyCard(cardData);
                if (inst != null)
                {
                    totalLoyalty -= cardData.baseLoyalty;
                    HandManager.Instance.enemyHand.Remove(cardData);
                    UIManager.Instance.PlacePlayedCard(inst, false);
                    GameManager.Instance.RecordPlayed(inst);
                    Debug.Log($"[AIManager] AI played minion {cardData.cardName}");
                }
            }
        }

        // 4. Разыгрываем спеллы и способности
        foreach (var cardData in handCopy.Where(c => c.type == CardType.Spell || c.type == CardType.HeroAbility).ToList())
        {
            if (cardData.loyaltyCost <= totalLoyalty)
            {
                // способности могут требовать таргетинга, но для простоты применяем без таргета
                var inst = TurnManager.Instance.TryPlayEnemyCard(cardData);
                if (inst != null)
                {
                    totalLoyalty -= cardData.loyaltyCost;
                    HandManager.Instance.enemyHand.Remove(cardData);
                    UIManager.Instance.PlacePlayedCard(inst, false);
                    GameManager.Instance.RecordPlayed(inst);
                    Debug.Log($"[AIManager] AI played spell/ability {cardData.cardName}");
                }
            }
        }

        Debug.Log("[AIManager] AI end turn.");
    }

    #endregion
}

#endregion
