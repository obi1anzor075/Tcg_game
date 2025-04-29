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
    public int maxHP;           // ������� ��������
    public int attack;          // ���� �����
=======
    public int baseHealth;           // ������� ��������
    public int baseAttack;          // ���� �����
>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    public int baseDefense;     // ��� (defense)
    public int baseLoyalty;     // ������� ���������� (cost ��� Minion)

    [Header("Cost (Spell & HeroAbility)")]
    public int loyaltyCost;     // ��������� � ���������� ��� Spell / HeroAbility

    [Header("Abilities")]
    [Tooltip("������ ������������: ��������� � ���������� (OnPlay, OnAttack � �.�.)")]
    public AbilityData[] abilities;
}
