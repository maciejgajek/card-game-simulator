using System.Collections.Generic;
public class SpellData : CardData
{
    private List<CardEffect> effects = new List<CardEffect>();
    private SpellTargetType targetType = SpellTargetType.EnemyPlayer;

    public override CardType                  Type        => CardType.Spell;
    public override IReadOnlyList<CardEffect> PlayEffects => effects;

    public SpellTargetType TargetType => targetType;
}
