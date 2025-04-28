using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;

    public AbilityTrigger trigger;      // когда срабатывает
    public TargetType targetType;      
    public bool isPriorityTarget;       // для пассивной-способности «приоритет»(если это приоритетная цель)


    // Список эффектов, которые нужно применить
    public EffectData[] effects;
}