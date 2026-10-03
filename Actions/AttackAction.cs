public class AttackAction : GameAction
{
    public int  AttackerBoardIndex;
    public int? TargetCreatureIndex;

    public AttackAction(int attackerBoardIndex, int? targetCreatureIndex)
    {
        AttackerBoardIndex  = attackerBoardIndex;
        TargetCreatureIndex = targetCreatureIndex;
    }

    public override bool IsLegal(GameState state)
    {
        if (state.Phase != GamePhase.PlayerTurn && state.Phase != GamePhase.EnemyTurn) return false;
        if (state.IsTerminal) return false;

        if (AttackerBoardIndex < 0) return false;
        if (AttackerBoardIndex >= state.Active.BoardCreatures.Count) return false;

        CardInstance attacker = state.Active.BoardCreatures[AttackerBoardIndex];
        if (attacker == null) return false;
        if (attacker.Data is not CreatureData) return false;

        if (attacker.HasAttackedThisTurn) return false;
        if (attacker.IsBlocking) return false;

        bool opponentHasBlockers = HasAnyBlocker(state.Opponent);

        if (opponentHasBlockers)
        {
            if (!TargetCreatureIndex.HasValue) return false;
            int idx = TargetCreatureIndex.Value;
            if (idx < 0 || idx >= state.Opponent.BoardCreatures.Count) return false;
            CardInstance target = state.Opponent.BoardCreatures[idx];
            if (target == null || !target.IsBlocking) return false;
        }
        else
        {
            if (TargetCreatureIndex.HasValue) return false;
        }

        return true;
    }

    public override void Apply(GameState state)
    {
        CardInstance attacker     = state.Active.BoardCreatures[AttackerBoardIndex];
        CreatureData attackerData = (CreatureData)attacker.Data;

        if (TargetCreatureIndex.HasValue)
        {
            CardInstance target     = state.Opponent.BoardCreatures[TargetCreatureIndex.Value];
            CreatureData targetData = (CreatureData)target.Data;

            target.CurrentHealth   -= attackerData.Attack;
            attacker.CurrentHealth -= targetData.Attack;

            if (target.CurrentHealth   <= 0) state.Opponent.RemoveCreatureFromBoard(target);
            if (attacker.CurrentHealth <= 0) state.Active.RemoveCreatureFromBoard(attacker);
        }
        else
        {
            state.Opponent.TakeDamage(attackerData.Attack);
        }

        attacker.HasAttackedThisTurn = true;

        if (attacker is WarriorInstance warrior)
            warrior.AddRage(attackerData.Attack);

        if (attacker.CurrentHealth > 0 && attacker.Data is CreatureData attackerCreatureData)
        {
            foreach (CreaturePassive passive in attackerCreatureData.Passives)
                passive?.OnAttack(state, state.Active, state.Opponent, attacker);
        }
    }

    private static bool HasAnyBlocker(PlayerState player)
    {
        foreach (CardInstance c in player.BoardCreatures)
            if (c != null && c.IsBlocking) return true;
        return false;
    }
}
