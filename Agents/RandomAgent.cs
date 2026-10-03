using System.Collections.Generic;

// Baseline agent — uniform random from legal actions. Uses state.Rng so choices
// are reproducible from a seed.
public class RandomAgent : IAgent
{
    public GameAction ChooseAction(GameState s)
    {
        List<GameAction> legal = s.GetLegalActions();
        if (legal.Count == 0) return null;
        return legal[s.Rng.Next(legal.Count)];
    }
}
