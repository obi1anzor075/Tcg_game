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

    public CardUI CreateHeroOnField(CardData data, Transform parent)
    {
        var go = Instantiate(_heroFieldPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(new CardInstance(data));
        return ui;
    }

    public CardUI CreateMinion(CardData data, Transform parent, bool inHand, bool isPlayer)
    {
        var prefab = inHand ? _minionHandPrefab : _minionPrefab;
        var go = Instantiate(prefab, parent);
        var ui = go.GetComponent<CardUI>();
        if (inHand) ui.Setup(data, isPlayer);
        else ui.SetupBattle(new CardInstance(data));
        return ui;
    }

    public CardUI CreateSpell(CardData data, Transform parent, bool inHand, bool isPlayer)
    {
        var prefab = inHand ? _spellHandPrefab : _spellPrefab;
        var go = Instantiate(prefab, parent);
        var ui = go.GetComponent<CardUI>();
        if (inHand) ui.Setup(data, isPlayer);
        else ui.SetupBattle(new CardInstance(data));
        return ui;
    }

    /// <summary>
    /// ƒл€ миньона на поле: принимает готовый экземпл€р и вызывает SetupBattle.
    /// </summary>
    public CardUI CreateMinionInstance(CardInstance instance, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_minionPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(instance);
        return ui;
    }

    /// <summary>
    /// ƒл€ спелла на поле (если есть визуалка на поле).
    /// </summary>
    public CardUI CreateSpellInstance(CardInstance instance, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_spellPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(instance);
        return ui;
    }
}
