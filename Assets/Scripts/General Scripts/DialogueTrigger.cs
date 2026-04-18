using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueTree tree;
    public DialogueManager manager;

    public GameObject visualPrompt; // Drag your "E" bubble/sprite here
    public float interactionDistance = 3f;
    public Transform player; // Drag your Player object here

    private bool playerInRange = false;

    void Start()
    {
        if (player == null)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    }

    void Update()
    {
        // Check distance between player and interactable
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= interactionDistance)
        {
            if (!playerInRange) ShowPrompt(true);
            
            // If they press E and dialogue isn't already running
            if (Keyboard.current.eKey.wasPressedThisFrame && !manager.isDialogueActive)
            {
                TriggerDialogue();
                ShowPrompt(false); // Hide E while talking
            }
        }
        else
        {
            if (playerInRange) ShowPrompt(false);
        }
    }

    void ShowPrompt(bool show)
    {
        playerInRange = show;
        if (visualPrompt != null) visualPrompt.SetActive(show);
    }


    public void TriggerDialogue()
        {
            manager.StartDialogue(tree);
        }
}
