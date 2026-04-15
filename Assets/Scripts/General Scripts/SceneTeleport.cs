using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;


public class SceneTeleport : MonoBehaviour
{
    public bool titleScreen;

    void Start()
    {   
    }

    void Update()
    {        
        if (titleScreen && Keyboard.current.enterKey.wasPressedThisFrame) SceneManager.LoadScene("Casino");
        if (Keyboard.current.f1Key.wasPressedThisFrame) SceneManager.LoadScene("Casino");
        if (Keyboard.current.f2Key.wasPressedThisFrame) SceneManager.LoadScene("Tic-Tac-Toe Slots");
        if (Keyboard.current.f3Key.wasPressedThisFrame) SceneManager.LoadScene("Roulette");
        if (Keyboard.current.f4Key.wasPressedThisFrame) SceneManager.LoadScene("BJScene");
        if (Keyboard.current.f5Key.wasPressedThisFrame) SceneManager.LoadScene("Poker");
        if (Keyboard.current.f6Key.wasPressedThisFrame) SceneManager.LoadScene("Title");
    }
}
