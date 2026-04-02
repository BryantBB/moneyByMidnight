using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class SceneTeleport : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame) SceneManager.LoadScene("Casino");
        if (Keyboard.current.f2Key.wasPressedThisFrame) SceneManager.LoadScene("Tic-Tac-Toe Slots");
        if (Keyboard.current.f3Key.wasPressedThisFrame) SceneManager.LoadScene("Roulette");
        if (Keyboard.current.f4Key.wasPressedThisFrame) SceneManager.LoadScene("BJScene");

    }
}
