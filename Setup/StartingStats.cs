using System;
[Serializable]
public class StartingStats
{
    public int Stamina  = 30;
    public int Wisdom   = 5;
    public int Mastery  = 5;
    public int Charisma = 5;
    public int Dexterity = 5;
    public int Strength  = 5;
    public int Intellect = 2;
    public int MaxMana             = 10;
    public int MaxSpecial          = 10;
    public int MaxWisdomProgress   = 10;
    public int MaxCharismaProgress = 10;
    public int MaxHandSize         = 7;
    public int StartingHandSize = 3;

    public void ApplyTo(PlayerState state)
    {
        if (state == null) return;

        state.Stamina             = Stamina;
        state.Wisdom              = Wisdom;
        state.Mastery             = Mastery;
        state.Charisma            = Charisma;
        state.Dexterity           = Dexterity;
        state.Strength            = Strength;
        state.Intellect           = Intellect;
        state.MaxMana             = MaxMana;
        state.MaxSpecial          = MaxSpecial;
        state.MaxWisdomProgress   = MaxWisdomProgress;
        state.MaxCharismaProgress = MaxCharismaProgress;
        state.Hand.SetMaxSize(MaxHandSize);
    }
}
