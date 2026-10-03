public class IceShardMageAbility : MageAbility
{
    private IceShardConfig config;

    public override void Execute(GameState state, MageInstance mage, PlayerState owner, PlayerState enemy)
    {
        if (config == null) return;
        IceShardMechanic.Cast(state, owner, config.Damage, mage);
    }
}
