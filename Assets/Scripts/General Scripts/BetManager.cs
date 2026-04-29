using UnityEngine;
using UnityEngine.SceneManagement;

public class BetManager : MonoBehaviour
{
    // with help from https://learn.unity.com/tutorial/implement-data-persistence-between-scenes
    public static BetManager Instance;
    public int _moneytobet;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // should only give starting money at the beginning and not resetting each time a scene loads
        if (_moneytobet == 0)
        {
            _moneytobet = 1000;
        }
        
    }

    public void updateMoneyToBet(int moneyWonOrLost)
    {
        BetManager.Instance._moneytobet += moneyWonOrLost;
        Debug.Log("Money changed!!! New total: " + _moneytobet);
    }

    public void EndOfRoundCheck()
    {
        if (_moneytobet >= 100000)
        {
            SceneManager.LoadScene("WIN");
        }
        else if (_moneytobet <= 0)
        {
            _moneytobet = 0;
            SceneManager.LoadScene("LOSE");
        }
    // TODO: IMPLEMENT TIME CONDITION LOSS
    //       else if (_moneytobet < 100000 & time == endOfDay)
    //     {
    //         SceneManager.LoadScene("LOSE");
    //     }
    }

}
