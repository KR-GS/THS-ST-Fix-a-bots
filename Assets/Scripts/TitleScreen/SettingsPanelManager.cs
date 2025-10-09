using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class SettingsPanelManager : MonoBehaviour, IDataPersistence
{
    private float[] speedValues = { 2f, 1.5f, 1f, 0.75f, 0.5f };

    private float hoSpeed = 1f;

    private int settingsState = 0;


    [Header("Prefabs")]
    public ArrowSelector arrowSelectorPrefab;
    public VolumeSlider volumeSliderPrefab;

    [Header("Parent")]
    public Transform contentParent;
    public GameObject mainPanel;

    [Header("Buttons")]
    public Button confirmButton;
    public Button exitButton;
    public Button mainMenuButton;

    [Header("Settings Tabs")]

    public Button generalButton;
    public Button lowerOrderButton;
    public Button higherOrderButton;

    private ArrowSelector higherOrderSpeed;
    private ArrowSelector lowerOrderMode;
    private ArrowSelector languageSelector;

    [Header("Audio")]
    public AudioMixer audioMixer;
    private VolumeSlider masterVolume;
    private VolumeSlider sfxVolume;
    private VolumeSlider musicVolume;

    [Header("Backing Values")]
    private string selectedLanguage = "English" ;
    private float masterVolumeValue = 0;
    private float sfxVolumeValue = 0;
    private float musicVolumeValue = 0;
    private int loModeIndex;

    [Header("Old Values")]
    private  float oldHoSpeed = 1f;
    private string oldSelectedLanguage = "English";
    private float oldMasterVolumeValue = 0;
    private float oldSFXVolumeValue = 0;
    private float oldMusicVolumeValue = 0;

    public string getSelectedLanguage()
    {
        return selectedLanguage;
    }

    public float getSelectedStageSpeed()
    {
        return hoSpeed;
    }
    

    private void Awake()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolumeValue = PlayerPrefs.GetFloat("MasterVolume");
            oldMasterVolumeValue = masterVolumeValue;
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolumeValue = PlayerPrefs.GetFloat("SFXVolume");
            oldSFXVolumeValue = sfxVolumeValue;
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeValue = PlayerPrefs.GetFloat("MusicValue");
            oldMusicVolumeValue = musicVolumeValue;
        }

    }

    private void Start()
    {
        generalButton.gameObject.SetActive(true);
        lowerOrderButton.gameObject.SetActive(true);
        higherOrderButton.gameObject.SetActive(true);
        //If on title screen
        if (!StaticData.isOnHigherOrder && !StaticData.isOnLowerOrder)
        {
            mainMenuButton.gameObject.SetActive(false);
        }
        //if on Lower order part
        else if (StaticData.isOnLowerOrder)
        {
            higherOrderButton.gameObject.SetActive(false);
        }
        // if on Higher order part
        else if (StaticData.isOnHigherOrder)
        {
            lowerOrderButton.gameObject.SetActive(false);
        }

        ShowGeneralSettings();

        generalButton.onClick.AddListener(() =>
        {
            ChangeSettingsState();
            ShowGeneralSettings();
        });
        higherOrderButton.onClick.AddListener(() =>
        {
            ChangeSettingsState();
            ShowHOSettings();
        });
        lowerOrderButton.onClick.AddListener(() =>
        {
            ChangeSettingsState();
            ShowLOSettings();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            BackToMainMenu();
            Time.timeScale = 1;
        });

        confirmButton.onClick.AddListener(() =>
        {
            SaveSettings();
            Time.timeScale = 1;
        });
        exitButton.onClick.AddListener(() =>
        {
            OnExitSettings();
            Time.timeScale = 1;
        });
    }

    public void SaveSettings()
    {
        switch (settingsState)
        {
            // General Settings
            case 0:
                selectedLanguage = languageSelector.GetCurrentValue();

                if(selectedLanguage == "English")
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                else if(selectedLanguage == "Filipino")
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];

                masterVolumeValue = masterVolume.GetValue();
                sfxVolumeValue = sfxVolume.GetValue();
                musicVolumeValue = musicVolume.GetValue();

                audioMixer.SetFloat("MasterVolume", masterVolumeValue);
                PlayerPrefs.SetFloat("MasterVolume", masterVolumeValue);

                audioMixer.SetFloat("SFXVolume", sfxVolumeValue);
                PlayerPrefs.SetFloat("SFXVolume", sfxVolumeValue);

                audioMixer.SetFloat("MusicVolume", musicVolumeValue);
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeValue);
                break;

            // HO Settings
            case 1:
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

                hoSpeed = speedValues[speedIndex];
                
                break;

            // LO Settings
            case 2:
                
                break;
        }
        
        ShowGeneralSettings();
        DataPersistenceManager.Instance.SaveGame();
        mainPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        if (StaticData.isOnHigherOrderGame)
            SceneManager.LoadScene("Stage_Select");
        else if (StaticData.isOnLowerOrderGame)
            SceneManager.LoadScene("LO_WS2D");
        else if (!StaticData.isOnHigherOrderGame && !StaticData.isOnLowerOrderGame)
        {
            SceneManager.LoadScene("Title_Screen");
        }
    }

    public void ChangeSettingsState()
    {
        switch (settingsState)
        {
            // General Settings
            case 0:
                selectedLanguage = languageSelector.GetCurrentValue();
                masterVolumeValue = masterVolume.GetValue();
                sfxVolumeValue = sfxVolume.GetValue();
                musicVolumeValue = musicVolume.GetValue();
                break;

            // HO Settings
            case 1:
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

                hoSpeed = speedValues[speedIndex];

                break;

            // LO Settings
            case 2:

                break;
        }
    }

    public void UpdateMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        masterVolumeValue = masterVolume.GetValue();
    }

    public void UpdateSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        sfxVolumeValue = sfxVolume.GetValue();
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        musicVolumeValue = musicVolume.GetValue();
    }

    public void LoadData(GameData data)
    {
        hoSpeed = data.stageSpeed;
        selectedLanguage = data.language;

        oldHoSpeed = hoSpeed;
        oldSelectedLanguage = selectedLanguage;

    }

    //TODO: Add things to be saved here
    public void SaveData(ref GameData data)
    {
        //GENERAL SETTINGS
        oldSelectedLanguage = selectedLanguage;
        oldMasterVolumeValue = masterVolumeValue;
        oldSFXVolumeValue = sfxVolumeValue;
        oldMusicVolumeValue = musicVolumeValue;

        //HO SETTINGS
        oldHoSpeed = hoSpeed;

        //LO SETTINGS

        Debug.Log("Speed is: " + hoSpeed);


        data.stageSpeed = hoSpeed;
        StaticData.cycleInterval = hoSpeed;
        data.language = selectedLanguage;
    }

    public void OnExitSettings()
    {
        //GENERAL SETTINGS
        selectedLanguage = oldSelectedLanguage;
        masterVolumeValue = oldMasterVolumeValue;
        sfxVolumeValue = oldSFXVolumeValue;
        musicVolumeValue = oldMusicVolumeValue;

        if(oldSelectedLanguage == "English")
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        else if(oldSelectedLanguage == "Filipino")
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];

        //HO SETTINGS
        hoSpeed = oldHoSpeed;

        //LO SETTINGS

        ShowGeneralSettings();

        Debug.Log("Settings Panel has Exited");
        mainPanel.SetActive(false);
    }

    private void ShowGeneralSettings()
    {
        settingsState = 0;
        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Language
        languageSelector = Instantiate(arrowSelectorPrefab, contentParent);
        switch (selectedLanguage)
        {
            case "English":
                languageSelector.Init("Language", new List<string> { "English", "Filipino" }, 0);
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
            case "Filipino":
                languageSelector.Init("Language", new List<string> { "English", "Filipino" }, 1);
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
        }

        // Volumes
        masterVolume = Instantiate(volumeSliderPrefab, contentParent);
        masterVolume.Init("Master Volume", 0);
        masterVolume.slider.value = masterVolumeValue;
        audioMixer.SetFloat("MasterVolume", masterVolumeValue);

        sfxVolume = Instantiate(volumeSliderPrefab, contentParent);
        sfxVolume.Init("SFX Volume", 0);
        sfxVolume.slider.value = sfxVolumeValue;
        audioMixer.SetFloat("SFXVolume", sfxVolumeValue);

        musicVolume = Instantiate(volumeSliderPrefab, contentParent);
        musicVolume.Init("Music Volume", 0);
        musicVolume.slider.value = musicVolumeValue;
        audioMixer.SetFloat("MusicVolume", musicVolumeValue);

        masterVolume.slider.onValueChanged.AddListener(UpdateMasterVolume);
        sfxVolume.slider.onValueChanged.AddListener(UpdateSFXVolume);
        musicVolume.slider.onValueChanged.AddListener(UpdateMusicVolume);
    }
    private void ShowHOSettings()
    {
        settingsState = 1;

        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        higherOrderSpeed = Instantiate(arrowSelectorPrefab, contentParent);
        switch (hoSpeed)
        {
            case 2.0f:
                higherOrderSpeed.Init("Adjustable Speed", new List<string> { "Slowest", "Slow", "Average", "Fast", "Fastest" }, 0);
                break;
            case 1.5f:
                higherOrderSpeed.Init("Adjustable Speed", new List<string> { "Slowest", "Slow", "Average", "Fast", "Fastest" }, 1);
                break;
            case 1.0f:
                higherOrderSpeed.Init("Adjustable Speed", new List<string> { "Slowest", "Slow", "Average", "Fast", "Fastest" }, 2);
                break;
            case 0.75f:
                higherOrderSpeed.Init("Adjustable Speed", new List<string> { "Slowest", "Slow", "Average", "Fast", "Fastest" }, 3);
                break;
            case 0.5f:
                higherOrderSpeed.Init("Adjustable Speed", new List<string> { "Slowest", "Slow", "Average", "Fast", "Fastest" }, 4);
                break;
                
        }
    }
    private void ShowLOSettings()
    {
        settingsState = 2;

        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        lowerOrderMode = Instantiate(arrowSelectorPrefab, contentParent);
        lowerOrderMode.Init("Mode", new List<string> { "Add Here", "Add Here", "Add Here" }, 0);
    }
}