using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateTexasHoldEm
{
    
    public class CardSpriteLibrary : MonoBehaviour
    {
        // ── Singleton ─────────────────────────────────────────────────────────
        public static CardSpriteLibrary Instance { get; private set; }

        // ── Inspector ─────────────────────────────────────────────────────────
        [Header("Resource Path")]
        [Tooltip("Folder name inside any Resources/ folder. Default: 'Deck01'")]
        [SerializeField] private string resourceFolder = "Deck01";

        [Tooltip("Filename prefix used by the asset package. Default: 'Deck01'")]
        [SerializeField] private string deckPrefix = "Deck01";

        [Header("Child Object Names (from prefab)")]
        [Tooltip("Exact name of the front-face child GameObject inside each card prefab.")]
        [SerializeField] private string frontChildName = "Front ";   // note the trailing space

        [Tooltip("Exact name of the card-back child GameObject inside each card prefab.")]
        [SerializeField] private string backChildName  = "Back_D1";

        // ── Caches ────────────────────────────────────────────────────────────
        private readonly Dictionary<string, Sprite> _faceCache = new();
        private Sprite _backSprite; // same back sprite shared across all cards

        // ── Naming maps (match Asset_PlayingCards convention exactly) ─────────
        // Suit:  "Club", "Diamond", "Heart", "Spade"  (singular, no 's')
        private static readonly Dictionary<Suit, string> SuitSegment = new()
        {
            { Suit.Clubs,    "Club"    },
            { Suit.Diamonds, "Diamond" },
            { Suit.Hearts,   "Heart"   },
            { Suit.Spades,   "Spade"   },
        };

        // Rank: digits for 2-10, spelled for face cards
        // Confirmed from: Deck01_Club_2
        private static readonly Dictionary<Rank, string> RankSegment = new()
        {
            { Rank.Two,   "2"     }, { Rank.Three, "3"     },
            { Rank.Four,  "4"     }, { Rank.Five,  "5"     },
            { Rank.Six,   "6"     }, { Rank.Seven, "7"     },
            { Rank.Eight, "8"     }, { Rank.Nine,  "9"     },
            { Rank.Ten,   "10"    }, { Rank.J,  "J"  },
            { Rank.Q, "Q" }, { Rank.K,  "K"  },
            { Rank.A,   "A"   },
        };

        // ═════════════════════════════════════════════════════════════════════
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PreloadAll();
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Public API
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>Returns the face Sprite for the given card, or null if not found.</summary>
        public Sprite GetCardSprite(Card card)
        {
            string key = BuildPrefabName(card);
            if (_faceCache.TryGetValue(key, out Sprite cached)) return cached;

            // Lazy-load fallback (should not be needed after PreloadAll)
            LoadBothSpritesFromPrefab(key);
            return _faceCache.TryGetValue(key, out Sprite lazy) ? lazy : null;
        }

        /// <summary>
        /// Returns the shared card-back Sprite.
        /// Extracted from the Back_D1 child of the first card prefab loaded.
        /// </summary>
        public Sprite GetBackSprite() => _backSprite;

        /// <summary>
        /// Builds the prefab filename for a card.
        /// Format: Deck01_{Suit}_{Rank}  e.g. Deck01_Club_2, Deck01_Heart_Ace
        /// </summary>
        public string BuildPrefabName(Card card)
        {
            string suit = SuitSegment.TryGetValue(card.Suit, out string s) ? s : card.Suit.ToString();
            string rank = RankSegment.TryGetValue(card.Rank, out string r) ? r : card.Rank.ToString();
            return $"{deckPrefix}_{suit}_{rank}";
        }

        // ═════════════════════════════════════════════════════════════════════
        //  Private
        // ═════════════════════════════════════════════════════════════════════

        private void PreloadAll()
        {
            int loaded = 0;

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    string key = BuildPrefabName(new Card(suit, rank));
                    LoadBothSpritesFromPrefab(key);
                    if (_faceCache.ContainsKey(key)) loaded++;
                }
            }

            Debug.Log($"[CardSpriteLibrary] Loaded {loaded}/52 face sprites. " +
                      $"Back sprite: {(_backSprite != null ? "OK" : "MISSING")}");
        }

        /// <summary>
        /// Loads one Deck01 prefab and extracts:
        ///   • the face sprite  → stored in _faceCache[prefabName]
        ///   • the back sprite  → stored in _backSprite (once, reused for all cards)
        /// </summary>
        private void LoadBothSpritesFromPrefab(string prefabName)
        {
            string path    = $"{resourceFolder}/{prefabName}";
            GameObject prefab = Resources.Load<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogWarning($"[CardSpriteLibrary] Prefab not found: '{path}'\n" +
                                 $"Did you copy it into Assets/.../Resources/{resourceFolder}/?");
                return;
            }

            // ── Front sprite ──────────────────────────────────────────────────
            Sprite face = FindSpriteInChild(prefab, frontChildName);
            if (face != null)
                _faceCache[prefabName] = face;
            else
                Debug.LogWarning($"[CardSpriteLibrary] No sprite on child '{frontChildName}' " +
                                 $"in prefab '{prefabName}'.");

            // ── Back sprite (grab once; identical in every prefab) ─────────────
            if (_backSprite == null)
            {
                _backSprite = FindSpriteInChild(prefab, backChildName);
                if (_backSprite == null)
                    Debug.LogWarning($"[CardSpriteLibrary] No sprite on child '{backChildName}' " +
                                     $"in prefab '{prefabName}'.");
            }
        }

        private static Sprite FindSpriteInChild(GameObject prefab, string childName)
        {
            foreach (Transform c in prefab.transform)
            {
                Debug.Log($"{c.name.Length} + {String.Equals(c.name, childName)} + {childName.Length}");
                if (String.Equals(c.name.Trim(), childName.Trim()))
                {
                    var sr = c.GetComponent<SpriteRenderer>();
                    return sr != null ? sr.sprite : null;
                }
            }

            // Transform child = prefab.transform.Find(childName);
            // if (child == null) {
            Debug.LogWarning($"[CardSpriteLibrary] Child '{childName}' not found in prefab '{prefab.name}'.");
            return null;
            // }

            // var sr = child.GetComponent<SpriteRenderer>();
            // return sr != null ? sr.sprite : null;
        }
    }
}