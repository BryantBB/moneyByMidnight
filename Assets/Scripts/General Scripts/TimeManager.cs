using UnityEngine;
using TMPro;

// received help from Ella on how to implement consistent, non destroyable time manager
// through her work in betManager
public class TimeManager : MonoBehaviour
{

    public static TimeManager Instance;
    public float time;
    public string timeString;

    [SerializeField] private TMP_Text timeText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (time == 0.0f)
        {
            time = 6.0f; // starting time is 6pm
        }
    }

    // Update is called once per frame
    public void updateTime(float timeToAdd)
    {
        time += timeToAdd;
        generateTimeString();
        displayTime();
        Debug.Log("update");

    }

    public void EndOfRoundCheck()
    {
        if (time >= 12.0f) // check if it is midnight or later, then end game and asses winnings
        {
            if (BetManager.Instance._moneytobet < 100000)
            {
                BetManager.Instance._moneytobet = 0;
                UnityEngine.SceneManagement.SceneManager.LoadScene("LOSE");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("WIN");
            }
        }
    }

    public void generateTimeString()
    {
        int hours = Mathf.FloorToInt(time);
        int minutes = Mathf.CeilToInt((time - hours) * 60);
        timeString = string.Format("{0:00}:{1:00}", hours, minutes);
        Debug.Log("Current time: " + timeString);
    }

    public void displayTime() // changing the time text box
    {
        if (timeText) timeText.text = $"Time: {timeString} PM";
        Debug.Log("display");

    }


}
