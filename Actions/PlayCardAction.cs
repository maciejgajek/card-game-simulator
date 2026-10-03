public class PlayCardAction : GameAction
{
    public CardInstance Card;
    public ActionTarget Target;

    public PlayCardAction(CardInstance card, ActionTarget target)
    {
        Card   = card;
        Target = target;
    }

    public PlayCardAction(CardInstance card, int? targetPlayerIndex, int? targetCreatureIndex, int? targetBuildingIndex = null)
        : this(card, new ActionTarget(targetPlayerIndex, targetCreatureIndex, targetBuildingIndex)) { }

    public override bool IsLegal(GameState state)
    {
        if (state.Phase != GamePhase.PlayerTurn && state.Phase != GamePhase.EnemyTurn) return false;
        if (state.IsTerminal) return false;

        if (Card == null) return false;
        if (!state.Active.Hand.Cards.Contains(Card)) return false;
        if (state.Active.Mana < Card.Data.ManaCost) return false;

        switch (Card.Data.Type)
        {
            case CardType.Spell:
                return IsValidSpellTarget(state);

            case CardType.Creature:
            case CardType.Building:
                return Target.IsNone || (Target.IsPlayer && Target.PlayerIndex == state.ActivePlayerIndex);

            default:
                return false;
        }
    }

    private bool IsValidSpellTarget(GameState state)
    {
        if (Card.Data is not SpellData spell) return false;
        SpellTargetType allowed = spell.TargetType;

        if (allowed == SpellTargetType.None) return Target.IsNone;

        if (Target.IsNone) return false;
        if (!Target.PlayerIndex.HasValue) return false;
        int targetPlayer  = Target.PlayerIndex.Value;
        int activePlayer  = state.ActivePlayerIndex;
        int enemyPlayer   = 1 - activePlayer;

        if (targetPlayer < 0 || targetPlayer > 1) return false;
        if (Target.IsCreature)
        {
            int i = Target.CreatureIndex.Value;
            if (i < 0 || i >= state.Players[targetPlayer].BoardCreatures.Count) return false;
        }
        if (Target.IsBuilding)
        {
            int i = Target.BuildingIndex.Value;
            if (i < 0 || i >= state.Players[targetPlayer].BoardBuildings.Count) return false;
        }

        bool ownerIsCaster = targetPlayer == activePlayer;
        bool ownerIsEnemy  = targetPlayer == enemyPlayer;

        return allowed switch
        {
            SpellTargetType.OwnPlayer             => Target.IsPlayer   &&  ownerIsCaster,
            SpellTargetType.EnemyPlayer           => Target.IsPlayer   &&  ownerIsEnemy,
            SpellTargetType.AnyPlayer             => Target.IsPlayer,
            SpellTargetType.OwnCreature           => Target.IsCreature &&  ownerIsCaster,
            SpellTargetType.EnemyCreature         => Target.IsCreature &&  ownerIsEnemy,
            SpellTargetType.AnyCreature           => Target.IsCreature,
            SpellTargetType.OwnCreatureOrPlayer   => ownerIsCaster && (Target.IsCreature || Target.IsPlayer),
            SpellTargetType.EnemyCreatureOrPlayer => ownerIsEnemy  && (Target.IsCreature || Target.IsPlayer),
            SpellTargetType.Any                   => true,
            _                                     => false
        };
    }

    public override void Apply(GameState state)
    {
        state.PlayCard(Card, Target);
    }
}
