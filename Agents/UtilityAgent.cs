using System;
using System.Collections.Generic;

public class UtilityAgent : IAgent
{
    private readonly int myPlayerIndex;

    public UtilityAgent(int playerIndex)
    {
        myPlayerIndex = playerIndex;
    }

    public GameAction ChooseAction(GameState state)
    {
        List<GameAction> legal = state.GetLegalActions();
        if (legal.Count == 0) return null;

        float bestScore = float.NegativeInfinity;
        GameAction best = null;

        foreach (GameAction action in legal)
        {
            GameState clone = state.Clone();
            action.Apply(clone);
            float score = Evaluate(clone);
            if (score > bestScore)
            {
                bestScore = score;
                best      = action;
            }
        }
        return best;
    }

    private float Evaluate(GameState state)
    {
        PlayerState me  = state.Players[myPlayerIndex];
        PlayerState opp = state.Players[1 - myPlayerIndex];
        if (opp.Health <= 0) return  10_000f;
        if (me.Health  <= 0) return -10_000f;
        float score = 0f;
        score -= opp.Health * 0.3f;
        float missing = me.Stamina - me.Health;
        score -= missing * missing * 0.02f;
        float oppMissing = opp.Stamina - opp.Health;
        score += oppMissing * oppMissing * 0.02f;
        score += BoardStrength(me)  *  0.7f;
        score -= BoardStrength(opp) *  0.7f;
        //if (me.Hand.Count > 1 || opp.Hand.Count <= 1) score += (me.Hand.Count - opp.Hand.Count) * 2f;
        // score += me.Hand.Count * 1f;
        score += (me.Hand.Count - opp.Hand.Count) * 2f;
        score += me.Mana * 0.1f;
        score += me.ManaPerTurn * 0.6f;
        score += (me.Charisma - opp.Charisma) * 0.5f;
        score += me.Special  * 0.05f;
        score -= opp.Special * 0.05f;
        //score -= CountCurrentToMaxHealth(me) * 0.5f;

        if (CountOwnAttack(me)      >  CountBlockingHealth(opp)) score += 1.0f;
        if (CountBlockingHealth(me) >= CountOwnAttack(opp))      score += 1.0f;
        if (CountBlockingHealth(opp) == 0 && CountAttackThreats(me) > 0) score += 5.0f;

        score -= CountBlockers(opp) * 2.0f;

        int oppThreats = CountAttackThreats(opp);
        score += Math.Min(CountBlockers(me), oppThreats) * 1.0f;

        score += CountBuildings(me)  *  0.3f;
        score -= CountBuildings(opp) *  0.3f;
        return score;
    }

    private static float BoardStrength(PlayerState p)
    {
        float total = 0f;
        foreach (CardInstance c in p.BoardCreatures)
        {
            if (c == null) continue;
            if (c.Data is CreatureData cd)
                total += cd.Attack + c.CurrentHealth;
        }
        return total;
    }

    private static int CountBlockers(PlayerState p)
    {
        int count = 0;
        foreach (CardInstance c in p.BoardCreatures)
            if (c != null && c.IsBlocking) count++;
        return count;
    }

    private static int CountAttackThreats(PlayerState p)
    {
        int count = 0;
        foreach (CardInstance c in p.BoardCreatures)
        {
            if (c == null) continue;
            if (c.IsBlocking) continue;
            if (c.Data is not CreatureData) continue;
            count++;
        }
        return count;
    }

    private static int CountOwnAttack(PlayerState p)
    {
        int count = 0;
        foreach (CardInstance c in p.BoardCreatures)
        {
            if (c == null) continue;
            if (c.IsBlocking) continue;
            if (c.Data is not CreatureData) continue;
            count += c.CurrentAttack;
        }
        return count;
    }

    private static int CountBlockingHealth(PlayerState p)
    {
        int count = 0;
        foreach (CardInstance c in p.BoardCreatures)
        {
            if (c == null) continue;
            if (!c.IsBlocking) continue;
            if (c.Data is not CreatureData) continue;
            count += c.CurrentHealth;
        }
        return count;
    }

    private static int CountBuildings(PlayerState p)
    {
        int count = 0;
        foreach (CardInstance c in p.BoardBuildings)
            if (c != null) count++;
        return count;
    }

    private static int CountCurrentToMaxHealth(PlayerState p)
    {
        int count = 0;
        foreach (CardInstance c in p.BoardCreatures)
        {
            if (c == null) continue;
            if (c.Data is not CreatureData cd) continue;
            count += c.CurrentHealth;
            count += cd.MaxHealth;
            if(c.CurrentHealth < cd.MaxHealth)
                count ++;
        }
        return count;
    }
}
