using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueTree", menuName = "Dialogue/Tree")]
public class DialogueTree : ScriptableObject
{
    // Now you can have a list of your Sentence class!
    public Sentence[] sentences;
}
