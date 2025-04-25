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
            card.ResetLoyalty();
            if (card.currentLoyalty > 0)
                _totalPlayerLoyalty += card.currentLoyalty;
        }

        Debug.Log($"[TurnManager] StartTurn � Total Loyalty: {_totalPlayerLoyalty}");
    }

    /// <summary>
    /// ����������� �����: ������ ������� �� ����, ����������� �����������.
    /// </summary>
    /// <returns>true ���� ����� �������, ����� false.</returns>
    public bool PlayCard(CardData cardData)
    {
        if (cardData.type == CardType.Minion &&
            cardData.baseLoyalty <= _totalPlayerLoyalty)
        {
            var newCard = new CardInstance(cardData);
            _playerCards.Add(newCard);
            _totalPlayerLoyalty -= cardData.baseLoyalty;
            return true;
        }
        else if ((cardData.type == CardType.Spell || cardData.type == CardType.HeroAbility) &&
                  cardData.loyaltyCost <= _totalPlayerLoyalty)
        {
            ApplyCardEffect(cardData);
            _totalPlayerLoyalty -= cardData.loyaltyCost;
            return true;
        }

        return false;
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
