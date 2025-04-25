using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "TCG/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;

    // эффекты
    public int damage;
    public int heal;

    // изменение верности
    [Tooltip("На сколько увеличивается (плюс) или уменьшается (минус) верность цели")]
    public int loyaltyDelta;

    [Tooltip("Если true — эффект loyaltyDelta применяется каждый ход, иначе один раз при активации способности")]
    public bool loyaltyPerTurn;

    public bool isPriorityTarget; // приоритет
}
