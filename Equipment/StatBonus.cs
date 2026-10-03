using System;

[Serializable]
public struct StatBonus
{
    public StatType StatType;
    public int      Amount;

    public StatBonus(StatType statType, int amount)
    {
        StatType = statType;
        Amount   = amount;
    }
}
