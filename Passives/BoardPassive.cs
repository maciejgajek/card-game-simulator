public abstract class BoardPassive
{
    private string passiveName;
    private string description;

    public string PassiveName => passiveName;
    public string Description => description;

    public virtual void OnTurnEnd(PlayerState owner) { }
    public virtual void OnTurnStart(PlayerState owner) { }

    public virtual void OnAttack(GameState state, PlayerState owner, PlayerState enemy, CardInstance source) { }
}
