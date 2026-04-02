using UnityEngine;
using UnityEngine.UI;

namespace UltimateTexasHoldEm
{
    /// <summary>
    /// Controls a single card UI prefab using sprites extracted from the
    /// Asset_PlayingCards package (Deck01_{suit}_{rank} prefabs).
    ///
    /// Each Deck01 prefab contains TWO children:
    ///   "Front "  — SpriteRenderer with the card face
    ///   "Back_D1" — SpriteRenderer with the card back (same in every prefab)
    ///
    /// CardSpriteLibrary loads each prefab and extracts both sprites.
    /// This CardView receives them and displays on plain UI Images so the
    /// cards sit correctly inside Unity's Canvas layout system.
    ///
    /// ── YOUR OWN CARD UI PREFAB STRUCTURE ──────────────────────────────────
    ///
    ///   CardRoot  (RectTransform, e.g. 80 × 112 px)
    ///    ├── FaceImage   (Image, Preserve Aspect: ✓)  ← set at runtime
    ///    └── BackImage   (Image, Preserve Aspect: ✓)  ← set at runtime
    ///
    ///  • Leave both Source Images blank — CardView assigns sprites at runtime.
    ///  • Do NOT place Deck01 prefabs directly in the Canvas.
    ///  • Attach CardView to CardRoot and wire FaceImage / BackImage below.
    /// ───────────────────────────────────────────────────────────────────────
    /// </summary>
    public class CardView : MonoBehaviour
    {
        [Header("Sprite Images (wire in prefab)")]
        [SerializeField] private Image faceImage;   // displays card face
        [SerializeField] private Image backImage;   // displays card back

        [Header("Flip Animation (optional)")]
        [Tooltip("If true, plays a simple scale-flip animation when Flip() is called.")]
        [SerializeField] private bool animateFlip = true;
        [SerializeField] private float flipDuration = 0.25f;

        // ── State ─────────────────────────────────────────────────────────────
        private Card _card;
        private bool _faceUp;
        private bool _flipping;

        // ═════════════════════════════════════════════════════════════════════
        //  Public API
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Initialise the card with a data model and orientation.
        /// Call this immediately after Instantiate.
        /// </summary>
        public void Setup(Card card, bool faceUp)
        {
            _card   = card;
            _faceUp = faceUp;
            ApplySprites();
            SetVisibility(faceUp);
        }

        /// <summary>Flip the card to face-up or face-down, with optional animation.</summary>
        public void Flip(bool faceUp)
        {
            if (_faceUp == faceUp || _flipping) return;
            _faceUp = faceUp;

            if (animateFlip && gameObject.activeInHierarchy)
                StartCoroutine(FlipCoroutine());
            else
                SetVisibility(faceUp);
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Private
        // ═════════════════════════════════════════════════════════════════════

        private void ApplySprites()
        {
            var library = CardSpriteLibrary.Instance;
            if (library == null)
            {
                Debug.LogError("[CardView] CardSpriteLibrary not found in scene. " +
                               "Add it to a persistent GameObject.");
                return;
            }

            if (faceImage != null)
            {
                Sprite face = library.GetCardSprite(_card);
                faceImage.sprite = face;
                faceImage.color  = Color.white; // reset any tint
            }

            if (backImage != null)
            {
                Sprite back = library.GetBackSprite();
                backImage.sprite = back;
                backImage.color  = Color.white;
            }
        }

        private void SetVisibility(bool showFace)
        {
            if (faceImage != null) faceImage.gameObject.SetActive(showFace);
            if (backImage != null) backImage.gameObject.SetActive(!showFace);
        }

        // ── Flip coroutine ────────────────────────────────────────────────────
        private System.Collections.IEnumerator FlipCoroutine()
        {
            _flipping = true;
            float half = flipDuration * 0.5f;
            var tf = transform;

            // Scale X to 0 (first half)
            float elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / half;
                tf.localScale = new Vector3(1f - t, 1f, 1f);
                yield return null;
            }
            tf.localScale = new Vector3(0f, 1f, 1f);

            // Swap face/back at the midpoint
            SetVisibility(_faceUp);

            // Scale X back to 1 (second half)
            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / half;
                tf.localScale = new Vector3(t, 1f, 1f);
                yield return null;
            }
            tf.localScale = Vector3.one;

            _flipping = false;
        }
    }
}