public class BlizzardEffect : CardEffect
{
    private IceShardConfig config;

    public override void ResolveEffect(GameState state, PlayerState caster, ActionTarget target)
    {
        if (config == null) return;
        int shots = caster.IceShardsCastTotal;
        for (int i = 0; i < shots; i++)
            IceShardMechanic.Cast(state, caster, config.Damage);
    }
}

