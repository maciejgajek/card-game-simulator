public class EndTurnAction : GameAction
{
    public int PlayerIndex { get; }

    public EndTurnAction(int playerIndex)
    {
        PlayerIndex = playerIndex;
    }

    public override bool IsLegal(GameState state)
    {
        if (state.Phase != GamePhase.PlayerTurn && state.Phase != GamePhase.EnemyTurn) return false;
        if (state.ActivePlayerIndex != PlayerIndex) return false;
        if (state.IsTerminal) return false;
        return true;
    }

    public override void Apply(GameState state) => state.EndTurn();
}
