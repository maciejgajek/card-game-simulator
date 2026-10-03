public class DrawEffect : CardEffect
{
    private int count = 1;

    public override void ResolveEffect(GameState state, PlayerState caster, ActionTarget target)
    {
        caster.DrawCards(count);
    }
}
