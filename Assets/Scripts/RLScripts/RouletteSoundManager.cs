// using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public enum RouletteSound
{
    SPIN, 
    SLOW, 
    BET, 
    WIN,
    LOSE 

}

[RequireComponent(typeof(AudioSource))]
public class RouletteSoundManager : MonoBehaviour
{
    private static RouletteSoundManager instance;
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

    public static void PlaySound(RouletteSound sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

    public static void LoopSound(RouletteSound sound, float volume = 1)
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
