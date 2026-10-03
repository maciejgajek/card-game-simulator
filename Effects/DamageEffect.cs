public class DamageEffect : CardEffect
{
    private int amount = 3;

    public override void ResolveEffect(GameState state, PlayerState caster, ActionTarget target)
    {
        if (target.IsNone || !target.PlayerIndex.HasValue) return;

        state.RaiseTargetedDamageCast(caster, target, amount);

        PlayerState targetPlayer = state.Players[target.PlayerIndex.Value];

        if (target.IsCreature)
            DamageCreature(targetPlayer, target.CreatureIndex.Value);
        else if (target.IsPlayer)
            targetPlayer.TakeDamage(amount);
    }

    private void DamageCreature(PlayerState owner, int creatureIndex)
    {
        if (creatureIndex < 0 || creatureIndex >= owner.BoardCreatures.Count) return;

        CardInstance creature = owner.BoardCreatures[creatureIndex];
        if (creature == null) return;

        creature.CurrentHealth -= amount;
        if (creature.CurrentHealth <= 0)
            owner.RemoveCreatureFromBoard(creature);
    }
}
