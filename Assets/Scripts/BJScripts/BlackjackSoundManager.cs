// using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public enum BlackjackSound
{
    SHUFFLE, 
    DEAL, 
    BET, 
    WIN,
    LOSE 

}

[RequireComponent(typeof(AudioSource))]
public class BlackjackSoundManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static BlackjackSoundManager instance;
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

    public static void PlaySound(BlackjackSound sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
