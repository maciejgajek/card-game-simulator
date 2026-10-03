using System.Collections.Generic;
public class PlayerState
{
    public int Stamina  = 30;
    public int Wisdom   = 5;
    public int Mastery  = 5;
    public int Charisma = 5;

    public int Dexterity = 5;
    public int Strength  = 5;
    public int Intellect = 2;

    public int Health;
    public int Mana;
    public int MaxMana            = 10;
    public int Special;
    public int MaxSpecial         = 10;
    public int ManaPerTurn;
    public int WisdomProgress;
    public int MaxWisdomProgress   = 10;
    public int CharismaProgress;
    public int MaxCharismaProgress = 10;

    public int IceShardsCastTotal;
    public int IceShardsCastThisTurn;

    public Deck Deck { get; private set; }
    public Hand Hand { get; } = new Hand();
    public Graveyard Graveyard { get; } = new Graveyard();
    public List<CardInstance> BoardCreatures { get; } = new List<CardInstance>();
    public List<CardInstance> BoardBuildings { get; } = new List<CardInstance>();

    public SpecialAbility SpecialAbility { get; set; }

    public void Initialize()
    {
        Health           = Stamina;
        Mana             = 0;
        ManaPerTurn      = 0;
        Special          = 0;
        WisdomProgress   = 0;
        CharismaProgress = 0;
    }

    public void TakeDamage(int amount) => Health = Math.Max(Health - amount, 0);
    public void Heal(int amount)       => Health = Math.Min(Health + amount, Stamina);

    public void AddCharisma(int amount)    => Charisma += amount;
    public void RemoveCharisma(int amount) => Charisma  = Math.Max(0, Charisma - amount);

    public bool IsSpecialFull => Special >= MaxSpecial;
    public void ResetSpecial() => Special = 0;
    public void ResetMana()    => Mana    = 0;
    public void SpendMana(int amount) => Mana = Math.Max(0, Mana - amount);

    public void GrantMana()
    {
        Mana = Math.Clamp(ManaPerTurn, 0, MaxMana);
    }

    public void IncreaseResources()
    {
        Special += Mastery;
        Special  = Math.Clamp(Special, 0, MaxSpecial);
    }

    public int IncreaseWisdom()
    {
        int overflows = 0;
        WisdomProgress += Wisdom;
        while (WisdomProgress >= MaxWisdomProgress)
        {
            WisdomProgress -= MaxWisdomProgress;
            ManaPerTurn++;
            overflows++;
        }
        return overflows;
    }

    public List<CardInstance> IncreaseCharisma()
    {
        var drawn = new List<CardInstance>();
        CharismaProgress += Charisma;
        while (CharismaProgress >= MaxCharismaProgress)
        {
            CharismaProgress -= MaxCharismaProgress;
            drawn.AddRange(DrawCards(1));
        }
        return drawn;
    }

    public void CreateDeck(List<CardData> cardList, GameRng rng)
    {
        Deck = new Deck(cardList);
        Deck.Shuffle(rng);
        Hand.Clear();
    }

    public List<CardInstance> DrawCards(int count)
    {
        var drawn = new List<CardInstance>(count);
        for (int i = 0; i < count; i++)
        {
            if (Hand.IsFull) break;
            CardInstance card = Deck.Draw();
            if (card == null) break;
            Hand.AddCard(card);
            drawn.Add(card);
        }
        return drawn;
    }

    public bool CanPlayCard(CardInstance card)
    {
        if (card == null) return false;
        if (!Hand.Cards.Contains(card)) return false;
        if (Mana < card.Data.ManaCost)  return false;
        return true;
    }

    public void AddCreatureToBoard(CardInstance card)
    {
        if (card != null) BoardCreatures.Add(card);
    }

    public bool RemoveCreatureFromBoard(CardInstance card)
    {
        if (card == null) return false;
        if (!BoardCreatures.Remove(card)) return false;
        foreach (Aura aura in card.Data.Auras)
            aura?.OnLeaveBoard(this);
        card.ResetToDefaults();
        Graveyard.Add(card);
        return true;
    }

    public void AddBuildingToBoard(CardInstance card)
    {
        if (card != null) BoardBuildings.Add(card);
    }

    public bool RemoveBuildingFromBoard(CardInstance card)
    {
        if (card == null) return false;
        if (!BoardBuildings.Remove(card)) return false;
        foreach (Aura aura in card.Data.Auras)
            aura?.OnLeaveBoard(this);
        card.ResetToDefaults();
        Graveyard.Add(card);
        return true;
    }

    public void PruneDestroyedBoardCards()
    {
        BoardCreatures.RemoveAll(c => c == null);
        BoardBuildings.RemoveAll(b => b == null);
    }

    public PlayerState Clone()
    {
        PlayerState copy = new PlayerState
        {
            Stamina             = Stamina,
            Wisdom              = Wisdom,
            Mastery             = Mastery,
            Charisma            = Charisma,
            Dexterity           = Dexterity,
            Strength            = Strength,
            Intellect           = Intellect,

            Health              = Health,
            Mana                = Mana,
            MaxMana             = MaxMana,
            Special             = Special,
            MaxSpecial          = MaxSpecial,
            ManaPerTurn         = ManaPerTurn,
            WisdomProgress      = WisdomProgress,
            MaxWisdomProgress   = MaxWisdomProgress,
            CharismaProgress    = CharismaProgress,
            MaxCharismaProgress = MaxCharismaProgress,

            IceShardsCastTotal    = IceShardsCastTotal,
            IceShardsCastThisTurn = IceShardsCastThisTurn,

            SpecialAbility      = SpecialAbility
        };

        copy.Deck = Deck?.Clone();

        foreach (CardInstance card in Hand.Cards)      copy.Hand.AddCard(card.Clone());
        foreach (CardInstance card in Graveyard.Cards) copy.Graveyard.Add(card.Clone());
        foreach (CardInstance card in BoardCreatures)  copy.BoardCreatures.Add(card.Clone());
        foreach (CardInstance card in BoardBuildings)  copy.BoardBuildings.Add(card.Clone());

        return copy;
    }
}
