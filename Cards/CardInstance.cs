public class CardInstance
{
    public CardData Data { get; }

    public int  CurrentHealth { get; set; }
    public int  CurrentAttack { get; set; }
    public bool HasAttackedThisTurn { get; set; }
    public bool HasUsedAbilityThisTurn { get; set; }
    public bool IsBlocking { get; set; }

    public CardInstance(CardData data)
    {
        Data = data;

        if (data is CreatureData creature)
        {
            CurrentHealth = creature.MaxHealth;
            CurrentAttack = creature.Attack;
        }
    }


    public static CardInstance Create(CardData data)
    {
        return data switch
        {
            MageData    mageData    => new MageInstance(mageData),
            WarriorData warriorData => new WarriorInstance(warriorData),
            MinionData  minionData  => new MinionInstance(minionData),
            _                       => new CardInstance(data)
        };
    }

    public virtual void ResetToDefaults()
    {
        if (Data is CreatureData creature)
            CurrentHealth = creature.MaxHealth;
        HasAttackedThisTurn    = false;
        HasUsedAbilityThisTurn = false;
        IsBlocking             = false;
    }

    public virtual CardInstance Clone()
    {
        return new CardInstance(Data)
        {
            CurrentHealth          = CurrentHealth,
            HasAttackedThisTurn    = HasAttackedThisTurn,
            HasUsedAbilityThisTurn = HasUsedAbilityThisTurn,
            IsBlocking             = IsBlocking
        };
    }
}
