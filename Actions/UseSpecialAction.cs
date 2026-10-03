public class UseSpecialAction : GameAction
{
    public int PlayerIndex { get; }

    public UseSpecialAction(int playerIndex)
    {
        PlayerIndex = playerIndex;
    }

    public override bool IsLegal(GameState state)
    {
        if (state.Phase != GamePhase.PlayerTurn && state.Phase != GamePhase.EnemyTurn) return false;
        if (state.IsTerminal) return false;
        if (state.ActivePlayerIndex != PlayerIndex) return false;
        if (!state.Active.IsSpecialFull) return false;
        return true;
    }

    public override void Apply(GameState state)
    {
        PlayerState caster = state.Active;
        caster.SpecialAbility?.Activate(state, caster);
        caster.ResetSpecial();
    }
}
