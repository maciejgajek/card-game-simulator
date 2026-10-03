using System.Collections.Generic;

// Top of deck is index 0. Random ops use the caller's GameRng for determinism.
public class Deck
{
    private readonly List<CardInstance> cards = new List<CardInstance>();

    public Deck(List<CardData> cardList)
    {
        foreach (var data in cardList)
            cards.Add(CardInstance.Create(data));
    }

    private Deck() { }

    public CardInstance Draw()
    {
        if (cards.Count == 0) return null;
        CardInstance card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    // Fisher-Yates.
    public void Shuffle(GameRng rng)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int rand = rng.Next(i, cards.Count);
            (cards[i], cards[rand]) = (cards[rand], cards[i]);
        }
    }

    public int Count => cards.Count;

    public Deck Clone()
    {
        Deck copy = new Deck();
        foreach (CardInstance card in cards)
            copy.cards.Add(card.Clone());
        return copy;
    }
}
