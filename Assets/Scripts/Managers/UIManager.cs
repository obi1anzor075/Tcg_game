using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public event Action OnEndTurnClicked;

    [Header("End Turn Button")]
    [SerializeField] private Button _endTurnButton;

    [Header("Hand Areas")]
    [SerializeField] private Transform _playerHandArea;
    [SerializeField] private Transform _enemyHandArea;
    [SerializeField] private GameObject _cardBackPrefab;

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

    [Header("Loyalty Display")]
    [SerializeField] private TMP_Text _loyaltyDisplay;

    // Последняя UI-карта, которую мы выложили (для анимации спелла)
    private CardUI _lastPlayedUI;

<<<<<<< HEAD
    private bool _isTargeting = false;
    private AbilityData _currentEffectAbility = null;

    public bool IsTargeting => _isTargeting;
    public AbilityData CurrentEffectAbility => _currentEffectAbility;

    #endregion

    #region Unity Methods
=======
    private bool _isTargeting;
    private AbilityData _currentAbility;

    public bool IsTargeting => _isTargeting;
    public AbilityData CurrentAbility => _currentAbility;
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _endTurnButton.onClick.AddListener(() => OnEndTurnClicked?.Invoke());
    }

<<<<<<< HEAD
    #endregion

    #region Public API

=======
    /// <summary>Рисует руки: игрока лицом, ИИ — рубашками.</summary>
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public void RefreshHands()
    {
        // Игрок
        foreach (Transform t in _playerHandArea) Destroy(t.gameObject);
        foreach (var data in HandManager.Instance.playerHand)
<<<<<<< HEAD
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
=======
            CardFactory.Instance.CreateMinion(data, _playerHandArea, inHand: true, isPlayerSide: true);

        // ИИ
        foreach (Transform t in _enemyHandArea) Destroy(t.gameObject);
        int count = HandManager.Instance.enemyHand.Count;
        for (int i = 0; i < count; i++)
            Instantiate(_cardBackPrefab, _enemyHandArea);
    }

    /// <summary>Рисует поле, используя уже существующие CardInstance.</summary>
    public void RefreshBattlefield()
    {
        // Игрок
        ClearArea(_playerHeroArea);
        ClearArea(_playerMinionsArea);

        foreach (var inst in TurnManager.Instance.PlayerCards)
        {
            if (inst.cardData.type == CardType.Hero)
                CardFactory.Instance.CreateHeroOnFieldInstance(inst, _playerHeroArea, isPlayer: true);
            else
                CardFactory.Instance.CreateMinionInstance(inst, _playerMinionsArea, isPlayer: true);
        }

        // Противник
        ClearArea(_enemyHeroArea);
        ClearArea(_enemyMinionsArea);

        foreach (var inst in TurnManager.Instance.EnemyCards)
        {
            if (inst.cardData.type == CardType.Hero)
                CardFactory.Instance.CreateHeroOnFieldInstance(inst, _enemyHeroArea, isPlayer: false);
            else
                CardFactory.Instance.CreateMinionInstance(inst, _enemyMinionsArea, isPlayer: false);
        }
    }

    private void ClearArea(Transform parent)
    {
        foreach (Transform t in parent) Destroy(t.gameObject);
    }

    /// <summary>Показывает только что сыгранную карту в PlayedArea и запоминает её UI.</summary>
    public void PlacePlayedCard(CardInstance inst, bool isPlayer)
    {
        Transform area = isPlayer ? _playerPlayedArea : _enemyPlayedArea;
        CardUI ui = CardFactory.Instance.CreateMinionInstance(inst, area, isPlayer);
        _lastPlayedUI = ui;
    }

    /// <summary>Очищает зоны сыгранных карт.</summary>
    public void ClearPlayedAreas()
    {
        ClearArea(_playerPlayedArea);
        ClearArea(_enemyPlayedArea);
        _lastPlayedUI = null;
    }

    /// <summary>Начинает режим таргетинга: выдвигает спелл и трясёт валидные цели.</summary>
    public void BeginTargetMode(AbilityData ability)
    {
        _isTargeting = true;
        _currentAbility = ability;

        // анимируем выдвижение спелла вверх
        if (_lastPlayedUI != null)
        {
            var rt = _lastPlayedUI.GetComponent<RectTransform>();
            rt.DOKill();
            rt.DOAnchorPosY(rt.anchoredPosition.y + 30f, 0.2f).SetEase(Ease.OutQuad);
        }

        HighlightValidTargets(ability);
    }

    /// <summary>Выход из таргетинга — сброс подсветки.</summary>
    public void EndTargetMode()
    {
        _isTargeting = false;
        _currentAbility = null;
        foreach (var ui in FindObjectsOfType<CardUI>())
            ui.SetHighlight(false);
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    }

    private void HighlightValidTargets(AbilityData ability)
    {
        foreach (var ui in FindObjectsOfType<CardUI>())
        {
            bool valid = ability.targetType switch
            {
                TargetType.Enemy => ui.IsEnemy,
                TargetType.Ally => !ui.IsEnemy,
                TargetType.Any => true,
                _ => false
            };

            ui.SetHighlight(valid);
            if (valid)
            {
                var rt = ui.GetComponent<RectTransform>();
                rt.DOKill();
                rt.DOShakeAnchorPos(duration: 0.5f, strength: new Vector2(8, 8), vibrato: 10, randomness: 90)
                  .SetLoops(1, LoopType.Yoyo);
            }
        }
    }

    /// <summary>Обновляет текстовое поле лояльности игрока.</summary>
    public void UpdateLoyaltyDisplay()
    {
        _loyaltyDisplay.text = $"Loyalty: {TurnManager.Instance.PlayerLoyalty}";
    }
}
