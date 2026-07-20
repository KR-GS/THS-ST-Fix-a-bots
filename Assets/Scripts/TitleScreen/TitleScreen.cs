using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour, IDataPersistence
{
    
    [Header("Panels")]
    public GameObject continuePanel;
    public GameObject settingsPanel;
    public RawImage trailerImage;

    [Header("Buttons")]
    public Button HOButton;
    public Button LOButton;
    public Button settingsButton;
    public Button continueButton;

    [Header("Continue Video")]
    public VideoPlayer continueVideo;
    // 60 seconds of idle time before the video plays
    public float idleTimeBeforeVideo = 10f;
    public RawImage videoRawImage;
    private float idleTimer;
    private bool videoPlaying = false;
    public SoundEffectsManager soundEffectsManager;

    private void Start()
    {
        idleTimer = 0f;

        StaticData.isOnHigherOrder = false;
        StaticData.isOnLowerOrder = false;
        StaticData.isOnHigherOrderGame = false;
        StaticData.isOnLowerOrderGame = false;

        continuePanel.SetActive(true);
        settingsPanel.SetActive(true);

        HOButton.gameObject.SetActive(false);
        LOButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        videoRawImage.gameObject.SetActive(false);
        trailerImage.gameObject.SetActive(false);


        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);

        settingsPanel.SetActive(false);

        HOButton.onClick.AddListener(() => LoadingScreenManager.Instance.SwitchtoSceneMath(1));
        //LoadScene("Stage_Select"));
        LOButton.onClick.AddListener(() => LoadingScreenManager.Instance.SwitchtoSceneMath(7));
        //LoadScene("LO_WS2D"));

        if(continueVideo != null){
            print("Continue video is not null");
        }
    }

    private void OnContinue()
    {
        if (continueVideo != null && continueVideo.isPlaying){
            videoRawImage.gameObject.SetActive(false);
            videoPlaying = false;
            continueVideo.Stop();
            idleTimer = 0f;
            soundEffectsManager.playMusic();
            trailerImage.gameObject.SetActive(false);
            return;
        }
        continuePanel.SetActive(false);
        HOButton.gameObject.SetActive(true);
        LOButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        // If the start screen is already pressed, do nothing
        if (!continuePanel.activeSelf)
            return;

        // Adds to the timer before playing the video
        idleTimer += Time.deltaTime;

        // Play the vid if the timer has reached the set idle time
        if (idleTimer >= idleTimeBeforeVideo && !videoPlaying)
        {
            videoRawImage.gameObject.SetActive(true);
            videoPlaying = true;

            if (continueVideo != null){
            continueVideo.isLooping = true;
            continueVideo.Play();
            soundEffectsManager.stopMusic();
            trailerImage.gameObject.SetActive(true);
            }
                
        }
    }

    void CheckVideoTime()
    {
        Debug.Log(continueVideo.time);
    }

    private void OnSettings()
    {
        settingsPanel.SetActive(true);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadData(GameData data)
    {
    }

    public void SaveData(ref GameData data)
    {
    }
}