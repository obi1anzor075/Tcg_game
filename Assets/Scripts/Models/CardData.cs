using UnityEngine;

public enum CardType { Hero, Minion, Spell, HeroAbility }

[CreateAssetMenu(fileName = "NewCard", menuName = "TCG/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite artwork;

    public CardType type;

    // Общие параметры для карточек с хп
    public int maxHP;           // здоровье для героя и верных
    public int attack;          // атака для героя и верных
    public int baseLoyalty;     // верность только для верных (для героя можно оставить 0)
    public AbilityData ability; // способность: у героя – его особая, у верного – его

    // Для спеллов и героических способностей
    public int loyaltyCost;     // сколько верности нужно потратить
    public AbilityData[] effects;
}
