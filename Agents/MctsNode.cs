using System.Collections.Generic;

public class MctsNode
{
    public GameState State { get; }
    public MctsNode Parent { get; }
    public GameAction ActionTaken { get; }
    public int PlayerJustMoved { get; }

    public List<GameAction> UntriedActions { get; }
    public List<MctsNode> Children { get; } = new List<MctsNode>();

    public int Visits { get; private set; }
    public float TotalReward { get; private set; }

    public bool IsFullyExpanded => UntriedActions.Count == 0;
    public bool IsTerminal => State.IsTerminal;

    public MctsNode(GameState state, MctsNode parent, GameAction actionTaken, int playerJustMoved)
    {
        State           = state;
        Parent          = parent;
        ActionTaken     = actionTaken;
        PlayerJustMoved = playerJustMoved;

        UntriedActions = state.IsTerminal ? new List<GameAction>() : state.GetLegalActions();
    }

    public void Update(int winner)
    {
        Visits++;
        if (winner == PlayerJustMoved) TotalReward += 1f;
    }

    public float GetUcb(float c)
    {
        if (Visits == 0) return float.PositiveInfinity;
        return (TotalReward / Visits) + c * (float)System.Math.Sqrt(System.Math.Log(Parent.Visits) / Visits);
    }

    public void AddChild(MctsNode child, GameAction actionTaken)
    {
        Children.Add(child);
        UntriedActions.Remove(actionTaken);
    }
}
