using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "TCG/Card")]
public class CardData : ScriptableObject
{
    [Header("General")]
    public string cardName;
    public Sprite artwork;
    public CardType type;

    [Header("Stats (Hero & Minion)")]
    public int baseHealth;           // текущее здоровье
    public int baseAttack;          // сила атаки
    public int baseDefense;     // щит (defense)
    public int baseLoyalty;     // базовая лояльность (cost для Minion)

    [Header("Cost (Spell & HeroAbility)")]
    public int loyaltyCost;     // стоимость в лояльности для Spell / HeroAbility

    [Header("Abilities")]
    [Tooltip("Список способностей: пассивных и триггерных (OnPlay, OnAttack и т.д.)")]
    public AbilityData[] abilities;
}
