public class IceShardEffect : CardEffect
{
    private IceShardConfig config;

    public override void ResolveEffect(GameState state, PlayerState caster, ActionTarget target)
    {
        if (config == null) return;
        IceShardMechanic.Cast(state, caster, config.Damage);
    }
}
