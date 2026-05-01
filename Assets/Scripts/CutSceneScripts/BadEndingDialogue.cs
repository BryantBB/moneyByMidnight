using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using NUnit.Framework;

public class BadEndingDialogue : MonoBehaviour
{
    public DialogueTree tree;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject dialogueBox;
    public GameObject margot;
    public GameObject hammscare;
    public GameObject hamm;
    public RectTransform textTransform; // To move the text for padding
    private bool sceneActive = false;
    private Queue<Sentence> sentences; // A FIFO list of lines

    public bool isDialogueActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        dialogueBox.SetActive(true);
        isDialogueActive = true;
        sceneActive = false;
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
            SceneManager.LoadScene("Title");
            return;
        }
        isDialogueActive = true;
        Sentence s = sentences.Dequeue();
        nameText.text = s.characterName;

        // Use the helper function you wrote! 
        // This is cleaner and handles both Left and Right offsets.
        float paddingValue = s.showCharacterSprite ? 25f : 0f;
        ApplyPadding(paddingValue);

        StopAllCoroutines();
        StartCoroutine(TypeSentence(s.text));
        if (sentences.Count == 1)
        {
            sceneActive = true;
            isDialogueActive = false;
            Invoke("jumpscare", 2.0f);
        }
    }

    public void jumpscare()
    {
        dialogueBox.SetActive(false);
        margot.SetActive(false);
        hamm.SetActive(false);
        hammscare.SetActive(true);
        Invoke("endScare",2.0f);
    }

    public void endScare()
    {
        dialogueBox.SetActive(true);
        hammscare.SetActive(false);
        DisplayNextSentence();
        FixCharacter();
    }
    public void FixCharacter()
    {
        if (nameText.text == "Margot")
        {
            hammscare.SetActive(false);
            margot.SetActive(true);
            hamm.SetActive(false);
        }
        if (nameText.text == "Dr. Hamm")
        {
            margot.SetActive(false);
            hamm.SetActive(true);
            hammscare.SetActive(false);
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
        dialogueBox.SetActive(false);
        isDialogueActive = false;
    }
}
