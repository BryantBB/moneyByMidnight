using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public enum PokerSound
{
    SHUFFLE, 
    DEAL, 
    BET, 
    WIN,
    LOSE 

}

[RequireComponent(typeof(AudioSource))]
public class PokerSoundManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static PokerSoundManager instance;
    private AudioSource audioSource;

    [SerializeField] private AudioClip[] soundList;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(PokerSound sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
