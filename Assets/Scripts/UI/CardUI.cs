using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class CardUI : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] private Image _artworkImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _loyaltyText;      // может быть не задан
    [SerializeField] private TMP_Text _hpText;           // может быть не задан
    [SerializeField] private TMP_Text _abilityDescText;  // может быть не задан

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

        // Арт и имя
        if (_artworkImage != null) _artworkImage.sprite = data.artwork;
        if (_nameText != null) _nameText.text = data.cardName;

        // Лояльность
        if (_loyaltyText != null) _loyaltyText.text = data.baseLoyalty.ToString();

        // HP
        if (_hpText != null) _hpText.text = data.maxHP.ToString();

        // Описание способности
        if (_abilityDescText != null)
        {
            if (data.ability != null)
            {
                _abilityDescText.text = data.ability.description;
                _abilityDescText.gameObject.SetActive(true);
            }
            else
            {
                _abilityDescText.gameObject.SetActive(false);
            }
        }

        // Настройка клика
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);

    }

    /// <summary>
    /// Показать рубашку карты (для ИИ).
    /// </summary>
    public void SetupBack()
    {
        if (_artworkImage != null) _artworkImage.gameObject.SetActive(false);
        if (_nameText != null) _nameText.text = "";
        if (_loyaltyText != null) _loyaltyText.text = "";
        if (_hpText != null) _hpText.text = "";
        if (_abilityDescText != null) _abilityDescText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Настроить UI для карты на поле (битва).
    /// </summary>
    public void SetupBattle(CardInstance instance)
    {
        _cardInstance = instance;
        _cardData = instance.cardData;
        _isPlayerHand = false;

        // Арт и имя
        if (_artworkImage != null) _artworkImage.sprite = _cardData.artwork;
        if (_nameText != null) _nameText.text = _cardData.cardName;

        // Лояльность
        if (_loyaltyText != null) _loyaltyText.text = instance.currentLoyalty.ToString();

        // HP
        if (_hpText != null) _hpText.text = instance.currentHP.ToString();

        // Описание способности
        if (_abilityDescText != null)
        {
            if (_cardData.ability != null)
            {
                _abilityDescText.text = _cardData.ability.description;
                _abilityDescText.gameObject.SetActive(true);
            }
            else
            {
                _abilityDescText.gameObject.SetActive(false);
            }
        }

        // Настройка клика
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);

    }

    /// <summary>
    /// Включить/выключить подсветку карты (если есть граница).
    /// </summary>
    public void SetHighlight(bool highlight)
    {

    }

    #endregion

    #region Event Handlers

    private void OnClicked()
    {
        if (_isPlayerHand)
        {
            // 1. Определяем стоимость
            int cost = (_cardData.type == CardType.Minion)
                ? _cardData.baseLoyalty
                : _cardData.loyaltyCost;

            // 2. Проверяем и списываем преданность
            if (!TurnManager.Instance.TrySpendLoyalty(cost))
            {
                Debug.LogWarning("Not enough loyalty to play " + _cardData.cardName);
                return;
            }

            // 3. Убираем карту из руки
            if (!HandManager.Instance.TryPlayCard(_cardData, true))
                return;

            // 4. Создаём временный экземпляр и показываем в панели сыгранных
            var inst = new CardInstance(_cardData);
            UIManager.Instance.PlacePlayedCard(inst, true);

            // 5. Запоминаем, чтобы потом «убить» её
            GameManager.Instance.RecordPlayed(inst);

            // 6. Обновляем UI руки
            UIManager.Instance.RefreshHands();
        }
        else if (UIManager.Instance.IsTargeting && _cardInstance != null)
        {
            // таргетинг способностей
            EffectProcessor.Instance.ApplyEffectTo(_cardInstance, UIManager.Instance.CurrentEffectCard);
            UIManager.Instance.EndTargetMode();
            UIManager.Instance.RefreshBattlefield();
        }
    }


    #endregion
}
