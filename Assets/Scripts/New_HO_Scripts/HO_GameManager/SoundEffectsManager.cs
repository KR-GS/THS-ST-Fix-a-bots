using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Music Source")]
    public AudioSource music;
    public AudioSource sfx;

    [Header("SFX Clips")]
    public AudioClip idle;
    public AudioClip miss;
    public AudioClip hit;

    public AudioClip happy;

    public AudioClip falling;
    public AudioClip flats;
    public AudioClip hammer;
    public AudioClip phillips;
    public AudioClip snip;
    public AudioClip staticElectric;
    public AudioClip stickerPop;
    public AudioClip swing;
    public AudioClip tennis;
    public AudioClip wrench;


    [Header("BGM Clips")]
    public AudioClip higherOrderBGM;
    public AudioClip lowerOrderBGM;
    public AudioClip titleScreenBGM;

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
        else
        {
            music.clip = titleScreenBGM;
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
        sfx.PlayOneShot(hit);
    }

    public void playHappy()
    {
        sfx.PlayOneShot(happy);
    }

    public void playMissSound()
    {
        sfx.PlayOneShot(miss);
    }
    public void playIdleSound()
    {
        sfx.PlayOneShot(idle);
    }
    public void playFallingSounds()
    {
        sfx.PlayOneShot(falling);
    }
    public void playFlatsSounds()
    {
        sfx.PlayOneShot(flats);
    }
    public void playHammerSounds()
    {
        sfx.PlayOneShot(hammer);
    }
    public void playPhillipsSounds()
    {
        sfx.PlayOneShot(phillips);
    }
    public void playSnipSounds()
    {
        sfx.PlayOneShot(snip);
    }
    public void playStaticSounds()
    {
        sfx.clip = staticElectric;
        sfx.loop = true;
        sfx.Play();
    }

    public void stopStaticSounds()
    {
        sfx.clip = staticElectric;
        sfx.loop = false;
        sfx.Stop();
    }

    public void playStickerSounds()
    {
        sfx.PlayOneShot(stickerPop);
    }

    public void playTennisSounds()
    {
        sfx.PlayOneShot(tennis);
    }

    public void playSwingSounds()
    {
        sfx.PlayOneShot(swing);
    }

    public void playWrenchSounds()
    {
        sfx.PlayOneShot(wrench);
    }
}
