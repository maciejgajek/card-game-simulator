public class HealEffect : CardEffect
{
    private int amount = 3;

    public override void ResolveEffect(GameState state, PlayerState caster, ActionTarget target)
    {
        if (target.IsNone || !target.PlayerIndex.HasValue) return;

        PlayerState targetPlayer = state.Players[target.PlayerIndex.Value];

        if (target.IsCreature)
            HealCreature(targetPlayer, target.CreatureIndex.Value);
        else if (target.IsPlayer)
            targetPlayer.Heal(amount);
    }

    private void HealCreature(PlayerState owner, int creatureIndex)
    {
        if (creatureIndex < 0 || creatureIndex >= owner.BoardCreatures.Count) return;

        CardInstance creature = owner.BoardCreatures[creatureIndex];
        if (creature == null) return;
        if (creature.Data is not CreatureData cd) return;

        creature.CurrentHealth = Math.Min(creature.CurrentHealth + amount, cd.MaxHealth);
    }
}
