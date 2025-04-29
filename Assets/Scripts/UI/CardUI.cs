using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button), typeof(LayoutElement))]
public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Animation Settings

    [Header("Hover Animation")]
    [SerializeField] private float _hoverY = 10f;
    [SerializeField] private float _scale = 1.05f;
    [SerializeField] private float _dur = 0.15f;
    [SerializeField] private RectTransform _visual;

    private Vector3 _startPos, _startScale;

    #endregion

    #region Inspector Fields

    [SerializeField] private Image _artworkImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _loyaltyText;
    [SerializeField] private TMP_Text _defenseText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _abilityDescText;
    [SerializeField] private Image _highlightBorder;

    #endregion

    #region State

    private CardData _cardData;
    private CardInstance _cardInstance;
    private bool _isPlayerHand;
    public bool IsEnemy { get; private set; }

    #endregion

    #region Unity

    private void Awake()
    {
        if (_visual == null)
            _visual = GetComponent<RectTransform>();
        _startPos = _visual.localPosition;
        _startScale = _visual.localScale;
    }

    #endregion

    #region Hover Handlers

    private void OnDisable()
    {
        // �������������� ������� ����� �����, ����������� � visual ��� ����� �������
        if (_visual != null) _visual.DOKill();
        transform.DOKill();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!_isPlayerHand) return;

        _visual.DOKill();
        transform.DOKill();

        _visual
            .DOLocalMoveY(_startPos.y + _hoverY, _dur)
            .SetEase(Ease.OutQuad);
        transform
            .DOScale(_startScale * _scale, _dur)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!_isPlayerHand) return;

        _visual.DOKill();
        transform.DOKill();

        _visual
            .DOLocalMove(_startPos, _dur)
            .SetEase(Ease.OutQuad);
        transform
            .DOScale(_startScale, _dur)
            .SetEase(Ease.OutQuad);
    }

    private bool _isEnemySide;
    public bool IsEnemy => _isEnemySide;

    #endregion

    #region Unity

    private void Awake()
    {
        if (_visual == null)
            _visual = GetComponent<RectTransform>();
        _startPos = _visual.localPosition;
        _startScale = _visual.localScale;
    }

    #endregion

    #region Hover Handlers

    private void OnDisable()
    {
        // �������������� ������� ����� �����, ����������� � visual ��� ����� �������
        if (_visual != null) _visual.DOKill();
        transform.DOKill();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!_isPlayerHand) return;

        _visual.DOKill();
        transform.DOKill();

        _visual
            .DOLocalMoveY(_startPos.y + _hoverY, _dur)
            .SetEase(Ease.OutQuad);
        transform
            .DOScale(_startScale * _scale, _dur)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!_isPlayerHand) return;

        _visual.DOKill();
        transform.DOKill();

        _visual
            .DOLocalMove(_startPos, _dur)
            .SetEase(Ease.OutQuad);
        transform
            .DOScale(_startScale, _dur)
            .SetEase(Ease.OutQuad);
    }

    #endregion

    #region Public API

    /// <summary>��������� UI ��� ����� � ����.</summary>
    public void Setup(CardData data, bool isPlayer)
    {
        _cardData = data;
        _cardInstance = null;
        _isPlayerHand = isPlayer;
        IsEnemy = !_isPlayerHand; 

        // ��������� LayoutElement � �������� ��������� layout, ����� �������� �� ���������
        var layoutElem = GetComponent<LayoutElement>();
        if (layoutElem != null) layoutElem.ignoreLayout = false;

        // ��������� LayoutElement � �������� ��������� layout, ����� �������� �� ���������
        var layoutElem = GetComponent<LayoutElement>();
        if (layoutElem != null) layoutElem.ignoreLayout = false;

        // ��� � ���
        _artworkImage?.gameObject.SetActive(true);
        _artworkImage.sprite = data.artwork;
        _nameText.text = data.cardName;

        // �����������/���������
        if (_loyaltyText != null)
        {
            _loyaltyText.text = (data.type == CardType.Minion)
                ? data.baseLoyalty.ToString()
                : data.loyaltyCost.ToString();
        }

        // ������ � HP (��� ����, ����� ������)
        _defenseText.text = data.baseDefense.ToString();
<<<<<<< HEAD
        _hpText.text = data.maxHP.ToString();
=======
        _hpText.text = data.baseHealth.ToString();
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

        // �������� ������������
        if (_abilityDescText != null)
        {
            if (data.abilities != null && data.abilities.Length > 0)
            {
                // ������ ����� �������� ������ �����������
                _abilityDescText.text = data.abilities[0].description;
                _abilityDescText.gameObject.SetActive(true);
            }
            else
            {
                _abilityDescText.gameObject.SetActive(false);
            }
        }

        // ������ ���������
        if (_highlightBorder != null) _highlightBorder.enabled = false;

        // ����������� ����
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);
    }

    /// <summary>�������� ������� ����� (��� ��).</summary>
    public void SetupBack()
    {
        _artworkImage?.gameObject.SetActive(false);
        _nameText.text = "";
        _loyaltyText.text = "";
        _defenseText.text = "";
        _hpText.text = "";
        _abilityDescText.gameObject.SetActive(false);
        _highlightBorder.enabled = false;
<<<<<<< HEAD
    }

    /// <summary>��������� UI ��� ����� �� ���� (�����).</summary>
    /// <summary>
    /// ��������� UI ��� ����� �� ���� (�����).
    /// </summary>
=======
        IsEnemy = true;             
    }

    /// <summary>��������� UI ��� ����� �� ���� (�����).</summary>
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public void SetupBattle(CardInstance instance, bool isPlayerSide)
    {
        _cardInstance = instance;
        _cardData = instance.cardData;

        // � ��� ��� ��� �� ����� � ����
        _isPlayerHand = false;
        IsEnemy = !isPlayerSide;

        // ����������, ��� ��� ����
        _isEnemySide = !isPlayerSide;

        // ��� � ���
        if (_artworkImage != null)
        {
            _artworkImage.gameObject.SetActive(true);
            _artworkImage.sprite = _cardData.artwork;
        }
        if (_nameText != null)
            _nameText.text = _cardData.cardName;

        // ����������
        if (_loyaltyText != null)
            _loyaltyText.text = instance.currentLoyalty.ToString();

        // ������
        if (_defenseText != null)
            _defenseText.text = instance.CurrentDefense.ToString();

        // HP
        if (_hpText != null)
            _hpText.text = instance.currentHP.ToString();

<<<<<<< HEAD
        // �������� ����������� (������, ���� ����)
=======
        // �������� ����������� (����� ������, ���� ����)
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        if (_abilityDescText != null)
        {
            if (_cardData.abilities != null && _cardData.abilities.Length > 0)
            {
                _abilityDescText.text = _cardData.abilities[0].description;
                _abilityDescText.gameObject.SetActive(true);
            }
            else
            {
                _abilityDescText.gameObject.SetActive(false);
            }
        }

        // ���������
        if (_highlightBorder != null)
            _highlightBorder.enabled = false;

        // ����
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);
    }
<<<<<<< HEAD
=======

>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    /// <summary>��������/��������� ��������� �����.</summary>
    public void SetHighlight(bool highlight)
    {
        if (_highlightBorder != null)
            _highlightBorder.enabled = highlight;
    }

    #endregion

    #region Event Handlers

    private void OnClicked()
    {
        if (!_isPlayerHand) return;

        switch (_cardData.type)
        {
<<<<<<< HEAD
            case CardType.Minion:
            case CardType.Spell:
            case CardType.HeroAbility:
                // ���� � �� �� �������� � �������� �����������
                var inst = TurnManager.Instance.TryPlayCard(_cardData, isPlayer:true);
                if (_cardData.type != CardType.Minion && inst != null)
                {
                    // inst ����� null ��� �������, �� ����������� ��� ������� � ������ �������
                }
                if (inst == null && _cardData.type == CardType.Minion)
                {
                    Debug.LogWarning($"Not enough loyalty to play {_cardData.cardName}");
                    return;
                }

                // ������� ����� �� ����
                HandManager.Instance.TryPlayCard(_cardData, true);

                // ���� ��� ������ � ���������� ��� � PlayedArea
                if (inst != null)
                    UIManager.Instance.PlacePlayedCard(inst, true);

                // ��� ������/����. ����������� � ����� ������ � ���������
                if (_cardData.type != CardType.Minion)
                    UIManager.Instance.BeginTargetMode(_cardData.abilities[0]);

                UIManager.Instance.RefreshHands();
                break;

            case CardType.Hero:
                // ����� �� ���� �� ������
                break;
=======
            switch (_cardData.type)
            {
                case CardType.Minion:
                    // �������� �������� ���������� ��������
                    var minionInst = TurnManager.Instance.TryPlayCard(_cardData, isPlayer: true);
                    if (minionInst == null)
                    {
                        Debug.LogWarning($"Not enough loyalty to play {_cardData.cardName}");
                        return;
                    }
                    HandManager.Instance.TryPlayCard(_cardData, true);
                    UIManager.Instance.UpdateLoyaltyDisplay();
                    UIManager.Instance.PlacePlayedCard(minionInst, true);
                    GameManager.Instance.RecordPlayed(minionInst);
                    UIManager.Instance.RefreshHands();
                    break;

                case CardType.Spell:
                case CardType.HeroAbility:
                    // �������� �������� ������/����. ����������� ��������
                    // 1) ���� ������ AbilityData
                    if (_cardData.abilities == null || _cardData.abilities.Length == 0)
                    {
                        Debug.LogWarning($"No abilities assigned to {_cardData.cardName}");
                        return;
                    }
                    var ability = _cardData.abilities[0];

                    // 2) ������� ������� ���������� � ��������� OnPlay-��������
                    //    ���������� ��� �� �����, ��� � ��� ���������: �� ����� ������� ApplyCardEffect ��� OnPlay/Passive
                    var spellInst = TurnManager.Instance.TryPlayCard(_cardData, isPlayer: true);
                    if (spellInst == null)
                    {
                        Debug.LogWarning($"Not enough loyalty to cast {_cardData.cardName}");
                        return;
                    }

                    // 3) ������� ����� �� ����
                    HandManager.Instance.TryPlayCard(_cardData, true);

                    // 4) ��������� UI ����
                    UIManager.Instance.RefreshHands();

                    // 5) ������ � ����� ���������� ��� ��������� �����������
                    UIManager.Instance.BeginTargetMode(ability);
                    break;

                case CardType.Hero:
                    // ����� �� ���� �� ������
                    break;
            }
        }
        else if (UIManager.Instance.IsTargeting && _cardInstance != null)
        {
            // �������� ���������� ������ � ���� ��������
            EffectProcessor.Instance.ApplyEffectTo(_cardInstance);
            UIManager.Instance.EndTargetMode();
            UIManager.Instance.RefreshBattlefield();
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        }
    }

    #endregion
}
