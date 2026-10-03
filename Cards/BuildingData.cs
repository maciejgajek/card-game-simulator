using System.Collections.Generic;
public class BuildingData : CardData
{
    private List<CardEffect> effects = new List<CardEffect>();
    private List<Aura> auras = new List<Aura>();
    private List<BuildingPassive> passives = new List<BuildingPassive>();

    public override CardType                       Type        => CardType.Building;
    public override IReadOnlyList<CardEffect>      PlayEffects => effects;
    public override IReadOnlyList<Aura>            Auras       => auras;
    public          IReadOnlyList<BuildingPassive> Passives    => passives;
}
