// using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public enum SlotsSound
{
    SPIN,  
    BET, 
    WIN,
    LOSE 

}

[RequireComponent(typeof(AudioSource))]
public class SlotsSoundManager : MonoBehaviour
{
    private static SlotsSoundManager instance;
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

    public static void PlaySound(SlotsSound sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

    public static void LoopSound(SlotsSound sound, float volume = 1)
    {
        instance.audioSource.clip = instance.soundList[(int)sound];
        instance.audioSource.volume = volume;
        instance.audioSource.loop = true;
        instance.audioSource.Play();
    }

    public static void StopLoop()
    {
        instance.audioSource.Stop();
        instance.audioSource.loop = false;
    }
}
