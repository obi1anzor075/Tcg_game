using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;

    public AbilityTrigger trigger;      // ����� �����������
<<<<<<< HEAD
    public bool isPriorityTarget;       // ��� ���������-����������� ����������(���� ��� ������������ ����)

=======
    public TargetType targetType;      
    public bool isPriorityTarget;       // ��� ���������-����������� ����������(���� ��� ������������ ����)


>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    // ������ ��������, ������� ����� ���������
    public EffectData[] effects;
}