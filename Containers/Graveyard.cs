using System.Collections.Generic;

// Ordered: index 0 is the oldest entry, last index is the most recent.
public class Graveyard
{
    private readonly List<CardInstance> cards = new List<CardInstance>();

    public IReadOnlyList<CardInstance> Cards => cards;
    public int Count => cards.Count;

    public void Add(CardInstance card)
    {
        if (card == null) return;
        cards.Add(card);
    }

    public CardInstance Remove(CardInstance card)
    {
        if (card == null || !cards.Remove(card)) return null;
        return card;
    }

    public void Clear() => cards.Clear();

    public Graveyard Clone()
    {
        Graveyard copy = new Graveyard();
        foreach (CardInstance card in cards)
            copy.Add(card.Clone());
        return copy;
    }
}
