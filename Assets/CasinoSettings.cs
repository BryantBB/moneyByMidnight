using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CasinoSettings : MonoBehaviour
{
    public Button RLButton;
    public Button PokerButton;
    public Button BJButton;
    public Button SlotsButton;
    
    public void RL()
    {
        SceneManager.LoadScene("RouletteScene");
    }

    public void Poker()
    {
        SceneManager.LoadScene("PokerScene");
    }

    public void Slots()
    {
        SceneManager.LoadScene("TicTacToeSlots");
    }

    public void BJ()
    {
        SceneManager.LoadScene("BJScene");
    }
}
