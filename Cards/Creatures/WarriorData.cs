using System.Collections.Generic;
public class WarriorData : CreatureData
{
    private int maxRage;
    private List<WarriorAbility> abilities = new List<WarriorAbility>();

    public int MaxRage => maxRage;

    public IReadOnlyList<WarriorAbility> Abilities => abilities;
}
