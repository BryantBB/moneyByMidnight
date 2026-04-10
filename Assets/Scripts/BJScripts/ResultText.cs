using UnityEngine;
using TMPro;
using System.Collections;

public class ResultText : MonoBehaviour
{
    public GameObject playerWon;
    public GameObject playerLost;
    public GameObject push;

    public void ShowText(string result, float duration)
    {
        if (result == "Player Won")
        {
            StartCoroutine(DisplayforTime(playerWon, duration));
        }
        if (result == "Player Lost")
        {
            StartCoroutine(DisplayforTime(playerLost, duration));
        }
        // if (result == "Push")
        // {
        //     StartCoroutine(DisplayforTime(push, duration));
        // }
    }

    public IEnumerator DisplayforTime(GameObject textbox, float duration)
    {
        textbox.SetActive(true);
        yield return new WaitForSeconds(duration);
        textbox.SetActive(false);
    }
}
