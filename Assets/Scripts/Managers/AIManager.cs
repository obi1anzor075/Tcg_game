using System;
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
        TurnManager.Instance.StartTurn(false);
        int loyalty = TurnManager.Instance.EnemyLoyalty;
        var hand = HandManager.Instance.enemyHand.ToList();

        // обобщённый метод
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


    #endregion
}

#endregion
