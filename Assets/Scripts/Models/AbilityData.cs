using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;

    public AbilityTrigger trigger;      // ����� �����������
    public TargetType targetType;      
    public bool isPriorityTarget;       // ��� ���������-����������� ����������(���� ��� ������������ ����)


    // ������ ��������, ������� ����� ���������
    public EffectData[] effects;
}