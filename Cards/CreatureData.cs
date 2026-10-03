using System.Collections.Generic;
public class CreatureData : CardData
{
    public int  Attack    { get; init; }
    public int  MaxHealth { get; init; }
    public bool CanBlock  { get; init; }

    public IReadOnlyList<CardEffect>       Effects  { get; init; } = new List<CardEffect>();
    public IReadOnlyList<Aura>             AuraList { get; init; } = new List<Aura>();
    public IReadOnlyList<CreaturePassive>  Passives { get; init; } = new List<CreaturePassive>();

    public override CardType                       Type        => CardType.Creature;
    public override IReadOnlyList<CardEffect>      PlayEffects => Effects;
    public override IReadOnlyList<Aura>            Auras       => AuraList;
}
