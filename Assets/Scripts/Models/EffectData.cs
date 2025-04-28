using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effect")]
public class EffectData : ScriptableObject
{
    public EffectType type;             // ��� �������
    public int value;                   // ��������, ���� ��� ���������� loyalty
    public bool targetsOwnPlayer;       // �� ���� ���������: true � �� ���������, false � �� ����������
}