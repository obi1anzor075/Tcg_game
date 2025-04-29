using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "TCG/Card")]
public class CardData : ScriptableObject
{
    [Header("General")]
    public string cardName;
    public Sprite artwork;
    public CardType type;

    [Header("Stats (Hero & Minion)")]
<<<<<<< HEAD
    public int maxHP;           // текущее здоровье
    public int attack;          // сила атаки
=======
    public int baseHealth;           // текущее здоровье
    public int baseAttack;          // сила атаки
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public int baseDefense;     // щит (defense)
    public int baseLoyalty;     // базовая лояльность (cost для Minion)

    [Header("Cost (Spell & HeroAbility)")]
    public int loyaltyCost;     // стоимость в лояльности для Spell / HeroAbility

    [Header("Abilities")]
    [Tooltip("Список способностей: пассивных и триггерных (OnPlay, OnAttack и т.д.)")]
    public AbilityData[] abilities;
}
