public abstract class WarriorAbility
{
    private string abilityName;
    private string description;
    private int    rageCost;
    private bool triggerAttackAnimation;

    public string AbilityName            => abilityName;
    public string Description            => description;
    public int    RageCost               => rageCost;
    public bool   TriggerAttackAnimation => triggerAttackAnimation;

    public abstract void Execute(GameState state, WarriorInstance warrior, PlayerState owner, PlayerState enemy);
}
