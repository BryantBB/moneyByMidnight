using System.Collections.Generic;

namespace UltimateTexasHoldEm
{
    /// <summary>
    /// Handles payout calculation for Ultimate Texas Hold'em.
    ///
    /// Three bets only — Ante, Blind, Play:
    ///   Ante  — pays 1:1 if player wins AND dealer qualifies; pushes if dealer doesn't qualify.
    ///   Blind — pays bonus based on hand rank when player wins; pushes on dealer no-qualify.
    ///   Play  — pays 1:1 if player wins AND dealer qualifies; pushes if dealer doesn't qualify.
    ///
    /// No Trips side bet.
    /// </summary>
    public static class PayoutCalculator
    {
        // Blind bonus pay table — applies when player wins
        private static readonly Dictionary<HandRank, float> BlindPayouts = new()
        {
            { HandRank.RoyalFlush,    500f },
            { HandRank.StraightFlush, 50f  },
            { HandRank.FourOfAKind,   10f  },
            { HandRank.FullHouse,     3f   },
            { HandRank.Flush,         1.5f },
            { HandRank.Straight,      1f   },
            // Hands below Straight pay even money (1:1) when player wins
        };

        public enum Outcome { PlayerWin, DealerWin, Push }

        public struct RoundResult
        {
            public Outcome Outcome;
            public float   AnteWinLoss;
            public float   BlindWinLoss;
            public float   PlayWinLoss;
            public float   TotalWinLoss;
            public string  PlayerHandName;
            public string  DealerHandName;
            public bool    DealerQualifies;
        }

        /// <summary>Dealer qualifies with a pair or better.</summary>
        public static bool DealerQualifies(HandResult dealerHand)
            => dealerHand.Rank >= HandRank.OnePair;

        public static RoundResult Resolve(
            HandResult playerHand,
            HandResult dealerHand,
            float ante,
            float blind,
            float play)
        {
            var result = new RoundResult
            {
                PlayerHandName  = playerHand.FriendlyName(),
                DealerHandName  = dealerHand.FriendlyName(),
                DealerQualifies = DealerQualifies(dealerHand)
            };

            int cmp = playerHand.CompareTo(dealerHand);
            result.Outcome = cmp > 0 ? Outcome.PlayerWin
                           : cmp < 0 ? Outcome.DealerWin
                           : Outcome.Push;

            // ── Ante ──────────────────────────────────────────────────────────
            // Win + dealer qualifies  → pay 1:1
            // Win + dealer no-qualify → push (return stake, no profit)
            // Dealer wins             → lose stake
            // Push                   → return stake
            result.AnteWinLoss = result.Outcome switch
            {
                Outcome.PlayerWin when result.DealerQualifies  =>  ante,
                Outcome.PlayerWin                              =>  0f,   // push
                Outcome.DealerWin                              => -ante,
                _                                              =>  0f    // push
            };

            // ── Blind ─────────────────────────────────────────────────────────
            // Player wins → bonus payout based on hand rank (minimum 1:1)
            // Dealer wins → lose stake
            // Push        → return stake (0 profit)
            if (result.Outcome == Outcome.PlayerWin)
            {
                float mult = BlindPayouts.TryGetValue(playerHand.Rank, out float m) ? m : 1f;
                result.BlindWinLoss = blind * mult;
            }
            else if (result.Outcome == Outcome.DealerWin)
            {
                result.BlindWinLoss = -blind;
            }
            else
            {
                result.BlindWinLoss = 0f;
            }

            // ── Play ──────────────────────────────────────────────────────────
            // Same rule as Ante — only pays when dealer qualifies
            result.PlayWinLoss = result.Outcome switch
            {
                Outcome.PlayerWin when result.DealerQualifies  =>  play,
                Outcome.PlayerWin                              =>  0f,   // push
                Outcome.DealerWin                              => -play,
                _                                              =>  0f    // push
            };

            result.TotalWinLoss = result.AnteWinLoss + result.BlindWinLoss + result.PlayWinLoss;

            return result;
        }
    }
}