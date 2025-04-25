using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Логика хода ИИ: набираем лояльность, затем раскидываем доступные карты/способности.
    /// </summary>
    public void PlayTurn()
    {
        // 1. Рассчитываем ресурс лояльности ИИ
        int totalLoyalty = 0;
        foreach (var card in TurnManager.Instance.EnemyCards)
        {
            // Сбрасываем loyalty к базовому
            card.ResetLoyalty();
            if (card.currentLoyalty > 0)
                totalLoyalty += card.currentLoyalty;
        }
        Debug.Log($"AI start turn. Total Loyalty = {totalLoyalty}");

        // 2. Пробуем разыграть миньонов из руки (или способности)
        //    Здесь предполагаем, что у ИИ есть список CardData в руке — добавьте свой менеджер руки, если нужно.
        List<CardData> hand = HandManager.Instance.enemyHand;

        // Сначала раскидываем всех миньонов, которых можем
        foreach (var cardData in new List<CardData>(hand))
        {
            if (cardData.type == CardType.Minion &&
                cardData.baseLoyalty <= totalLoyalty)
            {
                TurnManager.Instance.PlayCard(cardData);
                totalLoyalty -= cardData.baseLoyalty;
                hand.Remove(cardData);
                Debug.Log($"AI played minion {cardData.cardName}");
            }
        }

        // Потом способности/спеллы
        foreach (var cardData in new List<CardData>(hand))
        {
            if ((cardData.type == CardType.Spell || cardData.type == CardType.HeroAbility) &&
                cardData.loyaltyCost <= totalLoyalty)
            {
                TurnManager.Instance.PlayCard(cardData);
                totalLoyalty -= cardData.loyaltyCost;
                hand.Remove(cardData);
                Debug.Log($"AI used ability {cardData.cardName}");
            }
        }

        Debug.Log("AI end turn.");
    }
}
