using System.Collections.Generic;

public class EquipmentData
{
    public string                   EquipmentName { get; init; } = "Unnamed Equipment";
    public string                   Description   { get; init; } = "";
    public EquipmentSlot            Slot          { get; init; } = EquipmentSlot.Helm;
    public bool                     IsTwoHanded   { get; init; }
    public IReadOnlyList<StatBonus> StatBonuses   { get; init; } = new List<StatBonus>();
}