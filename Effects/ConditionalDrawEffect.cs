public class ConditionalDrawEffect : CardEffect
{
    private int baseDraw = 1;
    private int boostedDraw = 3;

    public override void ResolveEffect(GameState state, PlayerState caster, ActionTarget target)
    {
        int amount = caster.IceShardsCastThisTurn > 0 ? boostedDraw : baseDraw;
        caster.DrawCards(amount);
    }
}
