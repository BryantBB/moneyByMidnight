using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePanel : MonoBehaviour
{
    public GameObject timePanel;

    void KeyCheck()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame) // checks for the 'T' key
        {
            ToggleVis();
        }
    }

    public void ToggleVis()
    {
        if (timePanel != null)
        {
            bool isActive = timePanel.activeSelf;
            timePanel.SetActive(!isActive);
        }
    }
}
