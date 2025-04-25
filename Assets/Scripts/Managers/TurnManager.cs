using System.Collections.Generic;
using UnityEngine;

#region Manager: TurnManager

/// <summary>
/// �������� �� ������ "����������", �������� ���� � �������� ���� �� ����.
/// </summary>
public class TurnManager : MonoBehaviour
{
    #region Singleton

    public static TurnManager Instance { get; private set; }

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

    #region Fields

    [Header("����� �� ����")]
    [SerializeField] private List<CardInstance> _playerCards = new();
    [SerializeField] private List<CardInstance> _enemyCards = new();

    private int _totalPlayerLoyalty;

    #endregion

    #region Properties

    /// <summary>������ ���� ������ �� ����.</summary>
    public IReadOnlyList<CardInstance> PlayerCards => _playerCards;

    /// <summary>������ ���� �� �� ����.</summary>
    public IReadOnlyList<CardInstance> EnemyCards => _enemyCards;

    /// <summary>������� ������ ���������� ������.</summary>
    public int TotalPlayerLoyalty => _totalPlayerLoyalty;

    #endregion

    #region Public Methods

    /// <summary>
    /// ������ ����: ���������� � ��������� ���������� � ���� ���� ������.
    /// </summary>
    public void StartTurn()
    {
        _totalPlayerLoyalty = 0;

        foreach (var card in _playerCards)
        {
            // ����� � ��������� ���������� �� ������ �����
            card.ResetLoyalty();
            if (card.currentLoyalty > 0)
                _totalPlayerLoyalty += card.currentLoyalty;
        }

        Debug.Log($"[TurnManager] New turn. Total Loyalty = {_totalPlayerLoyalty}");
    }


    /// <summary>
    /// �������� ��������� �����. ���� ��� ������ � ��������� �� ���� � ���������� ���������; 
    /// ���� ����������� � ��������� �. � ����� ������� ��������� ������ ����������.
    /// </summary>
    public CardInstance TryPlayCard(CardData cardData)
    {
        // ��������� �����:
        int cost = cardData.type == CardType.Minion
            ? cardData.baseLoyalty
            : cardData.loyaltyCost;

        if (cost > _totalPlayerLoyalty)
            return null; // ������������ �����������

        // ��������� ������
        _totalPlayerLoyalty -= cost;

        if (cardData.type == CardType.Minion)
        {
            var inst = new CardInstance(cardData);
            _playerCards.Add(inst);
            return inst;
        }
        else
        {
            // �����������
            ApplyCardEffect(cardData);
            return null;
        }
    }

    /// <summary>
    /// ��������� ����� �� ����
    /// </summary>
    public void AddPlayerCard(CardInstance instance)
    {
        _playerCards.Add(instance);
    }

    /// <summary>
    /// ������� ������ ����� � ���� (������ ��� ��).
    /// </summary>
    /// <returns>true ���� ����� ���� ������� � �������.</returns>
    public void RemoveDeadCards()
    {
        _playerCards.RemoveAll(c => !c.IsAlive);
        _enemyCards.RemoveAll(c => !c.IsAlive);
    }

    /// <summary>��������� ��������� ����������� (��� �������� CardInstance).</summary>
    public bool TrySpendLoyalty(int amount)
    {
        if (amount > _totalPlayerLoyalty) return false;
        _totalPlayerLoyalty -= amount;
        return true;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// ��������� ������� �����, ������� � ��������� ���������� ����� �����.
    /// </summary>
    private void ApplyCardEffect(CardData cardData)
    {
        // TODO: �������� ���������; ���� ������ �����������
        foreach (var eff in cardData.effects)
        {
            Debug.Log($"[TurnManager] Applying effect {eff.abilityName} (damage {eff.damage}, heal {eff.heal}, loyalty {eff.loyaltyDelta})");
            // ������: target.ModifyLoyalty(eff.loyaltyDelta);
        }
    }

    #endregion
}

#endregion
