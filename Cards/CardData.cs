using System;
using System.Collections.Generic;

public abstract class CardData
{
    public string CardName { get; init; } = "";
    public int    ManaCost { get; init; }
    public string Text     { get; init; } = "";

    public abstract CardType Type { get; }

    public virtual IReadOnlyList<CardEffect> PlayEffects => Array.Empty<CardEffect>();

    public virtual IReadOnlyList<Aura> Auras => Array.Empty<Aura>();
}