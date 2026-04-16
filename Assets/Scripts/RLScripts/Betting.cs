using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class Betting : MonoBehaviour
{
    public GameObject chipPrefab;
    public Transform chipContainer; // Optional: A blank UI Panel to hold all chips
    public GameObject[] buttonObjects;

    public int availableMoney;
    public int totalBet = 0; // Total of bets: normal + special
    public int currentBet = 100; // Current bet

    public Button betButton;
    public TMP_Text totalBetText;
    public TMP_Text currentBetText;
    public TMP_Text moneyText;
    
    public int betOnWinner;
    public int totalToReturn;
    public int winningIndex;
    public int[] Bets = new int[32]; //0-19 = numbers 00-18, 20-22 = columns 1-3, 23-25 = 6s, 26-27 = 0-9/10-18, 28-29 = odd/even, 30-31 = red/black

    //arrays for special bets
    public int[] column1 = {3, 6, 9, 12, 15, 18};
    public int[] column2 = {2, 5, 8, 11, 14, 17};
    public int[] column3 = {1, 4, 7, 10, 13, 16};
    public int[] black = {2, 4, 6, 8, 10, 11, 13, 15, 17};
    public int[] red = {1, 3, 5, 7, 9, 12, 14, 16, 18};

    // Update is called once per frame
    void Update()
    {
        ButtonsPressed();
        totalBetText.text = $"Total Placed Bets: <color=white>{totalBet}</color>";
        currentBetText.text = $"Current Bet: <color=white>{currentBet}</color>";
        moneyText.text = $"Money: <color=white>{BetManager.Instance._moneytobet}</color>";

        betButton.interactable = totalBet > 0;
    }
    
    // Checks if any of the keyboard buttons related to betting have been pressed and updates the bet
    public void ButtonsPressed()
    {
        int availableMoney = BetManager.Instance._moneytobet - totalBet;
        if (Keyboard.current.digit1Key.wasPressedThisFrame && availableMoney >= currentBet + 100)
        {
            currentBet += 100;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && availableMoney >= currentBet + 200)
        {
            currentBet += 200;
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && availableMoney >= currentBet + 300)
        {
            currentBet += 300;
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            currentBet = availableMoney;
        }
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            currentBet = 100; // reset bet to minimum
        }
    }


    public void PlaceBet(int boardIndex)
    {
        // Check if the player has enough money first!
        if (currentBet > 0 && totalBet + currentBet <= BetManager.Instance._moneytobet)
        {
            Bets[boardIndex] += currentBet;
            totalBet += currentBet;

            SpawnChip(boardIndex);
            Debug.Log($"Bet placed on {boardIndex}");

            Debug.Log("Full Board Bets: " + string.Join(", ", Bets));

            // --- SAFETY CHECK ---
            // If we just bet so much that we can't afford our current chip anymore, lower the currentBet automatically.
            int available = BetManager.Instance._moneytobet - totalBet;
            if (available < currentBet)
            {
                // If they can't afford the current bet, drop it to the available amount or 100 (minimum), or 0 if they are totally broke.
                currentBet = available; 
            }
        }

        if (currentBet == 0)
        {
            Debug.Log("Not enough money!");
        }
    }


    public void ClearAllBets()
    {
        Array.Clear(Bets, 0, Bets.Length);
        availableMoney = BetManager.Instance._moneytobet;
        totalBet = 0;
        currentBet = 100;

        ClearAllVisualChips();
    }


    public void ResolveSpin(string winnerName)
    {
        int winNum = GetIndexFromName(winnerName);
        if (winNum == -1) return; // Error safety

        int roundWinnings = 0;

        // 1. Check Number bets (Indices 0-19)
        // Payout is 21x
        if (Bets[winNum] > 0)
        {
            roundWinnings += Bets[winNum] * 21;
        }

        // 2. Check Special bets (Indices 20-31)
        if (winNum >= 1 && winNum <= 18) // Columns/Specials don't apply to 0/00
        {
            // 2. Check Columns (Indices 20-22) 
            // Payout is 3x
            if (Bets[20] > 0 && column1.Contains(winNum)) roundWinnings += Bets[20] * 3;
            if (Bets[21] > 0 && column2.Contains(winNum)) roundWinnings += Bets[21] * 3;
            if (Bets[22] > 0 && column3.Contains(winNum)) roundWinnings += Bets[22] * 3;

            // 3. Check 6s (Indices 23-25: 1-6, 7-12, 13-18)
            // Payout is 3x
            if (Bets[23] > 0 && winNum >= 1 && winNum <= 6)   roundWinnings += Bets[23] * 3;
            if (Bets[24] > 0 && winNum >= 7 && winNum <= 12)  roundWinnings += Bets[24] * 3;
            if (Bets[25] > 0 && winNum >= 13 && winNum <= 18) roundWinnings += Bets[25] * 3;

            // 4. Check Halves (Indices 26-27: 1-9, 10-18)
            // Payout is 2x
            if (Bets[26] > 0 && winNum >= 1 && winNum <= 9)   roundWinnings += Bets[26] * 2;
            if (Bets[27] > 0 && winNum >= 10 && winNum <= 18) roundWinnings += Bets[27] * 2;

            // 5. Check Odd/Even (Indices 28-29)
            // Payout is 2x
            bool isEven = winNum % 2 == 0;
            if (Bets[28] > 0 && !isEven) roundWinnings += Bets[28] * 2; // Odd
            if (Bets[29] > 0 && isEven)  roundWinnings += Bets[29] * 2; // Even

            // 6. Check Red/Black (Indices 30-31)
            // Payout is 2x
            if (Bets[30] > 0 && red.Contains(winNum))   roundWinnings += Bets[30] * 2;
            if (Bets[31] > 0 && black.Contains(winNum)) roundWinnings += Bets[31] * 2;
        }

        // FINAL RESOLUTION
        BetManager.Instance.updateMoneyToBet(-totalBet); // Subtract what was risked
        BetManager.Instance.updateMoneyToBet(roundWinnings); // Add what was won

        if (roundWinnings > 0)
            Debug.Log($"<color=green>WIN!</color> Landed on {winnerName}. Total Payout: {roundWinnings}");
        else
            Debug.Log($"<color=red>LOSS.</color> Landed on {winnerName}. Total Lost: {totalBet}");
        
        ClearAllBets();
    }

    private int GetIndexFromName(string name)
    {
        if (name == "00") return 19;
        
        if (int.TryParse(name, out int result))
        {
            return result;
        }
        return -1; 
    }


    // written with help from Gemini
    void SpawnChip(int index)
    {
        if (chipPrefab == null || buttonObjects[index] == null || chipContainer == null) return;

        // 1. Spawn the chip inside the Global Container (NOT the button)
        // This ensures it uses the Canvas scale, which is the same for everyone
        GameObject newChip = Instantiate(chipPrefab, chipContainer);

        // 2. Force the chip to be the exact size you want in pixels
        RectTransform chipRect = newChip.GetComponent<RectTransform>();
        
        // Set these to your preferred chip size
        chipRect.sizeDelta = new Vector2(40, 40); 
        chipRect.localScale = Vector3.one;

        // 3. Move the chip to the BUTTON'S position
        // Since they are both in the UI/Canvas, transform.position works great
        newChip.transform.position = buttonObjects[index].transform.position;

        // 4. Add the random "poker stack" jitter
        float randomOffset = 8f;
        chipRect.anchoredPosition += new Vector2(
            UnityEngine.Random.Range(-randomOffset, randomOffset),
            UnityEngine.Random.Range(-randomOffset, randomOffset)
        );
    }

    public void ClearAllVisualChips()
    {
        // Simply destroy everything inside the dedicated chip folder
        foreach (Transform child in chipContainer)
        {
            Destroy(child.gameObject);
        }
    }   
}
