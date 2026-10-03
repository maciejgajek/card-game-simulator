public class WarriorInstance : CardInstance
{
    private readonly WarriorData warriorData;
    private int currentRage;

    public int         CurrentRage => currentRage;
    public WarriorData WarriorData => warriorData;

    public WarriorInstance(WarriorData data) : base(data)
    {
        warriorData = data;
        currentRage = 0;
    }

    public bool CanUseAbility(int index)
    {
        if (index < 0 || index >= warriorData.Abilities.Count) return false;
        return currentRage >= warriorData.Abilities[index].RageCost;
    }

    public void UseAbility(GameState state, int index, PlayerState owner, PlayerState enemy)
    {
        WarriorAbility ability = warriorData.Abilities[index];
        currentRage           -= ability.RageCost;
        ability.Execute(state, this, owner, enemy);
    }

    public void AddRage(int amount)
    {
        currentRage = Math.Min(currentRage + amount, warriorData.MaxRage);
    }

    public override void ResetToDefaults()
    {
        base.ResetToDefaults();
        currentRage = 0;
    }

    public override CardInstance Clone()
    {
        return new WarriorInstance(warriorData)
        {
            CurrentHealth          = CurrentHealth,
            HasAttackedThisTurn    = HasAttackedThisTurn,
            HasUsedAbilityThisTurn = HasUsedAbilityThisTurn,
            IsBlocking             = IsBlocking,
            currentRage            = currentRage
        };
    }
}
