using UnityEngine;
using UnityEngine.UI;
using System;

#region UI: UIManager

/// <summary>
/// Управляет отрисовкой руки, поля и режимом таргетинга способностей.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton & Events

    public static UIManager Instance { get; private set; }
    public event Action OnEndTurnClicked;

    #endregion

    #region Editor Fields

    [Header("End Turn Button")]
    [SerializeField] private Button _endTurnButton;

    [Header("Hand Areas")]
    [SerializeField] private Transform _playerHandArea;
    [SerializeField] private Transform _enemyHandArea;

    [Header("Battlefield Areas")]
    [SerializeField] private Transform _playerBattleArea;
    [SerializeField] private Transform _enemyBattleArea;

    #endregion

    #region State

    private bool _isTargeting = false;
    private CardData _currentEffectCard = null;

    public bool IsTargeting => _isTargeting;
    public CardData CurrentEffectCard => _currentEffectCard;

    #endregion

    #region Unity Methods

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

    private void Start()
    {
        _endTurnButton.onClick.AddListener(() => OnEndTurnClicked?.Invoke());
    }

    #endregion

    #region Public API

    /// <summary>
    /// Обновляет UI рук игрока и ИИ, создавая карты через CardFactory.
    /// </summary>
    public void RefreshHands()
    {
        foreach (Transform t in _playerHandArea) Destroy(t.gameObject);
        foreach (var data in HandManager.Instance.playerHand)
        {
            if (data.type == CardType.Minion)
                CardFactory.Instance.CreateMinion(data, _playerHandArea, true, true);
            else
                CardFactory.Instance.CreateSpell(data, _playerHandArea, true, true);
        }

        foreach (Transform t in _enemyHandArea) Destroy(t.gameObject);
        foreach (var data in HandManager.Instance.enemyHand)
        {
            var ui = (data.type == CardType.Minion)
                ? CardFactory.Instance.CreateMinion(data, _enemyHandArea, true, false)
                : CardFactory.Instance.CreateSpell(data, _enemyHandArea, true, false);
            ui.SetupBack();
        }
    }

    /// <summary>
    /// Обновляет UI поля битвы, создавая карты через CardFactory.
    /// </summary>
    public void RefreshBattlefield()
    {
        foreach (Transform t in _playerBattleArea) Destroy(t.gameObject);
        foreach (var inst in TurnManager.Instance.PlayerCards)
        {
            if (inst.cardData.type == CardType.Hero)
                CardFactory.Instance.CreateHeroOnField(inst.cardData, _playerBattleArea);
            else if (inst.cardData.type == CardType.Minion)
                CardFactory.Instance.CreateMinion(inst.cardData, _playerBattleArea, false, true);
            else
                CardFactory.Instance.CreateSpell(inst.cardData, _playerBattleArea, false, true);
        }

        foreach (Transform t in _enemyBattleArea) Destroy(t.gameObject);
        foreach (var inst in TurnManager.Instance.EnemyCards)
        {
            if (inst.cardData.type == CardType.Hero)
                CardFactory.Instance.CreateHeroOnField(inst.cardData, _enemyBattleArea);
            else if (inst.cardData.type == CardType.Minion)
                CardFactory.Instance.CreateMinion(inst.cardData, _enemyBattleArea, false, false);
            else
                CardFactory.Instance.CreateSpell(inst.cardData, _enemyBattleArea, false, false);
        }
    }

    /// <summary>
    /// Входит в режим таргетинга: все карты подсвечиваются.
    /// </summary>
    public void BeginTargetMode(CardData effectCard)
    {
        _isTargeting = true;
        _currentEffectCard = effectCard;
        HighlightAll(true);
    }

    /// <summary>
    /// Выходит из режима таргетинга.
    /// </summary>
    public void EndTargetMode()
    {
        _isTargeting = false;
        _currentEffectCard = null;
        HighlightAll(false);
    }

    #endregion

    #region Helpers

    private void HighlightAll(bool enable)
    {
        foreach (var ui in FindObjectsOfType<CardUI>())
            ui.SetHighlight(enable);
    }

    #endregion
}

#endregion
