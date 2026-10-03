using System;
using System.Collections.Generic;

public class Harness
{
    public List<CardData> PlayerDeck  = new();
    public List<CardData> EnemyDeck   = new();
    public int StartingHandSize = 3;
    public int TurnCap          = 100;
    public int ActionCap        = 100;
    public int TournamentCount  = 100;
    public int BaseSeed         = 0;

    private readonly IAgent playerAgent;
    private readonly IAgent enemyAgent;

    public Harness(IAgent player, IAgent enemy)
    {
        playerAgent = player;
        enemyAgent  = enemy;
    }

    private struct GameResult
    {
        public int Winner;
        public int Turns;
        public int PlayerHealth;
        public int EnemyHealth;
        public int Seed;
        public bool HitTurnCap;
    }

    public void RunTournament()
    {
        int playerWins = 0, enemyWins = 0, ties = 0, totalTurns = 0;
        int rootSeed = BaseSeed == 0 ? Environment.TickCount : BaseSeed;

        for (int i = 0; i < TournamentCount; i++)
        {
            GameResult r = RunGame(rootSeed + i);
            if      (r.Winner == 0) playerWins++;
            else if (r.Winner == 1) enemyWins++;
            else                    ties++;
            totalTurns += r.Turns;

            if (i < 5 || i == TournamentCount - 1)
                PrintGame(i + 1, r);
        }

        PrintSummary(playerWins, enemyWins, ties, totalTurns, rootSeed);
    }

    private GameResult RunGame(int seed)
    {
        var playerState = new PlayerState();
        var enemyState  = new PlayerState();
        var gs          = new GameState(playerState, enemyState, seed);

        playerState.Initialize();
        enemyState.Initialize();

        playerState.CreateDeck(PlayerDeck, gs.Rng);
        enemyState .CreateDeck(EnemyDeck , gs.Rng);

        playerState.DrawCards(StartingHandSize);
        enemyState .DrawCards(StartingHandSize);

        gs.ActivePlayerIndex = gs.Rng.Next(0, 2);
        gs.Phase = gs.ActivePlayerIndex == GameState.PlayerIndex ? GamePhase.PlayerTurn : GamePhase.EnemyTurn;

        int turnCount = 0;
        while (!gs.IsTerminal && turnCount < TurnCap)
        {
            IAgent current = gs.ActivePlayerIndex == GameState.PlayerIndex ? playerAgent : enemyAgent;
            PlayerState active = gs.Players[gs.ActivePlayerIndex];

            gs.StartTurn();
            active.IncreaseResources();
            active.IncreaseWisdom();
            active.GrantMana();
            active.IncreaseCharisma();

            int acts = 0;
            while (acts++ < ActionCap)
            {
                GameAction a = current.ChooseAction(gs);
                if (a == null)              break;
                if (a is EndTurnAction)     break;
                a.Apply(gs);
                if (gs.IsTerminal)          break;
            }

            gs.EndTurn();
            gs.ActivePlayerIndex = 1 - gs.ActivePlayerIndex;
            gs.Phase = gs.ActivePlayerIndex == GameState.PlayerIndex ? GamePhase.PlayerTurn : GamePhase.EnemyTurn;
            turnCount++;
        }

        int winner;
        if (gs.Winner.HasValue)                           winner = gs.Winner.Value;
        else if (playerState.Health > enemyState.Health)  winner = GameState.PlayerIndex;
        else if (enemyState.Health  > playerState.Health) winner = GameState.EnemyIndex;
        else                                              winner = -1;

        return new GameResult {
            Winner = winner, Turns = turnCount,
            PlayerHealth = playerState.Health, EnemyHealth = enemyState.Health,
            Seed = seed, HitTurnCap = turnCount >= TurnCap
        };
    }

    private static void PrintGame(int i, GameResult r)
    {
        string w = r.Winner switch { 0 => "Player", 1 => "Enemy", _ => "Tie" };
        string cap = r.HitTurnCap ? " [cap]" : "";
        Console.WriteLine($"  Match {i,3} seed={r.Seed}  winner={w,-6}  turns={r.Turns,3}  HP {r.PlayerHealth,3} / {r.EnemyHealth,3}{cap}");
    }

    private void PrintSummary(int p, int e, int t, int totalTurns, int root)
    {
        int n = TournamentCount;
        Console.WriteLine();
        Console.WriteLine($"=== Tournament summary (root seed {root}) ===");
        Console.WriteLine($"  Player wins: {p,4}  ({100f*p/n:F1}%)");
        Console.WriteLine($"  Enemy  wins: {e,4}  ({100f*e/n:F1}%)");
        Console.WriteLine($"  Ties       : {t,4}  ({100f*t/n:F1}%)");
        Console.WriteLine($"  Avg turns  : {(float)totalTurns/n:F1}");
    }
}