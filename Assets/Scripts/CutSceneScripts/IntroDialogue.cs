using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class IntroDialogue : MonoBehaviour
{
    public DialogueTree tree;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject dialogueBox;
    public GameObject bubea;
    public GameObject margot;
    public GameObject bear;
    public GameObject hamm;
    public RectTransform textTransform; // To move the text for padding
    private bool sceneActive = false;
    private Queue<Sentence> sentences; // A FIFO list of lines

    public bool isDialogueActive = false;

    void Awake()
    {
        sentences = new Queue<Sentence>();
        StartDialogue();
    }

    void Update()
    {
        if (Keyboard.current == null) return; // Safety check

        if (sceneActive) return;

        if (isDialogueActive && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            DisplayNextSentence();
            FixCharacter();
        }
    }

    public void StartDialogue()
    {
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        sentences.Clear();

        foreach (Sentence sentence in tree.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
        FixCharacter();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        Sentence s = sentences.Dequeue();
        nameText.text = s.characterName;

        // Use the helper function you wrote! 
        // This is cleaner and handles both Left and Right offsets.
        float paddingValue = s.showCharacterSprite ? 25f : 0f;
        ApplyPadding(paddingValue);

        StopAllCoroutines();
        StartCoroutine(TypeSentence(s.text));
    }
    
    public void FixCharacter()
    {
        if ((nameText.text == "Bubea" || nameText.text == "Margot" || nameText.text == "Margot and Bubea") && (sentences.Count > 8))
        {
            bubea.SetActive(true);
            margot.SetActive(true);
            hamm.SetActive(false);
            bear.SetActive(false);
        }
        if ((nameText.text == "Margot") && (sentences.Count <= 8))
        {
            bubea.SetActive(false);
            margot.SetActive(true);
            hamm.SetActive(false);
            bear.SetActive(false);
        }
        if (nameText.text == "Dr. Hamm")
        {
            bubea.SetActive(false);
            margot.SetActive(false);
            hamm.SetActive(true);
            bear.SetActive(false);
        }
        if (nameText.text == "Bear Boris")
        {
            bubea.SetActive(false);
            margot.SetActive(false);
            hamm.SetActive(false);
            bear.SetActive(true);
        }
        
    }

    void ApplyPadding(float paddingAmount)
    {
        // offsetMin.x is the 'Left' property in the Inspector
        // offsetMax.x is the 'Right' property (needs to be 0 to stay glued to the right)
        textTransform.offsetMin = new Vector2(paddingAmount, textTransform.offsetMin.y);
        textTransform.offsetMax = new Vector2(0, textTransform.offsetMax.y);
    }

    IEnumerator TypeSentence(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f); // Typing speed
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
    }

}
