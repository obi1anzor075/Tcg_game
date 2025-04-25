using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "TCG/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;

    // �������
    public int damage;
    public int heal;

    // ��������� ��������
    [Tooltip("�� ������� ������������� (����) ��� ����������� (�����) �������� ����")]
    public int loyaltyDelta;

    [Tooltip("���� true � ������ loyaltyDelta ����������� ������ ���, ����� ���� ��� ��� ��������� �����������")]
    public bool loyaltyPerTurn;

    public bool isPriorityTarget; // ���������
}
