using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

public class Deck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<Card> _deck = new List<Card>();
    private List<Card> _playerHand = new List<Card>();
    private List<Card> _dealerHand = new List<Card>();
    private List<Card> _npcHand = new List<Card>();
    private static System.Random shuffleRnd = new System.Random();
    
    public List<Card> getPlayerHand() { return _playerHand; }
    public List<Card> getDealerHand() { return _dealerHand; }
    public List<Card> getNPCHand() { return _npcHand; }
    public List<CardData> _cards;
    public GameObject cardPrefab;
    private int hitCount;
    public void ResetDeck()
    {
        hitCount = 0;
        for (int i = 0; i < _cards.Count; i++)
        {
            Debug.Log("Card i="+i);
            GameObject cardGO = Instantiate(cardPrefab, new Vector3(5,0,0), Quaternion.identity);
            Card card = cardGO.GetComponent<Card>();
            card.Initialize(_cards[i]);
            _deck.Add(card);
        }
        Shuffle(_deck); //now deck is shuffled
    }

    public void ResetForDeal()
    {
        for(int i = 0; i <= _playerHand.Count; i++)
        {
            _playerHand[0].transform.position = new Vector3(1000,1000,-1000);
            _playerHand.RemoveAt(0);
        }
        for(int i = 0; i <= _npcHand.Count; i++)
        {
            _npcHand[0].transform.position = new Vector3(1000,1000,-1000);
            _npcHand.RemoveAt(0);
        }
        for(int i = 0; i <= _dealerHand.Count; i++)
        {
            _dealerHand[0].transform.position = new Vector3(1000,1000,-1000);
            _dealerHand.RemoveAt(0);
        }
        hitCount = 0;
    }


    public static void Shuffle(List<Card> list)
    // Fisher-Yates shuffle from https://discussions.unity.com/t/c-poker-hands/676349/15
    {
        for (int i = list.Count; i > 1; i--)
        {
            int pos = shuffleRnd.Next(i);
            var x = list[i-1];
            list[i-1] = list[pos];
            list[pos] = x;
        }
    }

    public void DealHit()
    {
        hitCount += 1;
        DealPlayerCard();
        if (hitCount == 1){
            _playerHand[hitCount+1].transform.position = new Vector3(hitCount+2,-3,0);
        } else
        {    
        _playerHand[hitCount+1].transform.position = new Vector3(hitCount+2,-3,0);
        }
        _playerHand[hitCount+1].FlipCard();
    }

    public void DealPlayerCard()
    {
        _playerHand.Add(_deck[0]);
        _deck.RemoveAt(0);
    }

    public void DealNPCCard()
    {
        _npcHand.Add(_deck[0]);
        _deck.RemoveAt(0);
    }

    public void DealDealerCard()
    {
        _dealerHand.Add(_deck[0]);
        _deck.RemoveAt(0);
    }

    public void DealAllHands()
    {
        hitCount = 0;
        DealNPCCard();
        _npcHand[0].transform.position = new Vector3(-3,0,0);
        _npcHand[0].FlipCard();
        DealPlayerCard();
        _playerHand[0].transform.position = new Vector3(-1,-3,0);
        _playerHand[0].FlipCard();
        DealDealerCard();
        _dealerHand[0].transform.position = new Vector3(0,3,0);
        _dealerHand[0].FlipCard();
        DealNPCCard();
        _npcHand[1].transform.position = new Vector3(-3,1,0);
        DealPlayerCard();
        _playerHand[1].transform.position = new Vector3(1,-3,0);
        _playerHand[1].FlipCard();
        DealDealerCard();
        _dealerHand[1].transform.position = new Vector3(2,3,0);

        Debug.Log("player hand = " + _playerHand);
        Debug.Log("dealer hand = " + _dealerHand);
        Debug.Log("npc hand = " + _npcHand);
    }
}

// ----- Graveyard ------
        // Card cardtoadd;
        // Card newcard;
        // for (int i = 0; i < _cards.Count; i++)
        // {
        //     //card = new GameObject("CardObject");
        //     // newcard = card.AddComponent<Card>();
        //     newcard = new Card();
        //     cardtoadd = newcard.Initialize(_cards, _cards[i].name,new Vector3(0,0,0), new Quaternion(0,0,0,0));
        //     _deck.Add(cardtoadd);
        // }
        // for (int i = 0; i < _deck.Count; i++)
        // {
        //     _deck[i].resetCard();
        // }
        
        //Debug.Log(_deck.ToString());

    // TODO - Fix
    // private List<Vector3> _positionList = new List<Vector3>{new Vector3(20,-60,0),new Vector3(40,60,0),new Vector3(-250,0,0),new Vector3(40,-60,0),new Vector3(20,60,0),new Vector3(-250,0,0)};
    // private List<Quaternion> _quantList = new List<Quaternion>{new Quaternion(0,0,0,0), new Quaternion(180,0,0,0), new Quaternion(0,180,0,0)};
    //     DealPlayerCard();
    //     _playerHand[0].SetPostion(_positionList[0]);
    //     _playerHand[0].SetQuaternion(_quantList[0]);
    //     _playerHand[0].SetActive(true);
    //     DealDealerCard();
    //     _dealerHand[0].SetPostion(_positionList[1]);
    //     _dealerHand[0].SetQuaternion(_quantList[1]);
    //     _dealerHand[0].SetActive(true);
    //     DealNPCCard();
    //     _npcHand[0].SetPostion(_positionList[2]);
    //     _npcHand[0].SetQuaternion(_quantList[2]);
    //     _npcHand[0].SetActive(true);
    //     DealPlayerCard();
    //     _playerHand[1].SetPostion(_positionList[3]);
    //     _playerHand[1].SetQuaternion(_quantList[0]);
    //     _playerHand[1].SetActive(true);
    //     DealDealerCard();
    //     _dealerHand[1].SetPostion(_positionList[4]);
    //     _dealerHand[1].SetQuaternion(_quantList[1]);
    //     _dealerHand[1].SetActive(true);
    //     DealNPCCard();
    //     _npcHand[1].SetPostion(_positionList[5]);
    //     _npcHand[1].SetQuaternion(_quantList[2]);
    //     _npcHand[1].SetActive(true);