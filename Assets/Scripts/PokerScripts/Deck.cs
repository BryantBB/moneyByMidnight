using System.Collections.Generic;
using UnityEngine;

namespace UltimateTexasHoldEm
{
    public class Deck
    {
        private List<Card> _cards = new List<Card>();

        public int Remaining => _cards.Count;

        public Deck()
        {
            Reset();
        }

        public void Reset()
        {
            _cards.Clear();
            foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
                foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
                    _cards.Add(new Card(suit, rank));
        }

        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public Card Deal()
        {
            if (_cards.Count == 0)
            {
                Debug.LogError("Deck is empty!");
                return null;
            }
            Card top = _cards[0];
            _cards.RemoveAt(0);
            return top;
        }

        public List<Card> DealMultiple(int count)
        {
            var hand = new List<Card>();
            for (int i = 0; i < count; i++)
                hand.Add(Deal());
            return hand;
        }
    }
}
