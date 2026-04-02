using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateTexasHoldEm
{
    public enum HandRank
    {
        HighCard = 0,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public class HandResult : IComparable<HandResult>
    {
        public HandRank Rank { get; }
        public List<Card> BestHand { get; }
        /// Tiebreaker values, descending importance
        public List<int> Tiebreakers { get; }

        public HandResult(HandRank rank, List<Card> bestHand, List<int> tiebreakers)
        {
            Rank       = rank;
            BestHand   = bestHand;
            Tiebreakers = tiebreakers;
        }

        public int CompareTo(HandResult other)
        {
            if (Rank != other.Rank) return Rank.CompareTo(other.Rank);
            for (int i = 0; i < Math.Min(Tiebreakers.Count, other.Tiebreakers.Count); i++)
            {
                int cmp = Tiebreakers[i].CompareTo(other.Tiebreakers[i]);
                if (cmp != 0) return cmp;
            }
            return 0;
        }

        public string FriendlyName() => Rank switch
        {
            HandRank.RoyalFlush    => "Royal Flush",
            HandRank.StraightFlush => "Straight Flush",
            HandRank.FourOfAKind   => "Four of a Kind",
            HandRank.FullHouse     => "Full House",
            HandRank.Flush         => "Flush",
            HandRank.Straight      => "Straight",
            HandRank.ThreeOfAKind  => "Three of a Kind",
            HandRank.TwoPair       => "Two Pair",
            HandRank.OnePair       => "One Pair",
            _                      => "High Card"
        };
    }

    public static class HandEvaluator
    {
        /// Evaluate best 5-card hand from any number of cards (≥5)
        public static HandResult Evaluate(List<Card> cards)
        {
            if (cards.Count < 5)
                throw new ArgumentException("Need at least 5 cards to evaluate.");

            var combos = GetCombinations(cards, 5);
            HandResult best = null;
            foreach (var combo in combos)
            {
                var result = EvaluateFive(combo);
                if (best == null || result.CompareTo(best) > 0)
                    best = result;
            }
            return best;
        }

        private static HandResult EvaluateFive(List<Card> five)
        {
            var sorted   = five.OrderByDescending(c => c.NumericValue).ToList();
            bool isFlush = five.All(c => c.Suit == five[0].Suit);
            bool isStraight = CheckStraight(sorted, out int straightHigh);

            var groups = sorted.GroupBy(c => c.NumericValue)
                               .OrderByDescending(g => g.Count())
                               .ThenByDescending(g => g.Key)
                               .ToList();

            int[] groupCounts = groups.Select(g => g.Count()).ToArray();
            int[] groupKeys   = groups.Select(g => g.Key).ToArray();

            // Royal Flush
            if (isFlush && isStraight && straightHigh == 14)
                return new HandResult(HandRank.RoyalFlush, five, new List<int> { 14 });

            // Straight Flush
            if (isFlush && isStraight)
                return new HandResult(HandRank.StraightFlush, five, new List<int> { straightHigh });

            // Four of a Kind
            if (groupCounts[0] == 4)
                return new HandResult(HandRank.FourOfAKind, five,
                    new List<int> { groupKeys[0], groupKeys[1] });

            // Full House
            if (groupCounts[0] == 3 && groupCounts[1] == 2)
                return new HandResult(HandRank.FullHouse, five,
                    new List<int> { groupKeys[0], groupKeys[1] });

            // Flush
            if (isFlush)
                return new HandResult(HandRank.Flush, five,
                    sorted.Select(c => c.NumericValue).ToList());

            // Straight
            if (isStraight)
                return new HandResult(HandRank.Straight, five, new List<int> { straightHigh });

            // Three of a Kind
            if (groupCounts[0] == 3)
                return new HandResult(HandRank.ThreeOfAKind, five,
                    new List<int> { groupKeys[0], groupKeys[1], groupKeys[2] });

            // Two Pair
            if (groupCounts[0] == 2 && groupCounts[1] == 2)
                return new HandResult(HandRank.TwoPair, five,
                    new List<int> { groupKeys[0], groupKeys[1], groupKeys[2] });

            // One Pair
            if (groupCounts[0] == 2)
                return new HandResult(HandRank.OnePair, five,
                    new List<int> { groupKeys[0], groupKeys[1], groupKeys[2], groupKeys.Length > 3 ? groupKeys[3] : 0 });

            // High Card
            return new HandResult(HandRank.HighCard, five,
                sorted.Select(c => c.NumericValue).ToList());
        }

        private static bool CheckStraight(List<Card> sorted, out int high)
        {
            var vals = sorted.Select(c => c.NumericValue).Distinct().OrderByDescending(v => v).ToList();

            // Wheel (A-2-3-4-5)
            if (vals.Contains(14) && vals.Contains(2) && vals.Contains(3) && vals.Contains(4) && vals.Contains(5))
            {
                high = 5;
                return true;
            }

            if (vals.Count >= 5)
            {
                for (int i = 0; i <= vals.Count - 5; i++)
                {
                    if (vals[i] - vals[i + 4] == 4)
                    {
                        high = vals[i];
                        return true;
                    }
                }
            }

            high = 0;
            return false;
        }

        private static IEnumerable<List<Card>> GetCombinations(List<Card> cards, int k)
        {
            int n = cards.Count;
            int[] indices = Enumerable.Range(0, k).ToArray();

            yield return indices.Select(i => cards[i]).ToList();

            while (true)
            {
                int i2 = k - 1;
                while (i2 >= 0 && indices[i2] == i2 + n - k) i2--;
                if (i2 < 0) yield break;
                indices[i2]++;
                for (int j = i2 + 1; j < k; j++) indices[j] = indices[j - 1] + 1;
                yield return indices.Select(i => cards[i]).ToList();
            }
        }
    }
}
