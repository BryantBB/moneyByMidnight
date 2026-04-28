using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;


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
    public GameObject casinobg;
    public GameObject pokertable;
    public GameObject sixspades;
    public GameObject sevenhearts;
    public GameObject acediamonds;
    public GameObject kingdiamonds;
    public GameObject queendiamonds;
    public GameObject twoclubs;
    public GameObject fourdiamonds;
    public GameObject jackdiamonds;
    public GameObject tendiamonds;
    public GameObject deck;
    public GameObject tenback;
    public GameObject jackback;
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
            if (sentences.Count == 13 || sentences.Count == 15 || sentences.Count == 16 || sentences.Count == 19 || sentences.Count == 23)
            {
                PokerScene();
            }
        }
    }

    public void StartDialogue()
    {
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        sentences.Clear();
        resetCards();

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
        casinobg.SetActive(true);
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

    public void PokerScene()
    {
        sceneActive = true;
        resetCharacters();
        pokertable.SetActive(true);
        sixspades.SetActive(true);
        sevenhearts.SetActive(true);
        jackback.SetActive(true);
        tenback.SetActive(true);
        deck.SetActive(true);
        if (sentences.Count <= 19)
        {
            acediamonds.SetActive(true);
            kingdiamonds.SetActive(true);
            queendiamonds.SetActive(true);
        }
        if (sentences.Count <= 16)
        {
            twoclubs.SetActive(true);
        }
        if (sentences.Count <= 15)
        {
            fourdiamonds.SetActive(true);
        }
        if (sentences.Count <= 13)
        {
            jackback.SetActive(false);
            jackdiamonds.SetActive(true);
            tenback.SetActive(false);
            tendiamonds.SetActive(true);
        }
        Invoke("setNotScene", 4.0f);
    }

    void setNotScene()
    {
        sceneActive = false;
        dialogueBox.SetActive(true);
        FixCharacter();
        resetCards();
    }
    void resetCharacters()
    {
        dialogueBox.SetActive(false);
        bubea.SetActive(false);
        margot.SetActive(false);
        hamm.SetActive(false);
        bear.SetActive(false);
    }
    void resetCards()
    {
        pokertable.SetActive(false);
        sixspades.SetActive(false);
        sevenhearts.SetActive(false);
        jackback.SetActive(false);
        tenback.SetActive(false);
        deck.SetActive(false);
        acediamonds.SetActive(false);
        kingdiamonds.SetActive(false);
        queendiamonds.SetActive(false);
        twoclubs.SetActive(false);
        fourdiamonds.SetActive(false);
        jackdiamonds.SetActive(false);
        tendiamonds.SetActive(false);
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
