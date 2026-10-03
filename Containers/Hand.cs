using System.Collections.Generic;
[System.Serializable]
public class Hand
{
    private List<CardInstance> cards = new List<CardInstance>();
    private int maxHandSize = 7;

    public List<CardInstance> Cards => cards;
    public int Count => cards.Count;
    public bool IsFull => cards.Count >= maxHandSize;

    public bool AddCard(CardInstance card)
    {
        if (IsFull) return false;
        cards.Add(card);
        return true;
    }

    public bool RemoveCard(CardInstance card) => cards.Remove(card);

    public void Clear() => cards.Clear();

    public void SetMaxSize(int max) => maxHandSize = max;

    public Hand Clone()
    {
        Hand copy = new Hand { maxHandSize = maxHandSize };
        foreach (CardInstance card in cards)
            copy.AddCard(card.Clone());
        return copy;
    }
}
