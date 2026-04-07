using System;

namespace UltimateTexasHoldEm
{
    public enum Suit { Spades, Hearts, Diamonds, Clubs }
    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, J, Q, K, A }

    [Serializable]
    public class Card
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public int NumericValue => (int)Rank;

        public string ShortName => $"{RankShort}{SuitSymbol}";

        private string RankShort => Rank switch
        {
            Rank.A   => "A",
            Rank.K  => "K",
            Rank.Q => "Q",
            Rank.J  => "J",
            Rank.Ten   => "10",
            _          => ((int)Rank).ToString()
        };

        private string SuitSymbol => Suit switch
        {
            Suit.Spades   => "♠",
            Suit.Hearts   => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs    => "♣",
            _             => "?"
        };

        public override string ToString() => ShortName;
    }
}
