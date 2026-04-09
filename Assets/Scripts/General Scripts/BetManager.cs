using UnityEngine;

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
        else
        {
            _moneytobet = 1000;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void updateMoneyToBet(int moneyWonOrLost)
    {
        BetManager.Instance._moneytobet += moneyWonOrLost;
    }
}
