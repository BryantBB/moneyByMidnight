using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public static TimeManager Instance;
    public float _time;
    public string timeString;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_time == 0.0f)
        {
            _time = 6.0f; // starting time is 6pm
        }
    }

    // Update is called once per frame
    public void updateTime(float timeToAdd)
    {
        _time += timeToAdd;
        displayTime();
    }

    public void EndOfRoundCheck()
    {
        if (_time == 12.0f)
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

    public void displayTime()
    {
        int hours = Mathf.FloorToInt(_time);
        int minutes = Mathf.FloorToInt((_time - hours) * 60);
        string timeString = string.Format("{0:00}:{1:00}", hours, minutes);
        this.timeString = timeString;
        Debug.Log("Current time: " + timeString);

        // Update the UI
        if (CasinoSettings.Instance != null)
            CasinoSettings.Instance.updateTimeText(timeString);
    }
}
