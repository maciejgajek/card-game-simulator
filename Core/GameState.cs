using System;
using System.Collections.Generic;

public class GameState
{
    public const int PlayerIndex = 0;
    public const int EnemyIndex  = 1;

    public PlayerState[] Players { get; }
    public int ActivePlayerIndex;
    public int TurnNumber;
    public GamePhase Phase;

    public GameRng Rng { get; private set; }

    public event Action<PlayerState, CardInstance, ActionTarget, int> OnIceShardCast;

    public void RaiseIceShardCast(PlayerState caster, CardInstance source, ActionTarget target, int damage)
        => OnIceShardCast?.Invoke(caster, source, target, damage);

    public event Action<PlayerState, ActionTarget, int> OnTargetedDamageCast;

    public void RaiseTargetedDamageCast(PlayerState caster, ActionTarget target, int damage)
        => OnTargetedDamageCast?.Invoke(caster, target, damage);

    public PlayerState Active   => Players[ActivePlayerIndex];
    public PlayerState Opponent => Players[1 - ActivePlayerIndex];
    public PlayerState Player   => Players[PlayerIndex];
    public PlayerState Enemy    => Players[EnemyIndex];

    public bool IsTerminal => Players[0].Health <= 0 || Players[1].Health <= 0;

    public int? Winner =>
        !IsTerminal            ? (int?)null :
         Players[0].Health > 0 ? 0          :
                                 1;

    public GameState(PlayerState player, PlayerState enemy, int? seed = null)
    {
        Players           = new[] { player, enemy };
        ActivePlayerIndex = PlayerIndex;
        TurnNumber        = 0;
        Phase             = GamePhase.Preparation;
        Rng               = new GameRng(seed ?? System.Environment.TickCount);
    }

    public void PlayCard(CardInstance card, ActionTarget target)
    {
        PlayerState caster = Active;

        caster.SpendMana(card.Data.ManaCost);

        caster.Hand.RemoveCard(card);

        foreach (CardEffect effect in card.Data.PlayEffects)
            if (effect != null) effect.ResolveEffect(this, caster, target);

        foreach (Aura aura in card.Data.Auras)
            if (aura != null) aura.OnEnterBoard(caster);

        switch (card.Data.Type)
        {
            case CardType.Spell:    caster.Graveyard.Add(card);        break;
            case CardType.Creature: caster.AddCreatureToBoard(card);   break;
            case CardType.Building: caster.AddBuildingToBoard(card);   break;
        }
    }

    public void EndTurn()
    {
        PlayerState active = Active;

        active.PruneDestroyedBoardCards();

        foreach (CardInstance creature in active.BoardCreatures)
        {
            if (creature is MageInstance mage) mage.OnTurnEnd();

            if (creature.Data is CreatureData creatureData)
            {
                foreach (CreaturePassive passive in creatureData.Passives)
                    passive?.OnTurnEnd(active);
            }
        }

        foreach (CardInstance building in active.BoardBuildings)
        {
            if (building.Data is BuildingData buildingData)
            {
                foreach (BuildingPassive passive in buildingData.Passives)
                    passive?.OnTurnEnd(active);
            }
        }

        if (active.Mana >= active.MaxMana) active.ResetMana();
    }

    public void StartTurn()
    {
        PlayerState active = Active;

        active.PruneDestroyedBoardCards();

        active.IceShardsCastThisTurn = 0;

        foreach (CardInstance creature in active.BoardCreatures)
        {
            creature.HasAttackedThisTurn    = false;
            creature.HasUsedAbilityThisTurn = false;

            if (creature is MageInstance mage)     mage.OnTurnStart();
            if (creature is MinionInstance minion) minion.OnTurnStart();
        }

        foreach (CardInstance building in active.BoardBuildings)
        {
            if (building.Data is BuildingData buildingData)
            {
                foreach (BuildingPassive passive in buildingData.Passives)
                    passive?.OnTurnStart(active);
            }
        }
    }

    public List<GameAction> GetLegalActions()
    {
        var actions = new List<GameAction>();

        if (IsTerminal) return actions;

        var endTurn = new EndTurnAction(ActivePlayerIndex);
        if (endTurn.IsLegal(this)) actions.Add(endTurn);

        foreach (CardInstance card in Active.Hand.Cards)
            EnumeratePlayActions(card, actions);

        List<int> blockerIndices = CollectBlockerIndices(Opponent);
        for (int i = 0; i < Active.BoardCreatures.Count; i++)
        {
            if (blockerIndices.Count == 0)
            {
                var attack = new AttackAction(i, null);
                if (attack.IsLegal(this)) actions.Add(attack);
            }
            else
            {
                foreach (int blockerIdx in blockerIndices)
                {
                    var attack = new AttackAction(i, blockerIdx);
                    if (attack.IsLegal(this)) actions.Add(attack);
                }
            }
        }

        for (int i = 0; i < Active.BoardCreatures.Count; i++)
        {
            CardInstance creature = Active.BoardCreatures[i];
            int abilityCount = creature switch
            {
                MageInstance mage       => mage.MageData.Abilities.Count,
                WarriorInstance warrior => warrior.WarriorData.Abilities.Count,
                MinionInstance minion   => minion.MinionData.Abilities.Count,
                _                       => 0
            };

            for (int j = 0; j < abilityCount; j++)
            {
                var useAbility = new UseAbilityAction(i, j);
                if (useAbility.IsLegal(this)) actions.Add(useAbility);
            }
        }

        for (int i = 0; i < Active.BoardCreatures.Count; i++)
        {
            CardInstance c = Active.BoardCreatures[i];
            if (c == null || c.IsBlocking) continue;

            var blockOn = new SetBlockAction(i, true);
            if (blockOn.IsLegal(this)) actions.Add(blockOn);
        }

        var special = new UseSpecialAction(ActivePlayerIndex);
        if (special.IsLegal(this)) actions.Add(special);

        return actions;
    }

    private void EnumeratePlayActions(CardInstance card, List<GameAction> actions)
    {
        if (card.Data.Type != CardType.Spell)
        {
            var play = new PlayCardAction(card, ActionTarget.None);
            if (play.IsLegal(this)) actions.Add(play);
            return;
        }

        SpellData spell = (SpellData)card.Data;
        int self  = ActivePlayerIndex;
        int enemy = 1 - ActivePlayerIndex;

        switch (spell.TargetType)
        {
            case SpellTargetType.None:
                TryAdd(actions, new PlayCardAction(card, ActionTarget.None));
                break;

            case SpellTargetType.OwnPlayer:
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(self)));
                break;

            case SpellTargetType.EnemyPlayer:
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(enemy)));
                break;

            case SpellTargetType.AnyPlayer:
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(self)));
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(enemy)));
                break;

            case SpellTargetType.OwnCreature:
                EnumerateCreatureTargets(card, self, actions);
                break;

            case SpellTargetType.EnemyCreature:
                EnumerateCreatureTargets(card, enemy, actions);
                break;

            case SpellTargetType.AnyCreature:
                EnumerateCreatureTargets(card, self,  actions);
                EnumerateCreatureTargets(card, enemy, actions);
                break;

            case SpellTargetType.OwnCreatureOrPlayer:
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(self)));
                EnumerateCreatureTargets(card, self, actions);
                break;

            case SpellTargetType.EnemyCreatureOrPlayer:
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(enemy)));
                EnumerateCreatureTargets(card, enemy, actions);
                break;

            case SpellTargetType.Any:
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(self)));
                TryAdd(actions, new PlayCardAction(card, ActionTarget.Player(enemy)));
                EnumerateCreatureTargets(card, self,  actions);
                EnumerateCreatureTargets(card, enemy, actions);
                EnumerateBuildingTargets(card, self,  actions);
                EnumerateBuildingTargets(card, enemy, actions);
                break;
        }
    }

    private void EnumerateCreatureTargets(CardInstance card, int playerIdx, List<GameAction> actions)
    {
        int count = Players[playerIdx].BoardCreatures.Count;
        for (int i = 0; i < count; i++)
            TryAdd(actions, new PlayCardAction(card, ActionTarget.Creature(playerIdx, i)));
    }

    private void EnumerateBuildingTargets(CardInstance card, int playerIdx, List<GameAction> actions)
    {
        int count = Players[playerIdx].BoardBuildings.Count;
        for (int i = 0; i < count; i++)
            TryAdd(actions, new PlayCardAction(card, ActionTarget.Building(playerIdx, i)));
    }

    private void TryAdd(List<GameAction> actions, PlayCardAction play)
    {
        if (play.IsLegal(this)) actions.Add(play);
    }

    private static List<int> CollectBlockerIndices(PlayerState player)
    {
        var result = new List<int>();
        for (int i = 0; i < player.BoardCreatures.Count; i++)
        {
            CardInstance c = player.BoardCreatures[i];
            if (c != null && c.IsBlocking) result.Add(i);
        }
        return result;
    }

    public GameState Clone()
    {
        GameState copy = new GameState(Players[0].Clone(), Players[1].Clone())
        {
            ActivePlayerIndex = ActivePlayerIndex,
            TurnNumber        = TurnNumber,
            Phase             = Phase
        };

        // Continue the same random sequence rather than starting a fresh one.
        copy.Rng = Rng.Clone();

        return copy;
    }
}
