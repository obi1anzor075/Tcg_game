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
        IsEnemy = !_isPlayerHand; 

        // Сохраняем LayoutElement и временно отключаем layout, чтобы анимация не дергалась
        var layoutElem = GetComponent<LayoutElement>();
        if (layoutElem != null) layoutElem.ignoreLayout = false;

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
<<<<<<< HEAD
        _hpText.text = data.maxHP.ToString();
=======
        _hpText.text = data.baseHealth.ToString();
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee

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
<<<<<<< HEAD
    }

    /// <summary>Настроить UI для карты на поле (битва).</summary>
    /// <summary>
    /// Настроить UI для карты на поле (битва).
    /// </summary>
=======
        IsEnemy = true;             
    }

    /// <summary>Настроить UI для карты на поле (битва).</summary>
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public void SetupBattle(CardInstance instance, bool isPlayerSide)
    {
        _cardInstance = instance;
        _cardData = instance.cardData;

        // в бою это уже не карта в руке
        _isPlayerHand = false;
        IsEnemy = !isPlayerSide;

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

<<<<<<< HEAD
        // Описание способности (первая, если есть)
=======
        // Описание способности (берем первую, если есть)
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

        // Подсветка
        if (_highlightBorder != null)
            _highlightBorder.enabled = false;

        // Клик
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);
    }
<<<<<<< HEAD
=======

>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
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
<<<<<<< HEAD
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
=======
            switch (_cardData.type)
            {
                case CardType.Minion:
                    // ———————— Розыгрыш преданного ————————
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
                    // ———————— Розыгрыш спелла/геро. способности ————————
                    // 1) Берём первую AbilityData
                    if (_cardData.abilities == null || _cardData.abilities.Length == 0)
                    {
                        Debug.LogWarning($"No abilities assigned to {_cardData.cardName}");
                        return;
                    }
                    var ability = _cardData.abilities[0];

                    // 2) Пробуем списать лояльность и применить OnPlay-пассивки
                    //    Используем тот же метод, что и для розыгрыша: он сразу вызовет ApplyCardEffect для OnPlay/Passive
                    var spellInst = TurnManager.Instance.TryPlayCard(_cardData, isPlayer: true);
                    if (spellInst == null)
                    {
                        Debug.LogWarning($"Not enough loyalty to cast {_cardData.cardName}");
                        return;
                    }

                    // 3) Удаляем карту из руки
                    HandManager.Instance.TryPlayCard(_cardData, true);

                    // 4) Обновляем UI руки
                    UIManager.Instance.RefreshHands();

                    // 5) Входим в режим таргетинга для выбранной способности
                    UIManager.Instance.BeginTargetMode(ability);
                    break;

                case CardType.Hero:
                    // героя из руки не играем
                    break;
            }
        }
        else if (UIManager.Instance.IsTargeting && _cardInstance != null)
        {
            // ———————— Применение спелла к цели ————————
            EffectProcessor.Instance.ApplyEffectTo(_cardInstance);
            UIManager.Instance.EndTargetMode();
            UIManager.Instance.RefreshBattlefield();
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        }
    }

    #endregion
}
