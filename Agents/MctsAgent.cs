using System.Collections.Generic;
public class MctsAgent : IAgent
{
    public int   IterationsPerBranch = 30;
    public int   MinIterations       = 100;
    public int   IterationsPerMove   = 1000;
    public int   MaxThinkingMs       = 1000;

    public float ExplorationConstant = 1.41f;
    public int   RolloutDepthCap     = 200;

    public bool  EarlyStopping        = true;
    public int   EarlyStopCheckEvery  = 100;

    public bool  UseUtilityRollouts = true;
    public float RolloutExploration = 0.2f;

    private readonly int myPlayerIndex;

    private readonly GameRng rolloutRng;

    private readonly UtilityAgent playerRolloutPolicy;
    private readonly UtilityAgent enemyRolloutPolicy;

    public MctsAgent(int playerIndex, int rolloutSeed = 0)
    {
        myPlayerIndex       = playerIndex;
        rolloutRng          = new GameRng(rolloutSeed != 0 ? rolloutSeed : System.Environment.TickCount);
        playerRolloutPolicy = new UtilityAgent(GameState.PlayerIndex);
        enemyRolloutPolicy  = new UtilityAgent(GameState.EnemyIndex);
    }

    public MctsAgent(int playerIndex, int iterationsPerMove, int maxThinkingMs, int rolloutSeed = 0)
        : this(playerIndex, rolloutSeed)
    {
        IterationsPerMove = iterationsPerMove;
        MaxThinkingMs     = maxThinkingMs;
    }

    public GameAction ChooseAction(GameState state)
    {
        GameState clone = state.Clone();
        MctsNode root = new MctsNode(clone, null, null, 1 - clone.ActivePlayerIndex);

        int branches = root.UntriedActions.Count;
        int targetIterations = Math.Clamp(branches * IterationsPerBranch, MinIterations, IterationsPerMove);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < targetIterations; i++)
        {
            if (MaxThinkingMs > 0 && sw.ElapsedMilliseconds >= MaxThinkingMs) break;

            MctsNode leaf = Select(root);
            if (!leaf.IsTerminal && !leaf.IsFullyExpanded)
                leaf = Expand(leaf);

            int winner = Rollout(leaf.State.Clone());
            Backpropagate(leaf, winner);

            if (EarlyStopping && i + 1 >= MinIterations && (i + 1) % EarlyStopCheckEvery == 0)
            {
                if (LeaderDominates(root)) break;
            }
        }

        int maxVisits = -1;
        MctsNode bestChild = null;
        foreach (MctsNode child in root.Children)
        {
            if (child.Visits > maxVisits)
            {
                maxVisits = child.Visits;
                bestChild = child;
            }
        }

        return TranslateForRealState(bestChild?.ActionTaken, state);
    }

    private static GameAction TranslateForRealState(GameAction action, GameState realState)
    {
        if (action == null) return null;

        if (action is PlayCardAction play)
        {
            foreach (CardInstance real in realState.Active.Hand.Cards)
            {
                if (real != null && real.Data == play.Card.Data)
                    return new PlayCardAction(real, play.Target);
            }
            return null;
        }

        return action;
    }

    private static bool LeaderDominates(MctsNode root)
    {
        int top    = 0;
        int second = 0;
        foreach (MctsNode child in root.Children)
        {
            int v = child.Visits;
            if      (v > top)    { second = top; top = v; }
            else if (v > second) { second = v; }
        }
        return top > 0 && top >= 2 * second;
    }

    private MctsNode Select(MctsNode root)
    {
        while (!root.State.IsTerminal && root.IsFullyExpanded)
        {
            float bestUcb = float.NegativeInfinity;
            MctsNode bestChild = null;
            foreach (MctsNode child in root.Children)
            {
                float ucb = child.GetUcb(ExplorationConstant);
                if (ucb > bestUcb)
                {
                    bestUcb = ucb;
                    bestChild = child;
                }
            }
            root = bestChild;
        }
        return root;
    }

    private MctsNode Expand(MctsNode node)
    {
        int index = rolloutRng.Next(node.UntriedActions.Count);
        GameAction action = node.UntriedActions[index];
        GameState clone = node.State.Clone();
        int playerJustMoved = clone.ActivePlayerIndex;
        ApplyActionInSearch(action, clone);
        MctsNode child = new MctsNode(clone, node, action, playerJustMoved);
        node.Children.Add(child);
        node.UntriedActions.Remove(action);
        return child;
    }

    private int Rollout(GameState state)
    {
        for (int i = 0; i < RolloutDepthCap; i++)
        {
            if (state.IsTerminal) break;

            GameAction pick;
            if (UseUtilityRollouts && rolloutRng.NextFloat() >= RolloutExploration)
            {
                IAgent policy = state.ActivePlayerIndex == GameState.PlayerIndex
                                ? playerRolloutPolicy
                                : enemyRolloutPolicy;
                pick = policy.ChooseAction(state);
                if (pick == null) break;
            }
            else
            {
                List<GameAction> legal = state.GetLegalActions();
                if (legal.Count == 0) break;
                pick = legal[rolloutRng.Next(legal.Count)];
            }

            ApplyActionInSearch(pick, state);
        }
        return state.Winner ?? -1;
    }

    private void Backpropagate(MctsNode leaf, int winner)
    {
        while (leaf != null)
        {
            leaf.Update(winner);
            leaf = leaf.Parent;
        }
    }

    private static void ApplyActionInSearch(GameAction action, GameState state)
    {
        action.Apply(state);
        if (action is EndTurnAction)
        {
            state.ActivePlayerIndex = 1 - state.ActivePlayerIndex;
            state.Phase = state.ActivePlayerIndex == 0 ? GamePhase.PlayerTurn : GamePhase.EnemyTurn;
            state.StartTurn();
            PlayerState activePlayer = state.Players[state.ActivePlayerIndex];
            activePlayer.IncreaseResources();
            activePlayer.IncreaseWisdom();
            activePlayer.GrantMana();
            activePlayer.IncreaseCharisma();
        }
    }
}
