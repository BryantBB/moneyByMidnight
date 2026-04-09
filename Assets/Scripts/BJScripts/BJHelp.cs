using UnityEngine;
using UnityEngine.UI;

public class BJHelp : MonoBehaviour
{
    public GameObject _helpPanel;
    public Button _closeButton;
    public Button _openButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _helpPanel.SetActive(true);    
    }

    public void closeHelp()
    {
        _helpPanel.SetActive(false);
    }

    public void openHelp()
    {
        _helpPanel.SetActive(true);
    }
    
}
