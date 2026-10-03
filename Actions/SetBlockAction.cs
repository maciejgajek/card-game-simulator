public class SetBlockAction : GameAction
{
    public int  CreatureBoardIndex;
    public bool ShouldBlock;

    public SetBlockAction(int creatureBoardIndex, bool shouldBlock)
    {
        CreatureBoardIndex = creatureBoardIndex;
        ShouldBlock        = shouldBlock;
    }

    public override bool IsLegal(GameState state)
    {
        if (state.Phase != GamePhase.PlayerTurn && state.Phase != GamePhase.EnemyTurn) return false;
        if (state.IsTerminal) return false;

        if (CreatureBoardIndex < 0) return false;
        if (CreatureBoardIndex >= state.Active.BoardCreatures.Count) return false;

        CardInstance creature = state.Active.BoardCreatures[CreatureBoardIndex];
        if (creature == null) return false;
        if (creature.Data is not CreatureData cd) return false;
        if (!cd.CanBlock) return false;

        if (creature.HasAttackedThisTurn)    return false;
        if (creature.HasUsedAbilityThisTurn) return false;

        if (creature.IsBlocking == ShouldBlock) return false;

        return true;
    }

    public override void Apply(GameState state)
    {
        state.Active.BoardCreatures[CreatureBoardIndex].IsBlocking = ShouldBlock;
    }
}
