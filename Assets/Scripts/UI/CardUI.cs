using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class CardUI : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] private Image _artworkImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _loyaltyText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private Image _highlightBorder;
    [SerializeField] private TMP_Text _abilityDescText; // для описания способности

    #endregion

    #region State

    private CardData _cardData;
    private CardInstance _cardInstance;
    private bool _isPlayerHand;

    #endregion

    #region Public API

    /// <summary>
    /// Настроить UI для карты в руке.
    /// </summary>
    public void Setup(CardData data, bool isPlayer)
    {
        _cardData = data;
        _cardInstance = null;
        _isPlayerHand = isPlayer;

        _artworkImage.sprite = data.artwork;
        _nameText.text = data.cardName;
        _loyaltyText.text = data.baseLoyalty.ToString();
        _hpText.text = data.maxHP.ToString();

        // Описание способности, если есть
        if (data.ability != null)
        {
            _abilityDescText.text = data.ability.description;
            _abilityDescText.gameObject.SetActive(true);
        }
        else
        {
            _abilityDescText.gameObject.SetActive(false);
        }

        // Настройка клика
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);

        // Скрыть подсветку
        _highlightBorder.enabled = false;
    }

    /// <summary>
    /// Отображение рубашки (для ИИ).
    /// </summary>
    public void SetupBack()
    {
        _artworkImage.gameObject.SetActive(false);
        _nameText.text = "";
        _loyaltyText.text = "";
        _hpText.text = "";
        _abilityDescText.gameObject.SetActive(false);
        _highlightBorder.enabled = false;
    }

    /// <summary>
    /// Настроить UI для карты на поле.
    /// </summary>
    public void SetupBattle(CardInstance instance)
    {
        _cardInstance = instance;
        _cardData = instance.cardData;
        _isPlayerHand = false;

        _artworkImage.sprite = _cardData.artwork;
        _nameText.text = _cardData.cardName;
        _loyaltyText.text = instance.currentLoyalty.ToString();
        _hpText.text = instance.currentHP.ToString();

        // Описание способности, если есть
        if (_cardData.ability != null)
        {
            _abilityDescText.text = _cardData.ability.description;
            _abilityDescText.gameObject.SetActive(true);
        }
        else
        {
            _abilityDescText.gameObject.SetActive(false);
        }

        // Настройка клика
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);

        _highlightBorder.enabled = false;
    }

    /// <summary>
    /// Включить/выключить подсветку.
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        _highlightBorder.enabled = highlight;
    }

    #endregion

    #region Event Handlers

    private void OnClicked()
    {
        if (UIManager.Instance.IsTargeting && _cardInstance != null)
        {
            EffectProcessor.Instance.ApplyEffectTo(_cardInstance, UIManager.Instance.CurrentEffectCard);
            UIManager.Instance.EndTargetMode();
            UIManager.Instance.RefreshBattlefield();
        }
        else if (_isPlayerHand)
        {
            if (!HandManager.Instance.TryPlayCard(_cardData, true))
                return;

            TurnManager.Instance.PlayCard(_cardData);

            if (_cardData.type == CardType.Spell || _cardData.type == CardType.HeroAbility)
                UIManager.Instance.BeginTargetMode(_cardData);
            else
                UIManager.Instance.RefreshBattlefield();

            UIManager.Instance.RefreshHands();
        }
    }

    #endregion
}
