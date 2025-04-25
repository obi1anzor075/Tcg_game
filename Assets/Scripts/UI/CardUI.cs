using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class CardUI : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] private Image _artworkImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _loyaltyText;      // ����� ���� �� �����
    [SerializeField] private TMP_Text _hpText;           // ����� ���� �� �����
    [SerializeField] private TMP_Text _abilityDescText;  // ����� ���� �� �����

    #endregion

    #region State

    private CardData _cardData;
    private CardInstance _cardInstance;
    private bool _isPlayerHand;

    #endregion

    #region Public API

    /// <summary>
    /// ��������� UI ��� ����� � ����.
    /// </summary>
    public void Setup(CardData data, bool isPlayer)
    {
        _cardData = data;
        _cardInstance = null;
        _isPlayerHand = isPlayer;

        // ��� � ���
        if (_artworkImage != null) _artworkImage.sprite = data.artwork;
        if (_nameText != null) _nameText.text = data.cardName;

        // ����������
        if (_loyaltyText != null) _loyaltyText.text = data.baseLoyalty.ToString();

        // HP
        if (_hpText != null) _hpText.text = data.maxHP.ToString();

        // �������� �����������
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

        // ��������� �����
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);

    }

    /// <summary>
    /// �������� ������� ����� (��� ��).
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
    /// ��������� UI ��� ����� �� ���� (�����).
    /// </summary>
    public void SetupBattle(CardInstance instance)
    {
        _cardInstance = instance;
        _cardData = instance.cardData;
        _isPlayerHand = false;

        // ��� � ���
        if (_artworkImage != null) _artworkImage.sprite = _cardData.artwork;
        if (_nameText != null) _nameText.text = _cardData.cardName;

        // ����������
        if (_loyaltyText != null) _loyaltyText.text = instance.currentLoyalty.ToString();

        // HP
        if (_hpText != null) _hpText.text = instance.currentHP.ToString();

        // �������� �����������
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

        // ��������� �����
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);

    }

    /// <summary>
    /// ��������/��������� ��������� ����� (���� ���� �������).
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
            // 1. ���������� ���������
            int cost = (_cardData.type == CardType.Minion)
                ? _cardData.baseLoyalty
                : _cardData.loyaltyCost;

            // 2. ��������� � ��������� �����������
            if (!TurnManager.Instance.TrySpendLoyalty(cost))
            {
                Debug.LogWarning("Not enough loyalty to play " + _cardData.cardName);
                return;
            }

            // 3. ������� ����� �� ����
            if (!HandManager.Instance.TryPlayCard(_cardData, true))
                return;

            // 4. ������ ��������� ��������� � ���������� � ������ ���������
            var inst = new CardInstance(_cardData);
            UIManager.Instance.PlacePlayedCard(inst, true);

            // 5. ����������, ����� ����� ������� �
            GameManager.Instance.RecordPlayed(inst);

            // 6. ��������� UI ����
            UIManager.Instance.RefreshHands();
        }
        else if (UIManager.Instance.IsTargeting && _cardInstance != null)
        {
            // ��������� ������������
            EffectProcessor.Instance.ApplyEffectTo(_cardInstance, UIManager.Instance.CurrentEffectCard);
            UIManager.Instance.EndTargetMode();
            UIManager.Instance.RefreshBattlefield();
        }
    }


    #endregion
}
