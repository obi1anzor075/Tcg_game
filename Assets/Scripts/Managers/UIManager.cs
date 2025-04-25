using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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
    [SerializeField] private Transform _playerHeroArea;
    [SerializeField] private Transform _playerMinionsArea;
    [SerializeField] private Transform _enemyHeroArea;
    [SerializeField] private Transform _enemyMinionsArea;

    [Header("Played Cards Area")]
    [SerializeField] private Transform _playedCardsArea;

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
    /// Обновляет UI поля битвы: герой по центру и верные впереди него.
    /// </summary>
    public void RefreshBattlefield()
    {
        // Игрок
        foreach (Transform t in _playerHeroArea) Destroy(t.gameObject);
        foreach (Transform t in _playerMinionsArea) Destroy(t.gameObject);

        // Герой игрока
        var playerHero = TurnManager.Instance.PlayerCards
            .FirstOrDefault(c => c.cardData.type == CardType.Hero);
        if (playerHero != null)
            CardFactory.Instance.CreateHeroOnField(playerHero.cardData, _playerHeroArea);

        // Верные игрока
        var playerMinions = TurnManager.Instance.PlayerCards
            .Where(c => c.cardData.type == CardType.Minion);
        foreach (var inst in playerMinions)
            CardFactory.Instance.CreateMinion(inst.cardData, _playerMinionsArea, false, true);

        // Противник
        foreach (Transform t in _enemyHeroArea) Destroy(t.gameObject);
        foreach (Transform t in _enemyMinionsArea) Destroy(t.gameObject);

        // Герой противника
        var enemyHero = TurnManager.Instance.EnemyCards
            .FirstOrDefault(c => c.cardData.type == CardType.Hero);
        if (enemyHero != null)
            CardFactory.Instance.CreateHeroOnField(enemyHero.cardData, _enemyHeroArea);

        // Верные противника
        var enemyMinions = TurnManager.Instance.EnemyCards
            .Where(c => c.cardData.type == CardType.Minion);
        foreach (var inst in enemyMinions)
            CardFactory.Instance.CreateMinion(inst.cardData, _enemyMinionsArea, false, false);
    }

    /// <summary>Показать карту, сыгранную из руки, в зоне PlayedCardsPanel.</summary>
    public void PlacePlayedCard(CardInstance instance, bool isPlayer)
    {
        if (instance.cardData.type == CardType.Minion || instance.cardData.type == CardType.Spell)
            CardFactory.Instance.CreateMinion(instance.cardData, _playedCardsArea, false, isPlayer);
        else
            CardFactory.Instance.CreateSpell(instance.cardData, _playedCardsArea, false, isPlayer);
    }

    /// <summary>
    /// Очищает панель сыгранных карт.
    /// </summary>
    public void ClearPlayedArea()
    {
        foreach (Transform t in _playedCardsArea)
            Destroy(t.gameObject);
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
