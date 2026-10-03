public abstract class MageAbility
{
    private string abilityName;
    private string description;
    private int    manaCost;
    private int    turnCooldown;

    public string AbilityName  => abilityName;
    public string Description  => description;
    public int    ManaCost     => manaCost;
    public int    TurnCooldown => turnCooldown;

    public abstract void Execute(GameState state, MageInstance mage, PlayerState owner, PlayerState enemy);
}
