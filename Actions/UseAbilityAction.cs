public class UseAbilityAction : GameAction
{
    public int CreatureBoardIndex;
    public int AbilityIndex;

    public UseAbilityAction(int creatureBoardIndex, int abilityIndex)
    {
        CreatureBoardIndex = creatureBoardIndex;
        AbilityIndex = abilityIndex;
    }

    public override bool IsLegal(GameState state)
    {
        if (state.Phase != GamePhase.PlayerTurn && state.Phase != GamePhase.EnemyTurn) return false;
        if (state.IsTerminal) return false;

        if (CreatureBoardIndex < 0) return false;
        if (CreatureBoardIndex >= state.Active.BoardCreatures.Count) return false;

        CardInstance creature = state.Active.BoardCreatures[CreatureBoardIndex];
        if (creature == null) return false;

        // Blockers can't use abilities (same rule as attacking).
        if (creature.IsBlocking) return false;

        return creature switch
        {
            MageInstance mage       => mage.CanUseAbility(AbilityIndex),
            WarriorInstance warrior => warrior.CanUseAbility(AbilityIndex),
            MinionInstance minion   => minion.CanUseAbility(AbilityIndex),
            _                       => false
        };
    }

    public override void Apply(GameState state)
    {
        CardInstance creature = state.Active.BoardCreatures[CreatureBoardIndex];
        PlayerState  owner    = state.Active;
        PlayerState  enemy    = state.Opponent;

        switch (creature)
        {
            case MageInstance mage:
                mage.UseAbility(state, AbilityIndex, owner, enemy);
                // Mages: one action per turn — no normal attack after an ability.
                mage.HasAttackedThisTurn = true;
                break;

            case WarriorInstance warrior:
                warrior.UseAbility(state, AbilityIndex, owner, enemy);
                break;

            case MinionInstance minion:
                minion.UseAbility(state, AbilityIndex, owner, enemy);
                break;
        }

        creature.HasUsedAbilityThisTurn = true;
    }
}
