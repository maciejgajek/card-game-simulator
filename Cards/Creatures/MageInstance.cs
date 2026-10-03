public class MageInstance : CardInstance
{
    private readonly MageData mageData;
    private int currentMana;
    private readonly int[] abilityCooldowns;

    public int      CurrentMana => currentMana;
    public MageData MageData    => mageData;

    public MageInstance(MageData data) : base(data)
    {
        mageData         = data;
        currentMana      = data.MaxMana;
        abilityCooldowns = new int[data.Abilities.Count];
    }

    // Mages either attack OR use an ability per turn — never both
    public bool CanUseAbility(int index)
    {
        if (index < 0 || index >= mageData.Abilities.Count) return false;
        return !HasAttackedThisTurn          &&
               abilityCooldowns[index] <= 0  &&
               currentMana >= mageData.Abilities[index].ManaCost;
    }

    public void UseAbility(GameState state, int index, PlayerState owner, PlayerState enemy)
    {
        MageAbility ability     = mageData.Abilities[index];
        currentMana            -= ability.ManaCost;
        abilityCooldowns[index] = ability.TurnCooldown;
        ability.Execute(state, this, owner, enemy);
    }

    public int GetCooldown(int index) =>
        (index >= 0 && index < abilityCooldowns.Length) ? abilityCooldowns[index] : 0;

    public void OnTurnStart()
    {
        for (int i = 0; i < abilityCooldowns.Length; i++)
            if (abilityCooldowns[i] > 0) abilityCooldowns[i]--;
    }

    public void OnTurnEnd()
    {
        currentMana = Math.Min(currentMana + mageData.ManaRegen, mageData.MaxMana);
    }

    public override void ResetToDefaults()
    {
        base.ResetToDefaults();
        currentMana = mageData.MaxMana;
        for (int i = 0; i < abilityCooldowns.Length; i++)
            abilityCooldowns[i] = 0;
    }

    public override CardInstance Clone()
    {
        MageInstance copy = new MageInstance(mageData)
        {
            CurrentHealth          = CurrentHealth,
            HasAttackedThisTurn    = HasAttackedThisTurn,
            HasUsedAbilityThisTurn = HasUsedAbilityThisTurn,
            IsBlocking             = IsBlocking,
            currentMana            = currentMana
        };
        for (int i = 0; i < abilityCooldowns.Length; i++)
            copy.abilityCooldowns[i] = abilityCooldowns[i];
        return copy;
    }
}
