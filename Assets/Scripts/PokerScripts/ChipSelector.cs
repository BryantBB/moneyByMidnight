using UnityEngine;
using TMPro;

namespace UltimateTexasHoldEm
{
    /// <summary>
    /// Optional helper: attach chip buttons to quickly set the ante input.
    /// Each button calls SetAnte(value) with its chip denomination.
    /// </summary>
    public class ChipSelector : MonoBehaviour
    {
        [SerializeField] private TMP_InputField anteInput;

        private float _pendingAnte = 0f;

        public void AddToAnte(float amount)
        {
            _pendingAnte += amount;
            if (anteInput) anteInput.text = _pendingAnte.ToString("0");
        }

        public void ClearBets()
        {
            _pendingAnte = 0f;
            if (anteInput) anteInput.text = "0";
        }

        public void SetAnte(float amount)
        {
            _pendingAnte = amount;
            if (anteInput) anteInput.text = amount.ToString("0");
        }
    }
}
