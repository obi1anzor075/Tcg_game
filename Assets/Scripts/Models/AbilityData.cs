using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;

    public AbilityTrigger trigger;      // когда срабатывает
<<<<<<< HEAD
    public bool isPriorityTarget;       // для пассивной-способности «приоритет»(если это приоритетная цель)

=======
    public TargetType targetType;      
    public bool isPriorityTarget;       // для пассивной-способности «приоритет»(если это приоритетная цель)


>>>>>>> 605d83d0ea2028f7b1cb35ae24fb7cb8822377ee
    // Список эффектов, которые нужно применить
    public EffectData[] effects;
}