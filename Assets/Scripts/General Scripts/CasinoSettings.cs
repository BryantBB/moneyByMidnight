using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CasinoSettings : MonoBehaviour
{
    public Button RLButton;
    public Button PokerButton;
    public Button BJButton;
    public Button SlotsButton;

    public TMP_Text timeText;

    public static CasinoSettings Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Find and initialize the timeText component
        timeText = GetComponent<TMP_Text>();
        // Or if it's a child: timeText = GetComponentInChildren<TMP_Text>();
        // Or manually assign in inspector if neither works
    }

    public void updateTimeText(string time)
    {
        if (timeText != null)
            timeText.text = time;
    }

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