using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "TCG/Card")]
public class CardData : ScriptableObject
{
    [Header("General")]
    public string cardName;
    public Sprite artwork;
    public CardType type;

    [Header("Stats (Hero & Minion)")]
    public int baseHealth;           // ������� ��������
    public int baseAttack;          // ���� �����
    public int baseDefense;     // ��� (defense)
    public int baseLoyalty;     // ������� ���������� (cost ��� Minion)

    [Header("Cost (Spell & HeroAbility)")]
    public int loyaltyCost;     // ��������� � ���������� ��� Spell / HeroAbility

    [Header("Abilities")]
    [Tooltip("������ ������������: ��������� � ���������� (OnPlay, OnAttack � �.�.)")]
    public AbilityData[] abilities;
}
