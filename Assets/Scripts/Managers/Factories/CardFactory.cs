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
    /// Создаёт героя на поле, передавая флаг стороны.
    /// </summary>
    public CardUI CreateHeroOnField(CardData data, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_heroFieldPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(new CardInstance(data), isPlayer);
=======
    /// Создаёт префаб героя на поле и передаёт, чей это герой.
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
    /// Создаёт миньона либо в руке, либо на поле.
    /// </summary>
    public CardUI CreateMinion(CardData data, Transform parent, bool inHand, bool isPlayer)
=======
    /// Создаёт миньона либо для руки, либо для поля.
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
    /// Создаёт спелл либо в руке, либо на поле.
    /// </summary>
    public CardUI CreateSpell(CardData data, Transform parent, bool inHand, bool isPlayer)
=======
    /// Создаёт спелл либо для руки, либо (опционально) для поля.
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
    /// Для миньона на поле: принимает готовый экземпляр.
=======
    /// Для уже созданного экземпляра героя на поле (например, в PlayedArea или при RefreshBattlefield).
    /// </summary>
    public CardUI CreateHeroOnFieldInstance(CardInstance inst, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_heroFieldPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(inst, isPlayer);
        return ui;
    }

    /// <summary>
    /// Для уже созданного экземпляра миньона на поле (например, в PlayedArea или при RefreshBattlefield).
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
    /// Для спелла на поле (если есть визуалка).
=======

    /// <summary>
    /// Для уже созданного экземпляра спелла на поле (если он там визуализируется).
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
