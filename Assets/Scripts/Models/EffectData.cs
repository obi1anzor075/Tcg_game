using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effect")]
public class EffectData : ScriptableObject
{
    public EffectType type;             // тип эффекта
    public int value;                   // например, урон или увеличение loyalty
    public bool targetsOwnPlayer;       // на кого действует: true Ч на владельца, false Ч на противника
}