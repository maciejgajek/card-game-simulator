using System.Collections.Generic;

public static class Program
{
    public static void Main()
    {
        var deck = BuildSimpleDeck();

        var harness = new Harness(
            new MctsAgent(GameState.PlayerIndex, iterationsPerMove: 500, maxThinkingMs: 1000),
            new UtilityAgent(GameState.EnemyIndex))
        {
            PlayerDeck       = deck,
            EnemyDeck        = deck,
            TournamentCount  = 50,
            BaseSeed         = 42
        };

        harness.RunTournament();
    }

    private static List<CardData> BuildSimpleDeck()
    {
        return new List<CardData>
        {
            NewCreature("Peasant",     cost: 1, atk: 2, hp: 2),
            NewCreature("Militia",     cost: 2, atk: 2, hp: 3, canBlock: true),
            NewCreature("Archer",      cost: 2, atk: 3, hp: 1),
            NewCreature("Footman",     cost: 3, atk: 3, hp: 3, canBlock: true),
            NewCreature("Knight",      cost: 4, atk: 4, hp: 4, canBlock: true),
            NewCreature("Guard",       cost: 3, atk: 2, hp: 5, canBlock: true),
            NewCreature("Berserker",   cost: 3, atk: 5, hp: 2),
            NewCreature("Champion",    cost: 5, atk: 5, hp: 5, canBlock: true),
            NewCreature("Hero",        cost: 6, atk: 6, hp: 6, canBlock: true),
            NewCreature("Dragon",      cost: 8, atk: 8, hp: 8)
        };
    }

    private static CreatureData NewCreature(string name, int cost, int atk, int hp, bool canBlock = false) =>
        new CreatureData {
            CardName  = name,
            ManaCost  = cost,
            Attack    = atk,
            MaxHealth = hp,
            CanBlock  = canBlock
        };
}
