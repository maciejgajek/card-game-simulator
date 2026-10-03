using System.Collections.Generic;
public class MageData : CreatureData
{
    private int maxMana;
    private int manaRegen;
    private List<MageAbility> abilities = new List<MageAbility>();

    public int MaxMana   => maxMana;
    public int ManaRegen => manaRegen;

    public IReadOnlyList<MageAbility> Abilities => abilities;
}
