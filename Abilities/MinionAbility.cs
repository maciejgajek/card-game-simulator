public abstract class MinionAbility
{
    private string abilityName;
    private string description;
    private int    turnCooldown;

    public string AbilityName  => abilityName;
    public string Description  => description;
    public int    TurnCooldown => turnCooldown;

    public abstract void Execute(GameState state, MinionInstance minion, PlayerState owner, PlayerState enemy);
}
