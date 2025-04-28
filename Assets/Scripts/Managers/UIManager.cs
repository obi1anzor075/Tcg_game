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

    [Header("Played Cards Area")]
    [SerializeField] private Transform _playerPlayedArea;
    [SerializeField] private Transform _enemyPlayedArea;

    [Header("Loyalty Display")]
    [SerializeField] private TMP_Text _loyaltyDisplay;

    // ��������� UI-�����, ������� �� �������� (��� �������� ������)
    private CardUI _lastPlayedUI;

    private bool _isTargeting;
    private AbilityData _currentAbility;

    public bool IsTargeting => _isTargeting;
    public AbilityData CurrentAbility => _currentAbility;

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

    /// <summary>������ ����: ������ �����, �� � ���������.</summary>
    public void RefreshHands()
    {
        // �����
        foreach (Transform t in _playerHandArea) Destroy(t.gameObject);
        foreach (var data in HandManager.Instance.playerHand)
            CardFactory.Instance.CreateMinion(data, _playerHandArea, inHand: true, isPlayerSide: true);

        // ��
        foreach (Transform t in _enemyHandArea) Destroy(t.gameObject);
        int count = HandManager.Instance.enemyHand.Count;
        for (int i = 0; i < count; i++)
            Instantiate(_cardBackPrefab, _enemyHandArea);
    }

    /// <summary>������ ����, ��������� ��� ������������ CardInstance.</summary>
    public void RefreshBattlefield()
    {
        // �����
        ClearArea(_playerHeroArea);
        ClearArea(_playerMinionsArea);

        foreach (var inst in TurnManager.Instance.PlayerCards)
        {
            if (inst.cardData.type == CardType.Hero)
                CardFactory.Instance.CreateHeroOnFieldInstance(inst, _playerHeroArea, isPlayer: true);
            else
                CardFactory.Instance.CreateMinionInstance(inst, _playerMinionsArea, isPlayer: true);
        }

        // ���������
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

    /// <summary>���������� ������ ��� ��������� ����� � PlayedArea � ���������� � UI.</summary>
    public void PlacePlayedCard(CardInstance inst, bool isPlayer)
    {
        Transform area = isPlayer ? _playerPlayedArea : _enemyPlayedArea;
        CardUI ui = CardFactory.Instance.CreateMinionInstance(inst, area, isPlayer);
        _lastPlayedUI = ui;
    }

    /// <summary>������� ���� ��������� ����.</summary>
    public void ClearPlayedAreas()
    {
        ClearArea(_playerPlayedArea);
        ClearArea(_enemyPlayedArea);
        _lastPlayedUI = null;
    }

    /// <summary>�������� ����� ����������: ��������� ����� � ����� �������� ����.</summary>
    public void BeginTargetMode(AbilityData ability)
    {
        _isTargeting = true;
        _currentAbility = ability;

        // ��������� ���������� ������ �����
        if (_lastPlayedUI != null)
        {
            var rt = _lastPlayedUI.GetComponent<RectTransform>();
            rt.DOKill();
            rt.DOAnchorPosY(rt.anchoredPosition.y + 30f, 0.2f).SetEase(Ease.OutQuad);
        }

        HighlightValidTargets(ability);
    }

    /// <summary>����� �� ���������� � ����� ���������.</summary>
    public void EndTargetMode()
    {
        _isTargeting = false;
        _currentAbility = null;
        foreach (var ui in FindObjectsOfType<CardUI>())
            ui.SetHighlight(false);
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

    /// <summary>��������� ��������� ���� ���������� ������.</summary>
    public void UpdateLoyaltyDisplay()
    {
        _loyaltyDisplay.text = $"Loyalty: {TurnManager.Instance.PlayerLoyalty}";
    }
}
