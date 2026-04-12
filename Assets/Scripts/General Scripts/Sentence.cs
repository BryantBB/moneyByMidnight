using UnityEngine;

[System.Serializable] 
public class Sentence 
{
    public string characterName;
    [TextArea(3, 10)]
    public string text;
    public bool showCharacterSprite; 
}