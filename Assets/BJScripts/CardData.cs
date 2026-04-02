using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField]
    private CardSuit _suit;
    [SerializeField]
    public CardNumber _number;

    public Sprite frontSprite;
    public Sprite backSprite;
    
}
