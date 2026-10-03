public abstract class SpecialAbility
{
    private string specialName;
    private string description;

    public string SpecialName => specialName;
    public string Description => description;

    public abstract void Activate(GameState state, PlayerState caster);
}
