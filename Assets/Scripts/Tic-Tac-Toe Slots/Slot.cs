using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slot : MonoBehaviour
{

    public bool isSpinning = false;

    private int prevIndex;
    private int currIndex;
    private int nextIndex;
    private int spinDirection;
    public int column;

    public float speed;

    private Token[] tokens = new Token[16];
    public SlotManager slotManager;
    public TokenDisplay[] tokenDisplays = new TokenDisplay[3];

    void Start()
    {
        Initialize();
        StartCoroutine(Spin());  
    }

    void Update()
    {
    }

    void Initialize()
    {
        for (int i = 0; i < tokens.Length; i++) 
            // tokens[i] = CreateToken(Random.Range(0, 8));
            tokens[i] = CreateToken(GetRandToken());
        UpdateRandCol(3, 0.1f);
        SpeedChange();
    }
    
    Token CreateToken(int id)
    {
        Token newToken = new();
        newToken.Init(id);    
        return newToken;    
    }

    void UpdateRandCol(int tokenAmount, float chance)
    {
        if (Random.value < chance)
        {
            int randomInt = GetRandToken();
            Token randomToken = CreateToken(randomInt);
            int startIndex = Random.Range(0, tokens.Length);

            for (int i = 0; i < tokenAmount; i++)
                tokens[(startIndex + i) % tokens.Length] = randomToken;
        }
    }

    void SpeedChange()
    {
        if (Random.value >= 0.8f)
        {
            if (Random.value >= 0.5f) speed *= 0.95f;
            else speed *= 1.05f;
        }
    }

    public IEnumerator Spin()
    {
        currIndex = Random.Range(0, tokens.Length);
        spinDirection = Random.value >= 0.5f ? 1 : -1;

        while(isSpinning)
        {
            yield return new WaitForSeconds(speed);
            SpeedChange();
            if (isSpinning) Shift();
            // spinDirection *= Random.value >= 0.8f ? 1 : -1;
        }
    }

    void Shift() {
        int temp = currIndex;

        currIndex = (currIndex + spinDirection + tokens.Length) % tokens.Length;
        prevIndex = temp;
        nextIndex = (currIndex + spinDirection + tokens.Length) % tokens.Length;

        UpdateDisplay(tokenDisplays[0], tokens[prevIndex]);
        UpdateDisplay(tokenDisplays[1], tokens[currIndex]);
        UpdateDisplay(tokenDisplays[2], tokens[nextIndex]); 
    }

    public Token GetPrevToken()
    {
        return tokens[prevIndex];
    }

    public Token GetCurrToken()
    {
        return tokens[currIndex];
    }

    public Token GetNextToken()
    {
        return tokens[nextIndex];
    }

    void UpdateDisplay(TokenDisplay tokenDisplay, Token token)
    {
        Sprite sprite = slotManager.sprites[token.id];
        tokenDisplay.SetSprite(sprite);
    }

    public void UpdateCheatToken(TokenDisplay tokenDisplay, Token token)
    {
    }

    public void WinDisplay()
    {
        if (tokens[prevIndex].win) tokenDisplays[0].FlashSprite(slotManager.sprites[8]);      
        if (tokens[currIndex].win) tokenDisplays[1].FlashSprite(slotManager.sprites[8]);         
        if (tokens[nextIndex].win) tokenDisplays[2].FlashSprite(slotManager.sprites[8]);
    }

    public void Reset()
    {
        ResetTokenDisplays();
        Initialize();
        isSpinning = true;
        StartCoroutine(Spin());
    }

    void ResetTokenDisplays()
    {
        tokenDisplays[0].flash = false;
        tokenDisplays[1].flash = false;
        tokenDisplays[2].flash = false;
    }

    int GetRandToken()
    {
        int[] weights = {20, 20, 20, 20, 20, 10, 5, 2};

        int totalWeight = 0;
        foreach (int w in weights) totalWeight += w;

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            currentSum += weights[i];
            if (randomValue < currentSum) return i;
        }
        return 0;
    }

}