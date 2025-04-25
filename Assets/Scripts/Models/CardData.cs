using UnityEngine;

public enum CardType { Hero, Minion, Spell, HeroAbility }

[CreateAssetMenu(fileName = "NewCard", menuName = "TCG/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite artwork;

    public CardType type;

    // ����� ��������� ��� �������� � ��
    public int maxHP;           // �������� ��� ����� � ������
    public int attack;          // ����� ��� ����� � ������
    public int baseLoyalty;     // �������� ������ ��� ������ (��� ����� ����� �������� 0)
    public AbilityData ability; // �����������: � ����� � ��� ������, � ������� � ���

    // ��� ������� � ����������� ������������
    public int loyaltyCost;     // ������� �������� ����� ���������
    public AbilityData[] effects;
}
