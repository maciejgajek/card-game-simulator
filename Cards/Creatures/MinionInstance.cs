public class MinionInstance : CardInstance
{
    private readonly MinionData minionData;
    private readonly int[]      abilityCooldowns;

    public MinionData MinionData => minionData;

    public MinionInstance(MinionData data) : base(data)
    {
        minionData       = data;
        abilityCooldowns = new int[data.Abilities.Count];
    }

    public bool CanUseAbility(int index)
    {
        if (index < 0 || index >= minionData.Abilities.Count) return false;
        return abilityCooldowns[index] <= 0;
    }

    public void UseAbility(GameState state, int index, PlayerState owner, PlayerState enemy)
    {
        MinionAbility ability   = minionData.Abilities[index];
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

    public override void ResetToDefaults()
    {
        base.ResetToDefaults();
        for (int i = 0; i < abilityCooldowns.Length; i++)
            abilityCooldowns[i] = 0;
    }

    public override CardInstance Clone()
    {
        MinionInstance copy = new MinionInstance(minionData)
        {
            CurrentHealth          = CurrentHealth,
            HasAttackedThisTurn    = HasAttackedThisTurn,
            HasUsedAbilityThisTurn = HasUsedAbilityThisTurn,
            IsBlocking             = IsBlocking
        };

        for (int i = 0; i < abilityCooldowns.Length; i++)
            copy.abilityCooldowns[i] = abilityCooldowns[i];

        return copy;
    }
}
