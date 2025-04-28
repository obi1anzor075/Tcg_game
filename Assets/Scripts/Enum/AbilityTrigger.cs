public enum AbilityTrigger
{
    Passive,       // всегда действует (например, приоритетная цель)
    OnPlay,        // при разыгрывании
    OnAttack,      // когда эта карта атакует
    OnDamaged,     // когда эта карта получает урон
    OnTurnStart,   // в начале хода
}