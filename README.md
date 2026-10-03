# Card Game Simulator

Unity-independent simulation engine for a turn-based card game, built in C#. Supports headless tournaments between pluggable AI agents, used to test and evaluate them to help improve game balance.

This is the extracted simulation core of a larger Unity card game kept intentionally free of any Unity dependency so it can run as a plain .NET console app for testing and AI research.

## Structure

- **Core (`Core/`)** — `GameState`, `PlayerState`, `GameRng`. Pure data + rules, no agents, no view.
- **Rules (`Actions/`, `Effects/`, `Mechanics/`, `Passives/`, `Auras/`)** — individual game actions, card effects, triggered abilities. Each action is a self-contained `GameAction` with `IsLegal(state)` and `Apply(state)` methods.
- **Agents (`Agents/`)** — AI implementations behind a shared `IAgent` interface:
  - `MctsAgent` — Monte Carlo Tree Search with configurable iteration budget and time cap. Uses `UtilityAgent` for rollout policy.
  - `UtilityAgent` / `UtilitierAgent` — one-ply heuristic agents with different scoring weights.
  - `RandomAgent` — baseline.

## Agents

| Agent | Strategy | Notes |
|-------|----------|-------|
| `RandomAgent` | Uniform random legal action | Baseline |
| `UtilityAgent` | One-ply heuristic: scores every legal action's resulting state | Fast, decent play |
| `UtilitierAgent` | Variant with different scoring weights | Used as self-play opponent for Utility |
| `MctsAgent` | UCT-based tree search with configurable iterations and time cap | Strongest; beats Utility ~66% over 50 games |

## Tuning

Open `Program.cs`:
- `TournamentCount` — number of matches
- `BaseSeed` — set to any non-zero value for a reproducible run
- `iterationsPerMove` / `maxThinkingMs` on `MctsAgent` — the time/strength trade-off

## Status

Core simulation and the four agents listed above are complete and tested. Content (cards, equipment, special abilities) is intentionally minimal in this standalone repo — the full card catalogue lives in the parent Unity project.

## License

MIT. See [LICENSE](LICENSE).
