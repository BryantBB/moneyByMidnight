using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.XR;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Interactions;
using TMPro;

public class BlackJController : MonoBehaviour
{
    public Deck _gamedeck;
    public Button hitButton;
    public Button standButton;
    public Button dealButton;
    public Button resetButton;
    public Button closehelp;
    public ResultText resText;
    public GameObject betPanel;
    public Button placeBet;
    public TMP_InputField betInput;
    private int bet;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartBJ()
    {
        betPanel.SetActive(true);
    }

    public void StartDeal()
    {
        bet = ParseInput(betInput, 10); // adapted from josh's code
        GameObject.Find("CurrentBet").GetComponent<TextMeshProUGUI>().text = "Current Bet: " + bet.ToString();
        betPanel.SetActive(false);
        _gamedeck.ResetDeck();
    }

    private static int ParseInput(TMP_InputField field, int fallback) // adapted from josh's code
        => field != null && int.TryParse(field.text, out int v) ? v : fallback;

    public int ScoreHand(List<Card> hand)
    {
        int score = 0;
        List<CardNumber> cardNums = new List<CardNumber>();
        for (int i = hand.Count - 1; i >= 0; i--)
        {
            if (hand[i].cardData._number == CardNumber.JACK || hand[i].cardData._number == CardNumber.QUEEN || hand[i].cardData._number == CardNumber.KING)
            {
                score += 10;
            } else
            {
                score += (int)hand[i].cardData._number;
                cardNums.Add(hand[i].cardData._number);
            }
        }
        if (score <= 11 && cardNums.Contains(CardNumber.ACE))
        {
            score += 10;
        }
        Debug.Log(score);
        return score;
    }

    public string ScoreAllHands()
    {
        int playerScore = ScoreHand(_gamedeck.getPlayerHand());
        int dealerScore = ScoreHand(_gamedeck.getDealerHand());
        int dealerHit = 0;
        if(playerScore > 21)
        {
            return "Player Lost";
        }
        while (dealerScore < 17)
        {
            dealerHit += 1;
            _gamedeck.DealDealerHit(dealerHit);
            dealerScore = ScoreHand(_gamedeck.getDealerHand());
        }
        if(dealerScore > 21)
        {
            return "Player Won";
        }
        if(playerScore == 21)
        {
            return "Player Won";
        }
        if(playerScore > dealerScore)
        {
            return "Player Won";
        }
        return "Player Lost";
    }

    public void DisplayResult()
    {
        GameObject.Find("CurrentBet").GetComponent<TextMeshProUGUI>().text = "";
        string result = ScoreAllHands();
        _gamedeck.getDealerHand()[1].FlipCard();
        resText.ShowText(result,3);
        Invoke("ResetDeck",3);
    }

    public void DealAllHands()
    {
        _gamedeck.DealAllHands();
    }
    public void DealHit()
    {
        _gamedeck.DealHit();
    }

    public void ResetDeck()
    {
        _gamedeck.ResetForDeal();
    }
}




//--------GRAVEYARD----------
// public class Deck
// {
//     public List<Card> _deck;
//     public List<Card> _playerHand;
//     public List<Card> _dealerHand;
//     public List<Card> _npcHand;
//     private static System.Random shuffleRnd = new System.Random();
//     private List<Vector3> _positionList = new List<Vector3>{new Vector3(20,-60,0),new Vector3(40,60,0),new Vector3(-250,0,0),new Vector3(40,-60,0),new Vector3(20,60,0),new Vector3(-250,0,0)};
//     private List<Quaternion> _quantList = new List<Quaternion>{new Quaternion(0,0,0,0), new Quaternion(180,0,0,0), new Quaternion(0,180,0,0)};
    
//     public List<Card> getPlayerHand() { return _playerHand; }
//     public List<Card> getDealerHand() { return _dealerHand; }
//     public List<Card> getNPCHand() { return _npcHand; }
//     public GameObject[] _cards;
//     public void ResetDeck()
//     {
//         _deck = new List<Card>();
//         _cards = GameObject.FindGameObjectsWithTag("Card");
//         Card cardtoadd;
//         Card newcard;
//         for (int i = 0; i < _cards.Length; i++)
//         {
//             //card = new GameObject("CardObject");
//             // newcard = card.AddComponent<Card>();
//             newcard = new Card();
//             cardtoadd = newcard.Initialize(_cards, _cards[i].name,new Vector3(0,0,0), new Quaternion(0,0,0,0));
//             _deck.Add(cardtoadd);
//         }
//         Shuffle(_deck); //now deck is shuffled
//         Debug.Log(_deck.ToString());
//     }

    
//     public static void Shuffle(List<Card> list)
//     // Fisher-Yates shuffle from https://discussions.unity.com/t/c-poker-hands/676349/15
//     {
//         for (int i = list.Count; i > 1; i--)
//         {
//             int pos = shuffleRnd.Next(i);
//             var x = list[i-1];
//             list[i-1] = list[pos];
//             list[pos] = x;
//         }
//     }

//     public void DealHit()
//     {
//         DealPlayerCard();
//     }

//     public void DealPlayerCard()
//     {
//         _playerHand.Add(_deck[0]);
//         _deck.RemoveAt(0);
//     }

//     public void DealNPCCard()
//     {
//         _npcHand.Add(_deck[0]);
//         _deck.RemoveAt(0);
//     }

//     public void DealDealerCard()
//     {
//         _dealerHand.Add(_deck[0]);
//         _deck.RemoveAt(0);
        
//     }

//     public void DealAllHands()
//     {
//         DealPlayerCard();
//         _playerHand[0].SetPostion(_positionList[0]);
//         _playerHand[0].SetQuaternion(_quantList[0]);
//         _playerHand[0].SetActive(true);
//         DealDealerCard();
//         _dealerHand[0].SetPostion(_positionList[1]);
//         _dealerHand[0].SetQuaternion(_quantList[1]);
//         _dealerHand[0].SetActive(true);
//         DealNPCCard();
//         _npcHand[0].SetPostion(_positionList[2]);
//         _npcHand[0].SetQuaternion(_quantList[2]);
//         _npcHand[0].SetActive(true);
//         DealPlayerCard();
//         _playerHand[1].SetPostion(_positionList[3]);
//         _playerHand[1].SetQuaternion(_quantList[0]);
//         _playerHand[1].SetActive(true);
//         DealDealerCard();
//         _dealerHand[1].SetPostion(_positionList[4]);
//         _dealerHand[1].SetQuaternion(_quantList[1]);
//         _dealerHand[1].SetActive(true);
//         DealNPCCard();
//         _npcHand[1].SetPostion(_positionList[5]);
//         _npcHand[1].SetQuaternion(_quantList[2]);
//         _npcHand[1].SetActive(true);
//     }

// }
//-------OLD DECK MAKER-------
        // Card newCard;
        // CardSuit newSuit = CardSuit.DIAMONDS;
        // CardNumber newNumber = CardNumber.ACE;
        // for (int i = 4; i > 0; i--)
        // {
        //     if (i == 1)
        //     {
        //         newSuit = CardSuit.DIAMONDS;
        //     }
        //     if (i == 2)
        //     {
        //         newSuit = CardSuit.HEARTS;
        //     }
        //     if (i == 3)
        //     {
        //         newSuit = CardSuit.SPADES;
        //     }
        //     if (i == 4)
        //     {
        //         newSuit = CardSuit.SPADES;
        //     }
        //     // add one of every number of the suit to the deck
        //     for (int j = 13; j > 0; j--)
        //     {
        //         if (j == 1)
        //         {
        //             newNumber = CardNumber.ACE;   
        //         }  
        //         if (j == 2)
        //         {
        //             newNumber = CardNumber.TWO;
        //         }
        //         if (j == 3)
        //         {
        //             newNumber = CardNumber.THREE;
        //         }
        //         if (j == 4)
        //         {
        //             newNumber = CardNumber.FOUR;
        //         }
        //         if (j == 5)
        //         {
        //             newNumber = CardNumber.FIVE;
        //         }
        //         if (j == 6)
        //         {
        //             newNumber = CardNumber.SIX;
        //         }
        //         if (j == 7)
        //         {
        //             newNumber = CardNumber.SEVEN;
        //         }
        //         if (j == 8)
        //         {
        //             newNumber = CardNumber.EIGHT;
        //         }
        //         if (j == 9)
        //         {
        //             newNumber = CardNumber.NINE;
        //         }
        //         if (j == 10)
        //         {
        //             newNumber = CardNumber.TEN;
        //         }
        //         if (j == 11)
        //         {
        //             newNumber = CardNumber.JACK;
        //         }
        //         if (j == 12)
        //         {
        //             newNumber = CardNumber.QUEEN;
        //         }
        //         if (j == 13)
        //         {
        //             newNumber = CardNumber.KING;
        //         }
        //         Vector3 newPosition = new Vector3(0,0,0); //TODO Fix Postion
        //         Quaternion newRotation = new Quaternion(0,0,0,0); //TODO Fix Rotation
        //         newCard = new Card(newSuit, newNumber, newPosition, newRotation);
        //         _deck.Add(newCard);
        //     }
        
        // }
        //deck should now have A-K of each suit in the order of enum suit
        