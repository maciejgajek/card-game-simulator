public abstract class GameAction
{
    public abstract bool IsLegal(GameState state);
    public abstract void Apply(GameState state);
}
