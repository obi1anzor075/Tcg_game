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
        // Гарантированно убиваем любые твины, привязанные к visual или этому объекту
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

    /// <summary>Настроить UI для карты в руке.</summary>
    public void Setup(CardData data, bool isPlayer)
    {
        _cardData = data;
        _cardInstance = null;
        _isPlayerHand = isPlayer;

        // Сохраняем LayoutElement и временно отключаем layout, чтобы анимация не дергалась
        var layoutElem = GetComponent<LayoutElement>();
        if (layoutElem != null) layoutElem.ignoreLayout = false;

        // Арт и имя
        _artworkImage?.gameObject.SetActive(true);
        _artworkImage.sprite = data.artwork;
        _nameText.text = data.cardName;

        // Преданность/стоимость
        if (_loyaltyText != null)
        {
            _loyaltyText.text = (data.type == CardType.Minion)
                ? data.baseLoyalty.ToString()
                : data.loyaltyCost.ToString();
        }

        // Защита и HP (для рука, можно скрыть)
        _defenseText.text = data.baseDefense.ToString();
        _hpText.text = data.maxHP.ToString();

        // Описание способностей
        if (_abilityDescText != null)
        {
            if (data.abilities != null && data.abilities.Length > 0)
            {
                // просто берем описание первой способности
                _abilityDescText.text = data.abilities[0].description;
                _abilityDescText.gameObject.SetActive(true);
            }
            else
            {
                _abilityDescText.gameObject.SetActive(false);
            }
        }

        // Скрыть подсветку
        if (_highlightBorder != null) _highlightBorder.enabled = false;

        // Подписываем клик
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);
    }

    /// <summary>Показать рубашку карты (для ИИ).</summary>
    public void SetupBack()
    {
        _artworkImage?.gameObject.SetActive(false);
        _nameText.text = "";
        _loyaltyText.text = "";
        _defenseText.text = "";
        _hpText.text = "";
        _abilityDescText.gameObject.SetActive(false);
        _highlightBorder.enabled = false;
    }

    /// <summary>Настроить UI для карты на поле (битва).</summary>
    /// <summary>
    /// Настроить UI для карты на поле (битва).
    /// </summary>
    public void SetupBattle(CardInstance instance, bool isPlayerSide)
    {
        _cardInstance = instance;
        _cardData = instance.cardData;

        // в бою это уже не карта в руке
        _isPlayerHand = false;

        // запоминаем, чей это боец
        _isEnemySide = !isPlayerSide;

        // Арт и имя
        if (_artworkImage != null)
        {
            _artworkImage.gameObject.SetActive(true);
            _artworkImage.sprite = _cardData.artwork;
        }
        if (_nameText != null)
            _nameText.text = _cardData.cardName;

        // Лояльность
        if (_loyaltyText != null)
            _loyaltyText.text = instance.currentLoyalty.ToString();

        // Защита
        if (_defenseText != null)
            _defenseText.text = instance.CurrentDefense.ToString();

        // HP
        if (_hpText != null)
            _hpText.text = instance.currentHP.ToString();

        // Описание способности (первая, если есть)
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

        // Подсветка
        if (_highlightBorder != null)
            _highlightBorder.enabled = false;

        // Клик
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);
    }
    /// <summary>Включить/выключить подсветку карты.</summary>
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
            case CardType.Minion:
            case CardType.Spell:
            case CardType.HeroAbility:
                // одна и та же проверка и списание преданности
                var inst = TurnManager.Instance.TryPlayCard(_cardData, isPlayer:true);
                if (_cardData.type != CardType.Minion && inst != null)
                {
                    // inst будет null для спеллов, но преданность уже списана и эффект применён
                }
                if (inst == null && _cardData.type == CardType.Minion)
                {
                    Debug.LogWarning($"Not enough loyalty to play {_cardData.cardName}");
                    return;
                }

                // убираем карту из руки
                HandManager.Instance.TryPlayCard(_cardData, true);

                // если это миньон — показываем его в PlayedArea
                if (inst != null)
                    UIManager.Instance.PlacePlayedCard(inst, true);

                // для спелла/геро. способности — сразу входим в таргетинг
                if (_cardData.type != CardType.Minion)
                    UIManager.Instance.BeginTargetMode(_cardData.abilities[0]);

                UIManager.Instance.RefreshHands();
                break;

            case CardType.Hero:
                // героя из руки не играем
                break;
        }
    }

    #endregion
}
