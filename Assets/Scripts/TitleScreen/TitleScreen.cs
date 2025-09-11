using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [Header("Panels")]
    public GameObject continuePanel;
    public GameObject settingsPanel;

    [Header("Buttons")]
    public Button HOButton;
    public Button LOButton;
    public Button settingsButton;
    public Button continueButton;

    private void Start()
    {
        
        continuePanel.SetActive(true);
        settingsPanel.SetActive(true);

        HOButton.gameObject.SetActive(false);
        LOButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);

        
        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);

        HOButton.onClick.AddListener(() => LoadScene("Stage_Select")); 
        //TODO: Enter LO landing page here
        LOButton.onClick.AddListener(() => LoadScene("LOScene")); 
    }

    private void OnContinue()
    {
        continuePanel.SetActive(false);
        HOButton.gameObject.SetActive(true);
        LOButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
    }

    private void OnSettings()
    {
        settingsPanel.SetActive(true);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}