public static class IceShardMechanic
{
    public static void Cast(GameState state, PlayerState caster, int damage, CardInstance source = null)
    {
        if (state == null || caster == null || damage <= 0) return;

        PlayerState opponent = state.Players[1 - IndexOf(state, caster)];

        ActionTarget target = PickRandomEnemyTarget(state, opponent);
        if (target.IsNone) return;

        caster.IceShardsCastTotal++;
        caster.IceShardsCastThisTurn++;

        state.RaiseIceShardCast(caster, source, target, damage);

        ApplyDamage(state, target, damage);
    }

    private static ActionTarget PickRandomEnemyTarget(GameState state, PlayerState opponent)
    {
        int oppIdx = IndexOf(state, opponent);
        int creatureCount = opponent.BoardCreatures.Count;
        int totalTargets  = creatureCount + 1;

        int pick = state.Rng.Next(totalTargets);
        if (pick < creatureCount)
            return ActionTarget.Creature(oppIdx, pick);
        return ActionTarget.Player(oppIdx);
    }

    private static void ApplyDamage(GameState state, ActionTarget target, int amount)
    {
        if (!target.PlayerIndex.HasValue) return;
        PlayerState owner = state.Players[target.PlayerIndex.Value];

        if (target.IsCreature)
        {
            int i = target.CreatureIndex.Value;
            if (i < 0 || i >= owner.BoardCreatures.Count) return;
            CardInstance creature = owner.BoardCreatures[i];
            if (creature == null) return;

            creature.CurrentHealth -= amount;
            if (creature.CurrentHealth <= 0)
                owner.RemoveCreatureFromBoard(creature);
        }
        else if (target.IsPlayer)
        {
            owner.TakeDamage(amount);
        }
    }

    private static int IndexOf(GameState state, PlayerState p)
        => state.Players[0] == p ? 0 : 1;
}
