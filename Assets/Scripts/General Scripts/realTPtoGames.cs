using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class realTPtoGames : MonoBehaviour
{
    public GameObject gametext;
    public GameObject player;
    public string gamename;
    public bool onbox = false;

    void Update()
    {
        if (onbox && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SceneManager.LoadScene(gamename);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == player.name) {
            gametext.SetActive(true);
            onbox = true;
        }
    }
}
