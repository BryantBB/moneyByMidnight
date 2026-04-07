using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateTexasHoldEm
{
    public enum GamePhase { PlaceBets, PreFlop, Flop, TurnRiver, Showdown }

    /// <summary>
    /// Core game controller for Ultimate Texas Hold'em (no Trips side bet).
    /// Attach to a scene GameObject. Subscribe to events in UI scripts.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        // ── Inspector ─────────────────────────────────────────────────────────
        [Header("Session")]
        [SerializeField] private float startingBalance = 1000f;
        [SerializeField] private float minAnte         = 5f;
        [SerializeField] private float maxAnte         = 500f;

        // ── Public State ──────────────────────────────────────────────────────
        public float     Balance     { get; private set; }
        public GamePhase Phase       { get; private set; }

        public float AnteBet         { get; private set; }
        public float BlindBet        { get; private set; }
        public float PlayBet         { get; private set; }

        public List<Card> PlayerHand     { get; private set; } = new();
        public List<Card> DealerHand     { get; private set; } = new();
        public List<Card> CommunityCards { get; private set; } = new();

        public bool PlayerFolded   { get; private set; }
        public bool CheckedPreFlop { get; private set; }

        public PayoutCalculator.RoundResult LastResult { get; private set; }

        // ── Events ────────────────────────────────────────────────────────────
        public event Action<GamePhase>                    OnPhaseChanged;
        public event Action<float>                        OnBalanceChanged;
        public event Action<List<Card>, bool>             OnPlayerCardsDealt;   // (cards, faceUp)
        public event Action<List<Card>, bool>             OnDealerCardsDealt;
        public event Action<List<Card>>                   OnCommunityCardsDealt;
        public event Action<PayoutCalculator.RoundResult> OnRoundResolved;
        public event Action<string>                       OnMessage;

        // ── Private ───────────────────────────────────────────────────────────
        private readonly Deck _deck = new();

        // ═════════════════════════════════════════════════════════════════════
        private void Awake()
        {
            Balance = startingBalance;
            Phase   = GamePhase.PlaceBets;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Public API — called from UI buttons
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Place Ante + Blind (equal to Ante) and deal initial cards.
        /// Total cost = ante x 2.
        /// </summary>
        public void PlaceBets(float ante)
        {
            if (Phase != GamePhase.PlaceBets) { Msg("Wait for the next round."); return; }

            ante = Mathf.Clamp(ante, minAnte, maxAnte);
            float cost = ante * 2f;

            if (cost > Balance) { Msg("Insufficient balance."); return; }

            AnteBet        = ante;
            BlindBet       = ante;
            PlayBet        = 0f;
            PlayerFolded   = false;
            CheckedPreFlop = false;

            Balance -= cost;
            OnBalanceChanged?.Invoke(Balance);

            DealInitialCards();
        }

        /// <summary>Pre-flop: raise 3x or 4x the ante. Transitions to Flop.</summary>
        public void RaisePreFlop(int multiplier)
        {
            if (Phase != GamePhase.PreFlop) return;
            multiplier = Mathf.Clamp(multiplier, 3, 4);

            float amount = AnteBet * multiplier;
            if (!TrySpend(amount)) return;

            PlayBet = amount;
            Msg($"Raised {multiplier}x — Play: ${PlayBet:0.##}");
            DealFlop();
        }

        /// <summary>Pre-flop: check (no play bet yet). Transitions to Flop.</summary>
        public void CheckPreFlop()
        {
            if (Phase != GamePhase.PreFlop) return;
            CheckedPreFlop = true;
            Msg("Check.");
            DealFlop();
        }

        /// <summary>Post-flop: raise 2x ante (only if checked pre-flop). Goes to Turn/River.</summary>
        public void RaiseFlop()
        {
            if (Phase != GamePhase.Flop) return;
            float amount = AnteBet * 2f;
            if (!TrySpend(amount)) return;

            PlayBet += amount;
            Msg($"Raised 2x — Play: ${PlayBet:0.##}");
            DealTurnRiver();
        }

        /// <summary>Post-flop: check (only if checked pre-flop). Goes to Turn/River.</summary>
        public void CheckFlop()
        {
            if (Phase != GamePhase.Flop) return;
            Msg("Check.");
            DealTurnRiver();
        }

        /// <summary>Turn/River: place 1x play bet then go to showdown.</summary>
        public void RaiseTurnRiver()
        {
            if (Phase != GamePhase.TurnRiver) return;
            if (!TrySpend(AnteBet)) return;

            PlayBet += AnteBet;
            Msg($"Raised 1x — Play: ${PlayBet:0.##}");
            RunShowdown();
        }

        /// <summary>Turn/River: fold — lose ante + blind, no play bet needed.</summary>
        public void Fold()
        {
            if (Phase != GamePhase.TurnRiver) return;
            PlayerFolded = true;
            Msg("Folded.");
            RunShowdown();
        }

        /// <summary>Reset and start a new round.</summary>
        public void StartNewRound()
        {
            if (Phase != GamePhase.Showdown) return;

            if (Balance < minAnte * 2f)
            {
                Msg("Insufficient Funds. Game Over.");
                
            }

            PlayerHand.Clear();
            DealerHand.Clear();
            CommunityCards.Clear();
            SetPhase(GamePhase.PlaceBets);
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Private: Deal Flow
        // ═════════════════════════════════════════════════════════════════════

        private void DealInitialCards()
        {
            _deck.Reset();
            _deck.Shuffle();

            PlayerHand = _deck.DealMultiple(2);
            DealerHand = _deck.DealMultiple(2);

            OnPlayerCardsDealt?.Invoke(new List<Card>(PlayerHand), true);
            OnDealerCardsDealt?.Invoke(new List<Card>(DealerHand), false);
            SetPhase(GamePhase.PreFlop);
        }

        private void DealFlop()
        {
            CommunityCards.AddRange(_deck.DealMultiple(3));
            OnCommunityCardsDealt?.Invoke(new List<Card>(CommunityCards));
            SetPhase(GamePhase.Flop);
        }

        private void DealTurnRiver()
        {
            CommunityCards.AddRange(_deck.DealMultiple(2));
            OnCommunityCardsDealt?.Invoke(new List<Card>(CommunityCards));
            SetPhase(GamePhase.TurnRiver);
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Private: Showdown & Payout
        // ═════════════════════════════════════════════════════════════════════

        private void RunShowdown()
        {
            SetPhase(GamePhase.Showdown);
            OnDealerCardsDealt?.Invoke(new List<Card>(DealerHand), true);

            // Build 7-card hands
            var pAll = new List<Card>(PlayerHand);
            pAll.AddRange(CommunityCards);
            var dAll = new List<Card>(DealerHand);
            dAll.AddRange(CommunityCards);

            var playerResult = HandEvaluator.Evaluate(pAll);
            var dealerResult = HandEvaluator.Evaluate(dAll);

            PayoutCalculator.RoundResult result;

            if (PlayerFolded)
            {
                // Ante and Blind are already deducted and lost on fold — no return
                result = new PayoutCalculator.RoundResult
                {
                    Outcome         = PayoutCalculator.Outcome.DealerWin,
                    AnteWinLoss     = 0f,
                    BlindWinLoss    = 0f,
                    PlayWinLoss     = 0f,
                    TotalWinLoss    = 0f,
                    PlayerHandName  = playerResult.FriendlyName(),
                    DealerHandName  = "Dealer wins (fold)",
                    DealerQualifies = true
                };
                // No balance change — bets were already deducted, nothing returned
            }
            else
            {
                result = PayoutCalculator.Resolve(
                    playerResult, dealerResult,
                    AnteBet, BlindBet, PlayBet);

                // Return stakes + winnings for each non-losing bet
                float netReturn = 0f;
                netReturn += SettleBet(AnteBet,  result.AnteWinLoss);
                netReturn += SettleBet(BlindBet, result.BlindWinLoss);
                netReturn += SettleBet(PlayBet,  result.PlayWinLoss);

                Balance += netReturn;
            }

            LastResult = result;
            OnBalanceChanged?.Invoke(Balance);
            OnRoundResolved?.Invoke(result);
        }

        /// <summary>
        /// On win  → return stake + profit.
        /// On push → return stake only.
        /// On loss → return nothing (stake already deducted).
        /// </summary>
        private static float SettleBet(float stake, float winLoss)
        {
            if (winLoss > 0)  return stake + winLoss;
            if (winLoss == 0) return stake;
            return 0f;
        }

        private bool TrySpend(float amount)
        {
            if (amount > Balance) { Msg("Insufficient balance."); return false; }
            Balance -= amount;
            OnBalanceChanged?.Invoke(Balance);
            return true;
        }

        private void SetPhase(GamePhase p) { Phase = p; OnPhaseChanged?.Invoke(p); }
        private void Msg(string m) => OnMessage?.Invoke(m);
    }
}