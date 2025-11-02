using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Music Source")]
    public AudioSource music;
    public AudioSource sfx;

    [Header("AudioClips")]
    public AudioClip idle;
    public AudioClip miss;
    public AudioClip hit;
    public AudioClip higherOrderBGM;
    public AudioClip lowerOrderBGM;

    private void Start()
    {
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));

        audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));

        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));

        if (StaticData.isOnHigherOrderGame)
        {
            music.clip = higherOrderBGM;
        }
        else if (StaticData.isOnLowerOrder)
        {
            music.clip = lowerOrderBGM;
        }
        music.loop = true;
        music.Play();
    }

    // USE THIS FOR FUTURE SFX USAGE
    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void playHitSound()
    {
        sfx.clip = hit;
        sfx.Play();
    }
    public void playMissSound()
    {
        sfx.clip = miss;
        sfx.Play();
    }
    public void playIdleSound()
    {
        sfx.clip = idle;
        sfx.Play();
    }
}
