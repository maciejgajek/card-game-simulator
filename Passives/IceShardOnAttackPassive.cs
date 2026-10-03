public class IceShardOnAttackPassive : CreaturePassive
{
    private IceShardConfig config;

    public override void OnAttack(GameState state, PlayerState owner, PlayerState enemy, CardInstance source)
    {
        if (config == null) return;
        IceShardMechanic.Cast(state, owner, config.Damage, source);
    }
}
