using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class TPtoGames : MonoBehaviour
{
    public GameObject toPoker;
    public GameObject pokertext;
    public GameObject toBJ;
    public GameObject bjtext;
    public GameObject toSlots;
    public GameObject slotstext;
    public GameObject toRoulette;
    public GameObject roulettetext;
    public GameObject player;

    void Update()
    {
        if (Vector3.Distance(player.transform.position, toPoker.transform.position) < 10.0f)
        {
            pokertext.SetActive(true);
        } else
        {
            pokertext.SetActive(false);
        }
        if (Vector3.Distance(player.transform.position, toBJ.transform.position) < 10.0f)
        {
            bjtext.SetActive(true);
        } else
        {
            bjtext.SetActive(false);
        }
        if (Vector3.Distance(player.transform.position, toSlots.transform.position) < 10.0f)
        {
            slotstext.SetActive(true);
        } else
        {
            slotstext.SetActive(false);
        }
        if (Vector3.Distance(player.transform.position, toRoulette.transform.position) < 10.0f)
        {
            roulettetext.SetActive(true);
        } else
        {
            roulettetext.SetActive(false);
        }
        if (Vector3.Distance(player.transform.position, toPoker.transform.position) < 10.0f && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SceneManager.LoadScene("Poker");
        }
        if (Vector3.Distance(player.transform.position, toBJ.transform.position) < 10.0f && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SceneManager.LoadScene("BJScene");
        }
        if (Vector3.Distance(player.transform.position, toSlots.transform.position) < 10.0f && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SceneManager.LoadScene("Tic-Tac-Toe Slots");
        }
        if (Vector3.Distance(player.transform.position, toRoulette.transform.position) < 10.0f && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SceneManager.LoadScene("Roulette");
        }
    }
}
