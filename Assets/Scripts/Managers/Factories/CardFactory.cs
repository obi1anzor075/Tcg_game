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
    /// Создаёт префаб героя на поле и передаёт, чей это герой.
    /// </summary>
    public CardUI CreateHeroOnField(CardData data, Transform parent, bool isPlayerSide)
    {
        var go = Instantiate(_heroFieldPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(new CardInstance(data), isPlayerSide);
        return ui;
    }

    /// <summary>
    /// Создаёт миньона либо для руки, либо для поля.
    /// </summary>
    public CardUI CreateMinion(CardData data, Transform parent, bool inHand, bool isPlayerSide)
    {
        var prefab = inHand ? _minionHandPrefab : _minionPrefab;
        var go = Instantiate(prefab, parent);
        var ui = go.GetComponent<CardUI>();
        if (inHand)
            ui.Setup(data, isPlayerSide);
        else
            ui.SetupBattle(new CardInstance(data), isPlayerSide);
        return ui;
    }

    /// <summary>
    /// Создаёт спелл либо для руки, либо (опционально) для поля.
    /// </summary>
    public CardUI CreateSpell(CardData data, Transform parent, bool inHand, bool isPlayerSide)
    {
        var prefab = inHand ? _spellHandPrefab : _spellPrefab;
        var go = Instantiate(prefab, parent);
        var ui = go.GetComponent<CardUI>();
        if (inHand)
            ui.Setup(data, isPlayerSide);
        else
            ui.SetupBattle(new CardInstance(data), isPlayerSide);
        return ui;
    }

    /// <summary>
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
    /// </summary>
    public CardUI CreateMinionInstance(CardInstance inst, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_minionPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(inst, isPlayer);
        return ui;
    }


    /// <summary>
    /// Для уже созданного экземпляра спелла на поле (если он там визуализируется).
    /// </summary>
    public CardUI CreateSpellInstance(CardInstance inst, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_spellPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(inst, isPlayer);
        return ui;
    }

    /// <summary>
    /// Для миньона на поле: принимает готовый экземпляр и вызывает SetupBattle.
    /// </summary>
    public CardUI CreateMinionInstance(CardInstance instance, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_minionPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(instance);
        return ui;
    }

    /// <summary>
    /// Для спелла на поле (если есть визуалка на поле).
    /// </summary>
    public CardUI CreateSpellInstance(CardInstance instance, Transform parent, bool isPlayer)
    {
        var go = Instantiate(_spellPrefab, parent);
        var ui = go.GetComponent<CardUI>();
        ui.SetupBattle(instance);
        return ui;
    }
}
