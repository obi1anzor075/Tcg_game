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

    [Header("Enemy Hand Back")]
    [SerializeField] private GameObject _cardBackPrefab;

    [Header("Played Cards Area")]
    [SerializeField] private Transform _playerPlayedArea;
    [SerializeField] private Transform _enemyPlayedArea;

    #endregion

    #region State

    private bool _isTargeting = false;
    private AbilityData _currentEffectAbility = null;

    public bool IsTargeting => _isTargeting;
    public AbilityData CurrentEffectAbility => _currentEffectAbility;

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

    public void RefreshHands()
    {
        // Игрок
        foreach (Transform t in _playerHandArea) Destroy(t.gameObject);
        foreach (var data in HandManager.Instance.playerHand)
            CardFactory.Instance.CreateMinion(data, _playerHandArea, inHand: true, isPlayer: true);

        // ИИ
        foreach (Transform t in _enemyHandArea) Destroy(t.gameObject);
        int cnt = HandManager.Instance.enemyHand.Count;
        for (int i = 0; i < cnt; i++)
            Instantiate(_cardBackPrefab, _enemyHandArea);
    }

    public void RefreshBattlefield()
    {
        // Очистка
        foreach (Transform t in _playerHeroArea) Destroy(t.gameObject);
        foreach (Transform t in _playerMinionsArea) Destroy(t.gameObject);
        foreach (Transform t in _enemyHeroArea) Destroy(t.gameObject);
        foreach (Transform t in _enemyMinionsArea) Destroy(t.gameObject);

        // Герой игрока
        var playerHero = TurnManager.Instance.PlayerCards
            .FirstOrDefault(c => c.cardData.type == CardType.Hero);
        if (playerHero != null)
            CardFactory.Instance.CreateHeroOnField(playerHero.cardData, _playerHeroArea, isPlayer: true);

        // Миньоны игрока
        var playerMinions = TurnManager.Instance.PlayerCards
            .Where(c => c.cardData.type == CardType.Minion);
        foreach (var inst in playerMinions)
            CardFactory.Instance.CreateMinionInstance(inst, _playerMinionsArea, isPlayer: true);

        // Герой противника
        var enemyHero = TurnManager.Instance.EnemyCards
            .FirstOrDefault(c => c.cardData.type == CardType.Hero);
        if (enemyHero != null)
            CardFactory.Instance.CreateHeroOnField(enemyHero.cardData, _enemyHeroArea, isPlayer: false);

        // Миньоны противника
        var enemyMinions = TurnManager.Instance.EnemyCards
            .Where(c => c.cardData.type == CardType.Minion);
        foreach (var inst in enemyMinions)
            CardFactory.Instance.CreateMinionInstance(inst, _enemyMinionsArea, isPlayer: false);
    }

    public void PlacePlayedCard(CardInstance instance, bool isPlayer)
    {
        var targetArea = isPlayer ? _playerPlayedArea : _enemyPlayedArea;
        if (instance.cardData.type == CardType.Minion)
            CardFactory.Instance.CreateMinionInstance(instance, targetArea, isPlayer);
        else
            CardFactory.Instance.CreateSpellInstance(instance, targetArea, isPlayer);
    }

    public void ClearPlayedAreas()
    {
        foreach (Transform t in _playerPlayedArea) Destroy(t.gameObject);
        foreach (Transform t in _enemyPlayedArea) Destroy(t.gameObject);
    }

    public void BeginTargetMode(AbilityData effectAbility)
    {
        _isTargeting = true;
        _currentEffectAbility = effectAbility;
        EffectProcessor.Instance.BeginTargeting(effectAbility);
        HighlightAll(true);
    }

    public void EndTargetMode()
    {
        _isTargeting = false;
        _currentEffectAbility = null;
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
