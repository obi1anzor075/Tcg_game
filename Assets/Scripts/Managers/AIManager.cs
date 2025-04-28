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
        // 1. ������ ���� ��: ����� � �������������� ����������
        TurnManager.Instance.StartEnemyTurn();
        int totalLoyalty = TurnManager.Instance.TotalEnemyLoyalty;
        Debug.Log($"[AIManager] AI start turn. Total Loyalty = {totalLoyalty}");

        // 2. �������� ����, ����� ��������� ������������� � �������������� �
        var handCopy = HandManager.Instance.enemyHand.ToList();

        // 3. ����������� ��������
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

        // 4. ����������� ������ � �����������
        foreach (var cardData in handCopy.Where(c => c.type == CardType.Spell || c.type == CardType.HeroAbility).ToList())
        {
            if (cardData.loyaltyCost <= totalLoyalty)
            {
                // ����������� ����� ��������� ����������, �� ��� �������� ��������� ��� �������
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
