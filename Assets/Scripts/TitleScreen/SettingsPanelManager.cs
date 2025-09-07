using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsPanelManager : MonoBehaviour
{
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
    private ArrowSelector higherOrderDifficulty;
    private ArrowSelector lowerOrderMode;
    private ArrowSelector languageSelector;

    private VolumeSlider masterVolume;
    private VolumeSlider sfxVolume;
    private VolumeSlider musicVolume;

    private void Start()
    {
        // Higher Order
        higherOrderSpeed = Instantiate(arrowSelectorPrefab, contentParent);
        higherOrderSpeed.Init("Adjustable Speed", new List<string> { "Slow", "Medium", "Fast" }, 1);

        // Lower Order
        lowerOrderMode = Instantiate(arrowSelectorPrefab, contentParent);
        lowerOrderMode.Init("Mode", new List<string> { "Classic", "Challenge", "Endless" }, 0);

        // Language
        languageSelector = Instantiate(arrowSelectorPrefab, contentParent);
        languageSelector.Init("Language", new List<string> { "English", "Filipino"}, 0);

        // Volumes
        masterVolume = Instantiate(volumeSliderPrefab, contentParent);
        masterVolume.Init("Master Volume", 0.8f);

        sfxVolume = Instantiate(volumeSliderPrefab, contentParent);
        sfxVolume.Init("SFX Volume", 0.8f);

        musicVolume = Instantiate(volumeSliderPrefab, contentParent);
        musicVolume.Init("Music Volume", 0.6f);

        confirmButton.onClick.AddListener(SaveSettings);
        exitButton.onClick.AddListener(OnExitSettings);
    }

    //TODO: Replace to save settings properly
    public void SaveSettings()
    {
        Debug.Log($"Higher Order Speed: {higherOrderSpeed.GetCurrentValue()}");
        Debug.Log($"Difficulty: {higherOrderDifficulty.GetCurrentValue()}");
        Debug.Log($"Lower Order Mode: {lowerOrderMode.GetCurrentValue()}");
        Debug.Log($"Language: {languageSelector.GetCurrentValue()}");

        Debug.Log($"Master Volume: {masterVolume.GetValue()}");
        Debug.Log($"SFX Volume: {sfxVolume.GetValue()}");
        Debug.Log($"Music Volume: {musicVolume.GetValue()}");

        mainPanel.SetActive(false);
    }

    public void OnExitSettings()
    {
        mainPanel.SetActive(false);
    }
}