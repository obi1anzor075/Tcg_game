using UnityEngine;

public class CardInstance
{
    public CardData cardData;

    public int currentHP;
    public int currentLoyalty;

    public bool IsAlive => currentHP > 0;
    public bool HasSwitchedSides => currentLoyalty < 0;
    public bool CanAct => currentLoyalty > 0;

    public CardInstance(CardData data)
    {
        cardData = data;
        currentHP = data.maxHP;
        currentLoyalty = data.baseLoyalty;
    }

    // Сбрасываем верность к базовому значению в начале хода
    public void ResetLoyalty()
    {
        currentLoyalty = cardData.baseLoyalty;
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
    }

    public void ModifyLoyalty(int delta)
    {
        currentLoyalty += delta;
    }
}
