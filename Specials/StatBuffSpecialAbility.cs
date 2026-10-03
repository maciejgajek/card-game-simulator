public class StatBuffSpecialAbility : SpecialAbility
{
    private int wisdomBonus = 1;
    private int charismaBonus = 1;

    public int WisdomBonus   => wisdomBonus;
    public int CharismaBonus => charismaBonus;

    public override void Activate(GameState state, PlayerState caster)
    {
        caster.Wisdom   += wisdomBonus;
        caster.Charisma += charismaBonus;
    }
}
