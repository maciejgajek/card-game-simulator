public class GraveyardRecallEffect : CardEffect
{
    public override void ResolveEffect(GameState state, PlayerState caster, ActionTarget target)
    {
        if (caster.Graveyard.Count == 0) return;
        if (caster.Hand.IsFull)          return;

        int index = state.Rng.Next(caster.Graveyard.Count);
        CardInstance picked = caster.Graveyard.Cards[index];

        caster.Graveyard.Remove(picked);
        caster.Hand.AddCard(picked);
    }
}
