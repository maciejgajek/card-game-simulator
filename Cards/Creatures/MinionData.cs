using System.Collections.Generic;
public class MinionData : CreatureData
{
    private List<MinionAbility> abilities = new List<MinionAbility>();

    public List<MinionAbility> Abilities => abilities;
}
