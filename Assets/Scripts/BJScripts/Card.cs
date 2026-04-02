using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Card : MonoBehaviour
// help from Bret
// and https://discussions.unity.com/t/best-way-to-program-a-deck-of-cards/491200/2 
// and https://www.w3schools.com/cpp/cpp_enum.asp 
// and https://medium.com/unity-coder-corner/unity-creating-a-card-game-ac7f46365a50
{
    public CardData cardData;
    [SerializeField]
    public SpriteRenderer frontRenderer;
    [SerializeField]
    public SpriteRenderer backRenderer;

    public void Initialize(CardData cardData)
    {
        this.cardData = cardData;
        frontRenderer.sprite = cardData.frontSprite;
        backRenderer.sprite = cardData.backSprite;
        transform.localScale = new Vector3(0.15f, 0.15f, 1.0f);
    }

    public void FlipCard()
    {
        // if (frontRenderer.enabled)
        // {
        //     frontRenderer.enabled = false;
        //     backRenderer.enabled = true;
        // } else {
        frontRenderer.enabled = true;
        backRenderer.enabled = false;
        //}   
    }
   
}

public enum CardSuit
{
    DIAMONDS = 1,
    HEARTS,
    SPADES,
    CLUBS
}

public enum CardNumber
{
    ACE = 1,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING
}

//----------GRAVEYARD----------
//----OLD CARD CONSTRUCTOR----
// public Card(CardSuit suit, CardNumber number, Vector3 position, Quaternion rotation)
    // {
    //     _cards = GameObject.FindGameObjectsWithTag ("Card");
    //     string newSuit = "";
    //     string newNum = "";
    //     string assetName = "";
    //     if (suit == CardSuit.DIAMONDS)
    //     {
    //         newSuit = "Diamond";
    //     }
    //     if (suit == CardSuit.CLUBS)
    //     {
    //         newSuit = "Club";
    //     }
    //     if (suit == CardSuit.HEARTS)
    //     {
    //         newSuit = "Heart";
    //     }
    //     if (suit == CardSuit.SPADES)
    //     {
    //         newSuit = "Spade";
    //     }
    //     if (number == CardNumber.ACE)
    //     {
    //         newNum = "A";
    //     }
    //     if (number == CardNumber.JACK)
    //     {
    //         newNum = "J";
    //     }
    //     if (number == CardNumber.QUEEN)
    //     {
    //         newNum = "Q";
    //     }
    //     if (number == CardNumber.KING)
    //     {
    //         newNum = "K";
    //     }
    //     if (newNum == "")
    //     {
    //         assetName = string.Format("Deck05_{0}_{1}", newSuit, number);  
    //     }
    //     else
    //     {
    //         assetName = string.Format("Deck05_{0}_{1}", newSuit, newNum);  
    //     }
    //     for (int i = 0; i < _cards.Length; i++)
    //     {
    //         if(_cards[i].name == assetName)
    //         {
    //             asset = _cards[i];
    //         }
    //     }
    //     _card = Instantiate(asset, position, rotation);
    //     _suit = suit;
    //     _number = number;
    // }