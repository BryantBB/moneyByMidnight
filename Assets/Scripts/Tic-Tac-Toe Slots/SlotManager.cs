using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class SlotManager : MonoBehaviour
{

    public Slot[] slots;
    private Token[] slotResults = new Token[9];
    private List<int> payouts = new List<int>();

    private int nextSlot = 0;
    private int lives = 9;
    private int totalPayout = 0;

    public int[] payoutAmounts = new int[8];
    public string[] tokenNames = new string[8];
    public Sprite[] sprites = new Sprite[9];
    public TokenDisplay[] cheatTokenDisplays = new TokenDisplay[3];

    public bool cheatEnabled;

    private bool cheatMode = false;    
    private bool canPlay = false;

    public TMP_Text livesText;
    public TMP_Text payoutText;
    public SlotButton slotButton;

    void Start()
    {
        UpdateText(livesText, lives);
        UpdateText(payoutText, 0);
    }

    void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame) Restart();
    }
    
    public void StopSlot()
    {
        if (lives > 0) {
            if (!canPlay)
            {
                RoundReset();
                return;
            }
            if (nextSlot < slots.Length)
            {
                slots[nextSlot].isSpinning = false;
                nextSlot++;
                if (nextSlot >= slots.Length)
                {
                    if (cheatEnabled) Cheat();
                    RoundEnd();                
                }
            }
        }
    }

    void Cheat()
    {
        if (cheatMode)
        {
            // slotButton.Disable();
            // Token[] cheatTokens = GenerateCheatTokens();
            // SetCheatTokenDisplays(cheatTokens);
            // flashCheatTokenDisplays(cheatTokens);
            // slotButton.Enable();
            
            // cheatTokens = slotCheater.GenerateCheatTokens();
            // cheatMode = false;
            // GenerateCheatTokens();         
            // print("Shift");
            // foreach (Slot s in slots)
            // {
            //     if (Random.value <= shiftChance)
            //         s.Shift();
            // }
            // canCheat = false;
        }
    }

    Token[] GenerateCheatTokens()
    {
        Token[] cheatTokens = new Token[3];
        for (int i = 0; i < cheatTokens.Length; i++)
        {
            int randInt = Random.Range(0, 8);
            Token newToken = new();
            newToken.Init(randInt);
            cheatTokens[i] = newToken;           
        }
        return cheatTokens;
    }

    void SetCheatTokenDisplays(Token[] cheatTokens)
    {
        int xPos = -3;
        for (int i = 0; i < cheatTokens.Length; i++) {
            int cheatTokenId = cheatTokens[i].id;
            Sprite cheatSprite = sprites[cheatTokenId];
            cheatTokenDisplays[i].SetSprite(cheatSprite);
            cheatTokenDisplays[i].transform.position = new Vector3(xPos, -4, -1);
            xPos += 3;
        }
    }

    void flashCheatTokenDisplays(Token[] cheatTokens)
    {
        for (int i = 0; i < cheatTokens.Length; i++)
        {
            cheatTokenDisplays[i].FlashWhite();
            // yield return new WaitForSeconds(0.16f);
        }
    }

    void RoundReset()
    {
        canPlay = true;
        if (cheatEnabled) cheatMode = true;
        nextSlot = 0;
        // shiftChance += 0.1f;
        foreach (Slot s in slots) s.Reset();
        payouts.Clear();
        // UpdateText(payoutText, 0.ToString());
    } 
    
    void RoundEnd()
    {
        FillResults();
        CalcWins();
        WinDisplays();
        int payout = GetPayout();
        UpdateTotalPayout(payout);
        if (payout <= 0)
        {
            lives -= 1;  
            UpdateText(livesText, lives);
        }
        if (lives <= 0) slotButton.Disable();    
        canPlay = false;
    }

    int GetPayout()
    {
        int payout = 0;
        foreach (int i in payouts) payout += i;
        return payout;
    }

    void UpdateTotalPayout(int payout)
    {
        totalPayout += payout;
        UpdateText(payoutText, totalPayout);
    }  

    void FillResults()
    {
        int i = 0;
        foreach (Slot s in slots)
        {
            slotResults[i] = s.GetPrevToken();
            slotResults[i + 1] = s.GetCurrToken();
            slotResults[i + 2] = s.GetNextToken();
            i += 3;
        }
    }

    void CalcWins()
    {
        if (payouts.Count != 0) payouts.Clear();

        for (int i = 0; i < 3; i++)
        {
            int horzWin = CalcHorzWin(i);
            if (horzWin != -1) payouts.Add(payoutAmounts[horzWin]);
            if (i != 1)
            {
                int diagWin = CalcDiagWin(i);
                if (diagWin != -1)
                    payouts.Add(payoutAmounts[diagWin]);
            }
        }

        for (int i = 0; i < 7; i += 3)
        {
            int vertWin = CalcVertWin(i);
            if (vertWin != -1) payouts.Add(payoutAmounts[vertWin]);

        }

        int rombusWin = CalcRombusWin();
        if (rombusWin != -1) payouts.Add(payoutAmounts[rombusWin]);
    }

    int CalcHorzWin(int i)
    {
        Token first = slotResults[i];   
        Token second = slotResults[i + 3];
        Token third = slotResults[i + 6];
        return CheckWin(new[] {first, second, third}) ? first.id : -1;
    }

    int CalcVertWin(int i)
    {
        Token first = slotResults[i];
        Token second = slotResults[i + 1];
        Token third = slotResults[i + 2];
        return CheckWin(new[] {first, second, third}) ? first.id : -1;
    }

    int CalcDiagWin(int i)
    {
        int step = i > 0 ? 2 : 4;
        Token first = slotResults[i];   
        Token second = slotResults[i + step];
        Token third = slotResults[i + (step * 2)];
        return CheckWin(new[] {first, second, third}) ? first.id : -1; 
    }

    int CalcRombusWin()
    {
        Token first = slotResults[1];
        Token second = slotResults[3];
        Token third = slotResults[7];
        Token fourth = slotResults[5];
        return CheckWin(new[] {first, second, third, fourth}) ? first.id : -1;
    }

    bool CheckWin(Token[] tokens)
    {
        for (int i = 0; i < tokens.Length - 1; i++)
            if (tokens[i].id != tokens[i + 1].id)
                return false;
        for (int i = 0; i < tokens.Length; i++) tokens[i].win = true;
        return true;
    }

    void UpdateText<T>(TMP_Text textMashPro, T value)
    {
        textMashPro.text = value.ToString();
    }

    void WinDisplays()
    {
        foreach (Slot s in slots) s.WinDisplay();
    }

    void Restart()
    {
        lives = 9;
        UpdateText(livesText, lives);
        totalPayout = 0;
        UpdateText(payoutText, totalPayout);
        slotButton.Enable();
    }

}