using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SettingsPanelManager : MonoBehaviour, IDataPersistence
{
    private float[] speedValues = { 2f, 1.5f, 1f, 0.75f, 0.5f };

    private float hospeed = 1f;

    [Header("Prefabs")]
    public ArrowSelector arrowSelectorPrefab;
    public VolumeSlider volumeSliderPrefab;

    [Header("Parent")]
    public Transform contentParent;
    public GameObject mainPanel;

    [Header("Buttons")]
    public Button confirmButton;
    public Button exitButton;

    private ArrowSelector higherOrderSpeed;
    private ArrowSelector lowerOrderMode;
    private ArrowSelector languageSelector;

    [Header("Audio")]
    public AudioMixer audioMixer;
    private VolumeSlider masterVolume;
    private VolumeSlider sfxVolume;
    private VolumeSlider musicVolume;

    private void Start()
    {

        // Higher Order
        higherOrderSpeed = Instantiate(arrowSelectorPrefab, contentParent);
        higherOrderSpeed.Init("Adjustable Speed", new List<string> { "Slowest", "Slow", "Average", "Fast", "Fastest" }, 1);
        

        // Lower Order
        lowerOrderMode = Instantiate(arrowSelectorPrefab, contentParent);
        lowerOrderMode.Init("Mode", new List<string> { "Add Here", "Add Here", "Add Here" }, 0);

        // Language
        languageSelector = Instantiate(arrowSelectorPrefab, contentParent);
        languageSelector.Init("Language", new List<string> { "English", "Filipino"}, 0);

        // Volumes
        masterVolume = Instantiate(volumeSliderPrefab, contentParent);
        masterVolume.Init("Master Volume", 0.8f);
        masterVolume.slider.value = PlayerPrefs.GetFloat("MasterVolume");
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));

        sfxVolume = Instantiate(volumeSliderPrefab, contentParent);
        sfxVolume.Init("SFX Volume", 0.8f);
        sfxVolume.slider.value = PlayerPrefs.GetFloat("SFXVolume");
        audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));

        musicVolume = Instantiate(volumeSliderPrefab, contentParent);
        musicVolume.Init("Music Volume", 0.6f);
        musicVolume.slider.value = PlayerPrefs.GetFloat("MusicVolume");
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));

        confirmButton.onClick.AddListener(SaveSettings);
        exitButton.onClick.AddListener(OnExitSettings);
    }

    //TODO: Replace to save settings properly
    public void SaveSettings()
    {
        Debug.Log($"Higher Order Speed: {higherOrderSpeed.GetCurrentValue()}");

        string selectedSpeed = higherOrderSpeed.GetCurrentValue();

        int speedIndex = 0;
        switch (selectedSpeed)
        {
            case "Slowest": speedIndex = 0; break;
            case "Slow": speedIndex = 1; break;
            case "Average": speedIndex = 2; break;
            case "Fast": speedIndex = 3; break;
            case "Fastest": speedIndex = 4; break;
        }

        hospeed = speedValues[speedIndex];

        
        Debug.Log($"Lower Order Mode: {lowerOrderMode.GetCurrentValue()}");
        Debug.Log($"Language: {languageSelector.GetCurrentValue()}");

        audioMixer.SetFloat("MasterVolume", masterVolume.GetValue());
        PlayerPrefs.SetFloat("MasterVolume", masterVolume.GetValue());

        audioMixer.SetFloat("SFXVolume", sfxVolume.GetValue());
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume.GetValue());

        audioMixer.SetFloat("MusicVolume", musicVolume.GetValue());
        PlayerPrefs.SetFloat("MusicVolume", musicVolume.GetValue());

        Debug.Log($"Master Volume: {masterVolume.GetValue()}");
        Debug.Log($"SFX Volume: {sfxVolume.GetValue()}");
        Debug.Log($"Music Volume: {musicVolume.GetValue()}");

        DataPersistenceManager.Instance.SaveGame();
        mainPanel.SetActive(false);
    }

    public void UpdateMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void UpdateSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SaveVolume()
    {
        Debug.Log($"Master Volume: {masterVolume.GetValue()}");
        Debug.Log($"SFX Volume: {sfxVolume.GetValue()}");
        Debug.Log($"Music Volume: {musicVolume.GetValue()}");

        
    }

    public void LoadVolume()
    {
        
    }

    public void LoadData(GameData data)
    {
        hospeed = data.stageSpeed;
        Debug.Log("[StageDataLoader] Data loaded into StaticData");
    }

    //TODO: Add things to be saved here
    public void SaveData(ref GameData data)
    {
        data.stageSpeed = hospeed;
        Debug.Log("[StageDataLoader] Data saved from StaticData");
    }

    public void OnExitSettings()
    {
        mainPanel.SetActive(false);
    }
}