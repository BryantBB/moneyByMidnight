using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UltimateTexasHoldEm
{
    /// <summary>
    /// Bridges the GameController to the Unity UI (no Trips side bet).
    /// Attach to the Canvas root. Wire all references in the Inspector.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        // ── Controller ────────────────────────────────────────────────────────
        [Header("Controller")]
        [SerializeField] private GameController game;

        // ── HUD Text ──────────────────────────────────────────────────────────
        [Header("HUD")]
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text phaseText;

        // ── Bet Input ─────────────────────────────────────────────────────────
        [Header("Bet Input")]
        [SerializeField] private TMP_InputField anteInput;

        // ── Live Bet Labels ───────────────────────────────────────────────────
        [Header("Bet Labels (live)")]
        [SerializeField] private TMP_Text anteBetLabel;
        [SerializeField] private TMP_Text blindBetLabel;
        [SerializeField] private TMP_Text playBetLabel;

        [SerializeField] private GameObject betLabels;

        // ── Card Areas ────────────────────────────────────────────────────────
        [Header("Card Areas")]
        [SerializeField] private Transform playerCardArea;
        [SerializeField] private Transform dealerCardArea;
        [SerializeField] private Transform communityCardArea;

        [Tooltip("Prefab with a CardView component. See CardView.cs for structure.")]
        [SerializeField] private GameObject cardPrefab;

        // ── Button Panels ─────────────────────────────────────────────────────
        [Header("Action Panels (one per phase)")]
        [SerializeField] private GameObject betPanel;
        [SerializeField] private GameObject preFlopPanel;
        [SerializeField] private GameObject flopPanel;
        [SerializeField] private GameObject turnRiverPanel;
        [SerializeField] private GameObject showdownPanel;

        // ── Result Display ────────────────────────────────────────────────────
        [Header("Result")]
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text playerHandText;
        [SerializeField] private TMP_Text dealerHandText;

        // ── Optional Screen Flash ─────────────────────────────────────────────
        [Header("Win/Lose Flash (optional)")]
        [Tooltip("Full-screen Image that briefly pulses green/red on round end. Raycast Target must be OFF.")]
        [SerializeField] private Image screenFlash;

        // help screen objects
        [SerializeField] private GameObject helpButton;
        [SerializeField] private GameObject helpPanel;

        [SerializeField] private GameObject closeHelpButton;

        // ── Private ───────────────────────────────────────────────────────────
        private readonly List<CardView> _playerCards    = new();
        private readonly List<CardView> _dealerCards    = new();
        private readonly List<CardView> _communityCards = new();

        // ═════════════════════════════════════════════════════════════════════
        //  Unity
        // ═════════════════════════════════════════════════════════════════════
        private void Start()
        {
            if (game == null) game = FindFirstObjectByType<GameController>();
            helpPanel.SetActive(true);
            game.OnPhaseChanged        += HandlePhaseChanged;
            game.OnBalanceChanged      += HandleBalanceChanged;
            game.OnPlayerCardsDealt    += HandlePlayerCards;
            game.OnDealerCardsDealt    += HandleDealerCards;
            game.OnCommunityCardsDealt += HandleCommunityCards;
            game.OnRoundResolved       += HandleRoundResult;
            game.OnMessage             += ShowMessage;

            UpdateBalance(game.Balance);
            SetAllPanels(GamePhase.PlaceBets);
            ClearResult();

            if (screenFlash) screenFlash.color = Color.clear;
        }

        private void OnDestroy()
        {
            if (game == null) return;
            game.OnPhaseChanged        -= HandlePhaseChanged;
            game.OnBalanceChanged      -= HandleBalanceChanged;
            game.OnPlayerCardsDealt    -= HandlePlayerCards;
            game.OnDealerCardsDealt    -= HandleDealerCards;
            game.OnCommunityCardsDealt -= HandleCommunityCards;
            game.OnRoundResolved       -= HandleRoundResult;
            game.OnMessage             -= ShowMessage;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Button Callbacks  (wire to Button.OnClick in the Inspector)
        // ═════════════════════════════════════════════════════════════════════

        public void Btn_PlaceBets()
        {
            float ante = ParseInput(anteInput, 10f);
            ClearAllCards();
            ClearResult();
            game.PlaceBets(ante);
        }

        public void Btn_Raise4x()      => game.RaisePreFlop(4);
        public void Btn_Raise3x()      => game.RaisePreFlop(3);
        public void Btn_CheckPreFlop() => game.CheckPreFlop();

        public void Btn_Raise2x()      => game.RaiseFlop();
        public void Btn_CheckFlop()    => game.CheckFlop();

        public void Btn_Raise1x()      => game.RaiseTurnRiver();
        public void Btn_Fold()         => game.Fold();

        public void Btn_NewRound()     => game.StartNewRound();

        public void Open_HelpPanel() => helpPanel.SetActive(true);
        public void Close_HelpPanel() => helpPanel.SetActive(false);



        // ═════════════════════════════════════════════════════════════════════
        //  Event Handlers
        // ═════════════════════════════════════════════════════════════════════

        private void HandlePhaseChanged(GamePhase phase)
        {
            if (phaseText) phaseText.text = PhaseLabel(phase);
            SetAllPanels(phase);
            RefreshBetLabels(phase);
        }

        private void HandleBalanceChanged(float balance) => UpdateBalance(balance);

        /// Player cards are always face-up.
        private void HandlePlayerCards(List<Card> cards, bool faceUp)
        {
            ClearCardList(_playerCards, playerCardArea);
            foreach (var card in cards)
                SpawnCard(card, faceUp, playerCardArea, _playerCards);
        }

        /// Dealer cards arrive twice:
        ///   1st call (faceUp=false) → spawn face-down at deal time
        ///   2nd call (faceUp=true)  → flip existing cards at showdown
        private void HandleDealerCards(List<Card> cards, bool faceUp)
        {
            if (!faceUp)
            {
                ClearCardList(_dealerCards, dealerCardArea);
                foreach (var card in cards)
                    SpawnCard(card, false, dealerCardArea, _dealerCards);
            }
            else
            {
                foreach (var view in _dealerCards)
                    view.Flip(true);
            }
        }

        /// Community cards accumulate — only newly added cards are spawned each call.
        private void HandleCommunityCards(List<Card> allCards)
        {
            int existing = _communityCards.Count;
            for (int i = existing; i < allCards.Count; i++)
                SpawnCard(allCards[i], true, communityCardArea, _communityCards);
        }

        private void HandleRoundResult(PayoutCalculator.RoundResult result)
        {
            if (resultText)
            {
                string outcome = result.Outcome switch
                {
                    PayoutCalculator.Outcome.PlayerWin => "You Win!",
                    PayoutCalculator.Outcome.DealerWin => "Dealer Wins",
                    _                                  => "Push"
                };

                string noQualify = result.DealerQualifies
                    ? ""
                    : "\n(Dealer doesn't qualify — Ante & Play pushed)";

                resultText.text =
                    $"{outcome}{noQualify}\n" +
                    $"Ante: {WL(result.AnteWinLoss)}   " +
                    $"Blind: {WL(result.BlindWinLoss)}   " +
                    $"Play: {WL(result.PlayWinLoss)}" +
                    $"\nNet: {WL(result.TotalWinLoss)}";
            }

            if (playerHandText) playerHandText.text = $"You: {result.PlayerHandName}";
            if (dealerHandText) dealerHandText.text = $"Dealer: {result.DealerHandName}";

            if (screenFlash)
            {
                Color flash = result.Outcome switch
                {
                    PayoutCalculator.Outcome.PlayerWin => new Color(0f,  1f, 0f, 0.25f),
                    PayoutCalculator.Outcome.DealerWin => new Color(1f,  0f, 0f, 0.25f),
                    _                                  => new Color(1f,  1f, 0f, 0.15f)
                };
                StartCoroutine(FlashScreen(flash));
            }
        }

        private void ShowMessage(string msg)
        {
            if (messageText) messageText.text = msg;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Helpers
        // ═════════════════════════════════════════════════════════════════════

        private void SpawnCard(Card card, bool faceUp, Transform parent, List<CardView> list)
        {
            if (cardPrefab == null)
            {
                Debug.LogError("[UIManager] cardPrefab is not assigned!");
                return;
            }

            var go   = Instantiate(cardPrefab, parent);
            var view = go.GetComponent<CardView>();

            if (view == null)
            {
                Debug.LogError("[UIManager] cardPrefab is missing a CardView component.");
                Destroy(go);
                return;
            }

            view.Setup(card, faceUp);
            list.Add(view);
        }

        private static void ClearCardList(List<CardView> list, Transform parent)
        {
            foreach (var view in list)
                if (view != null) Destroy(view.gameObject);
            list.Clear();
        }

        private void ClearAllCards()
        {
            ClearCardList(_playerCards,    playerCardArea);
            ClearCardList(_dealerCards,    dealerCardArea);
            ClearCardList(_communityCards, communityCardArea);
        }

        private void SetAllPanels(GamePhase phase)
        {
            betPanel?.SetActive(phase       == GamePhase.PlaceBets);
            preFlopPanel?.SetActive(phase   == GamePhase.PreFlop);
            flopPanel?.SetActive(phase      == GamePhase.Flop);
            turnRiverPanel?.SetActive(phase == GamePhase.TurnRiver);
            showdownPanel?.SetActive(phase  == GamePhase.Showdown);
        }

        private void UpdateBalance(float bal)
        {
            if (balanceText) balanceText.text = $"Balance: ${bal:N2}";
        }

        private void RefreshBetLabels(GamePhase phase)
        {
            betLabels?.SetActive(phase != GamePhase.Showdown);
            if (phase == GamePhase.PlaceBets) resultText.text = "";
            if (anteBetLabel)  anteBetLabel.text  = $"Ante: ${game.AnteBet:N2}";
            if (blindBetLabel) blindBetLabel.text = $"Blind: ${game.BlindBet:N2}";
            if (playBetLabel)  playBetLabel.text  = $"Play: ${game.PlayBet:N2}";
        }

        private void ClearResult()
        {
            if (resultText)     resultText.text     = "";
            if (playerHandText) playerHandText.text = "";
            if (dealerHandText) dealerHandText.text = "";
        }

        private static float ParseInput(TMP_InputField field, float fallback)
            => field != null && float.TryParse(field.text, out float v) ? v : fallback;

        private static string WL(float v)
            => v >= 0 ? $"<color=#00cc44>+${v:N2}</color>"
                      : $"<color=#cc2200>-${Mathf.Abs(v):N2}</color>";

        private static string PhaseLabel(GamePhase p) => p switch
        {
            GamePhase.PlaceBets  => "Place Your Bets",
            GamePhase.PreFlop    => "Pre-Flop — Raise 4x / 3x or Check",
            GamePhase.Flop       => "Flop — Raise 2x or Check",
            GamePhase.TurnRiver  => "Turn & River — Raise 1x or Fold",
            GamePhase.Showdown   => "Showdown",
            _                    => ""
        };

        private System.Collections.IEnumerator FlashScreen(Color target)
        {
            if (screenFlash == null) yield break;
            float duration = 0.5f;
            float elapsed  = 0f;

            screenFlash.color = target;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                screenFlash.color = Color.Lerp(target, Color.clear, elapsed / duration);
                yield return null;
            }
            screenFlash.color = Color.clear;
        }
    }
}