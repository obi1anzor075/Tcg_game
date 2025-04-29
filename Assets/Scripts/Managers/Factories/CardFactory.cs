using UnityEngine;

public class CardFactory : MonoBehaviour
{
    public static CardFactory Instance { get; private set; }

    [Header("Field Prefabs")]
    [SerializeField] private GameObject _heroFieldPrefab;
    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private GameObject _spellPrefab;

    [Header("Hand Prefabs")]
    [SerializeField] private GameObject _minionHandPrefab;
    [SerializeField] private GameObject _spellHandPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    /// <summary>
<<<<<<< HEAD
    /// ������ ����� �� ����, ��������� ���� �������.
    /// </summary>
    public CardUI CreateHeroOnField(CardData data, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_heroFieldPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(new CardInstance(data), isPlayer);
=======
    /// ������ ������ ����� �� ���� � �������, ��� ��� �����.
    /// </summary>
    public CardUI CreateHeroOnField(CardData data, Transform parent, bool isPlayerSide)
    {
        var go = Instantiate(_heroFieldPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(new CardInstance(data), isPlayerSide);
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        return ui;
    }

    /// <summary>
<<<<<<< HEAD
    /// ������ ������� ���� � ����, ���� �� ����.
    /// </summary>
    public CardUI CreateMinion(CardData data, Transform parent, bool inHand, bool isPlayer)
=======
    /// ������ ������� ���� ��� ����, ���� ��� ����.
    /// </summary>
    public CardUI CreateMinion(CardData data, Transform parent, bool inHand, bool isPlayerSide)
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    {
        var prefab = inHand ? _minionHandPrefab : _minionPrefab;
        var go = Instantiate(prefab, parent);
        var ui = go.GetComponent<CardUI>();
<<<<<<< HEAD
        if (inHand) ui.Setup(data, isPlayer);
        else ui.SetupBattle(new CardInstance(data), isPlayer);
=======
        if (inHand)
            ui.Setup(data, isPlayerSide);
        else
            ui.SetupBattle(new CardInstance(data), isPlayerSide);
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        return ui;
    }

    /// <summary>
<<<<<<< HEAD
    /// ������ ����� ���� � ����, ���� �� ����.
    /// </summary>
    public CardUI CreateSpell(CardData data, Transform parent, bool inHand, bool isPlayer)
=======
    /// ������ ����� ���� ��� ����, ���� (�����������) ��� ����.
    /// </summary>
    public CardUI CreateSpell(CardData data, Transform parent, bool inHand, bool isPlayerSide)
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    {
        var prefab = inHand ? _spellHandPrefab : _spellPrefab;
        var go = Instantiate(prefab, parent);
        var ui = go.GetComponent<CardUI>();
<<<<<<< HEAD
        if (inHand) ui.Setup(data, isPlayer);
        else ui.SetupBattle(new CardInstance(data), isPlayer);
=======
        if (inHand)
            ui.Setup(data, isPlayerSide);
        else
            ui.SetupBattle(new CardInstance(data), isPlayerSide);
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
        return ui;
    }

    /// <summary>
<<<<<<< HEAD
    /// ��� ������� �� ����: ��������� ������� ���������.
=======
    /// ��� ��� ���������� ���������� ����� �� ���� (��������, � PlayedArea ��� ��� RefreshBattlefield).
    /// </summary>
    public CardUI CreateHeroOnFieldInstance(CardInstance inst, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_heroFieldPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(inst, isPlayer);
        return ui;
    }

    /// <summary>
    /// ��� ��� ���������� ���������� ������� �� ���� (��������, � PlayedArea ��� ��� RefreshBattlefield).
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    /// </summary>
    public CardUI CreateMinionInstance(CardInstance inst, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_minionPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(inst, isPlayer);
        return ui;
    }

<<<<<<< HEAD
    /// <summary>
    /// ��� ������ �� ���� (���� ���� ��������).
=======

    /// <summary>
    /// ��� ��� ���������� ���������� ������ �� ���� (���� �� ��� ���������������).
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    /// </summary>
    public CardUI CreateSpellInstance(CardInstance inst, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_spellPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(inst, isPlayer);
        return ui;
    }
}
