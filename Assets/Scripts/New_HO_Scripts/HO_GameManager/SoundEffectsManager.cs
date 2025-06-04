using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public AudioSource src;

    public AudioClip idle, miss, hit;
    public void playHitSound()
    {
        src.clip = hit;
        src.Play();
    }
    public void playMissSound()
    {
        src.clip = miss;
        src.Play();
    }
    public void playIdleSound()
    {
        src.clip = idle;
        src.Play();
    }
}
