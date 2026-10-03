using System;
using System.Collections.Generic;
using System.Text;
[Serializable]
public class PlayerEquipment
{
    public EquipmentData Helm;
    public EquipmentData Armor;
    public EquipmentData Trinket;
    public EquipmentData MainWeapon;
    public EquipmentData Offhand;

    public void ApplyTo(PlayerState state, string ownerLabel = "Player")
    {
        if (state == null) return;

        ApplyOne(Helm,       state, ownerLabel);
        ApplyOne(Armor,      state, ownerLabel);
        ApplyOne(Trinket,    state, ownerLabel);
        ApplyOne(MainWeapon, state, ownerLabel);

        bool offhandBlocked = MainWeapon != null && MainWeapon.IsTwoHanded;
        if (!offhandBlocked)
            ApplyOne(Offhand, state, ownerLabel);
        else if (Offhand != null)
            Console.WriteLine($"[Equipment] {ownerLabel}: Offhand '{Offhand.EquipmentName}' " +
                      $"ignored — MainWeapon '{MainWeapon.EquipmentName}' is two-handed.");
    }

    private static void ApplyOne(EquipmentData eq, PlayerState state, string ownerLabel)
    {
        if (eq == null) return;
        if (eq.StatBonuses.Count == 0) return;

        StringBuilder summary = new StringBuilder();
        for (int i = 0; i < eq.StatBonuses.Count; i++)
        {
            StatBonus bonus = eq.StatBonuses[i];
            ApplyBonusToState(state, bonus);

            if (i > 0) summary.Append(", ");
            summary.Append(bonus.Amount >= 0 ? "+" : "");
            summary.Append(bonus.Amount).Append(' ').Append(bonus.StatType);
        }

        Console.WriteLine($"[Equipment] {ownerLabel}: '{eq.EquipmentName}' ({eq.Slot}) applied {summary}.");
    }

    private static void ApplyBonusToState(PlayerState state, StatBonus bonus)
    {
        switch (bonus.StatType)
        {
            case StatType.Stamina:   state.Stamina   += bonus.Amount; break;
            case StatType.Wisdom:    state.Wisdom    += bonus.Amount; break;
            case StatType.Mastery:   state.Mastery   += bonus.Amount; break;
            case StatType.Charisma:  state.Charisma  += bonus.Amount; break;
            case StatType.Dexterity: state.Dexterity += bonus.Amount; break;
            case StatType.Strength:  state.Strength  += bonus.Amount; break;
            case StatType.Intellect: state.Intellect += bonus.Amount; break;
        }
    }

    public IEnumerable<(EquipmentSlot slot, EquipmentData data)> EnumerateSlots()
    {
        yield return (EquipmentSlot.Helm,       Helm);
        yield return (EquipmentSlot.Armor,      Armor);
        yield return (EquipmentSlot.Trinket,    Trinket);
        yield return (EquipmentSlot.MainWeapon, MainWeapon);
        yield return (EquipmentSlot.Offhand,    Offhand);
    }

    public bool TrySetSlot(EquipmentSlot slot, EquipmentData data)
    {
        if (data != null && data.Slot != slot) return false;

        switch (slot)
        {
            case EquipmentSlot.Helm:       Helm       = data; return true;
            case EquipmentSlot.Armor:      Armor      = data; return true;
            case EquipmentSlot.Trinket:    Trinket    = data; return true;
            case EquipmentSlot.MainWeapon: MainWeapon = data; return true;
            case EquipmentSlot.Offhand:    Offhand    = data; return true;
        }
        return false;
    }
}
