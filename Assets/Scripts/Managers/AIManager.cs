using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Manager: AIManager

/// <summary>
/// ��������� ������� ���� ��: ����� � ������ �����������, 
/// ����������� �������� �������� � ������������ �� ����.
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
    /// ��������� ��� ��: ��������� ������, ������� ������� �������, ����� ������.
    /// </summary>
    public void PlayTurn()
    {
<<<<<<< HEAD
        // 1. ������ ���� ��: ����� � �������������� ����������
        TurnManager.Instance.StartTurn(isPlayer: false);
        int totalLoyalty = TurnManager.Instance.EnemyLoyalty;
        Debug.Log($"[AIManager] AI start turn. Total Loyalty = {totalLoyalty}");

        // 2. �������� ����, ����� ��������� ������������� � �������������� �
        var handCopy = HandManager.Instance.enemyHand.ToList();

        // 3. ����������� ��������
        foreach (var cardData in handCopy.Where(c => c.type == CardType.Minion).ToList())
        {
            if (cardData.baseLoyalty <= totalLoyalty)
            {
                var inst = TurnManager.Instance.TryPlayCard(cardData, isPlayer:false);
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

        // 4. ����������� ������ � �����������
        foreach (var cardData in handCopy.Where(c => c.type == CardType.Spell || c.type == CardType.HeroAbility).ToList())
        {
            if (cardData.loyaltyCost <= totalLoyalty)
            {
                // ����������� ����� ��������� ����������, �� ��� �������� ��������� ��� �������
                var inst = TurnManager.Instance.TryPlayCard(cardData, isPlayer:false);
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

=======
        TurnManager.Instance.StartTurn(false);
        int loyalty = TurnManager.Instance.EnemyLoyalty;
        var hand = HandManager.Instance.enemyHand.ToList();

        // ���������� �����
        TryPlayAllOfType(hand, CardType.Minion, loyalty, c => c.baseLoyalty);
        TryPlayAllOfType(hand, CardType.Spell, loyalty, c => c.loyaltyCost);
        TryPlayAllOfType(hand, CardType.HeroAbility, loyalty, c => c.loyaltyCost);
    }

    private void TryPlayAllOfType(
          List<CardData> handCopy,
          CardType type,
          int currentLoyalty,
          Func<CardData, int> costSelector)
    {
        foreach (var card in handCopy.Where(c => c.type == type).ToList())
        {
            int cost = costSelector(card);
            if (cost <= currentLoyalty)
            {
                var inst = TurnManager.Instance.TryPlayCard(card, isPlayer:false);
                if (inst != null)
                {
                    currentLoyalty -= cost;
                    HandManager.Instance.enemyHand.Remove(card);
                    UIManager.Instance.PlacePlayedCard(inst, false);
                    GameManager.Instance.RecordPlayed(inst);
                }
            }
        }
    }


>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    #endregion
}

#endregion
