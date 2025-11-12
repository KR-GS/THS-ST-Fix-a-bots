using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class StageSelectManager : MonoBehaviour, IDataPersistence
{
    public SoundEffectsManager soundEffectsManager;
    public Button[] stageButtons;
    public Button randButton;

    [Header("Stage Info UI")]
    public GameObject stageInfoPanel, speedPanel, settingsPanel;
    public TMP_Text livesText, restartsText, timeText, speedText;
    public Button yesButton, noButton, decreaseButton, increaseButton, confirmSpeedButton, confirmSpeedPanelButton;

    [Header("Level Map 1")]
    public Image levelMap1;
    public Sprite[] levelMapSprites;

    [Header("Level Map 2")]
    public Image levelMap2;
    public Sprite[] levelMapSprites2;

    [Header("Level Map 3")]
    public Image levelMap3;
    public Sprite[] levelMapSprites3;

    [Header("Level Map Button")]
    public Button leftButton;
    public Button  rightButton;

    [Header("Settings Manager")]
    public SettingsPanelManager settingsManager;

    [Header("Stars")]
    public Image[] starObjects;
    public Sprite fullStarSprite;
    public Sprite emptyStarSprite;
    private float confirmedSpeed = 1f;

    private int currentLevelMap = 0;

    private int selectedStageNum;
    private (int max, float cycInt, float cycLen, int prePressed, bool formSeen, bool lockCoef, bool lockConst, bool refSeen, int coef, int constant, int tutorial) selectedConfig;

    private void Awake()
    {
        StaticData.isOnHigherOrder = true;
        StaticData.isOnHigherOrderGame = false;
        StaticData.isOnLowerOrder = false;
        if (currentLevelMap > 0)
        {
            leftButton.interactable = true;
            leftButton.gameObject.SetActive(true);
        }
        else
        {
            leftButton.interactable = false;
            leftButton.gameObject.SetActive(false);
        }
    } 

    IEnumerator Start()
    {

        Debug.Log("Num Stage Done: " + StaticData.numStageDone);
        yield return null;

        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogError("The DataPersistence.Instance is NULL!!!");
        }

        //UpdateSpeedDisplay();

        //settingsPanel.SetActive(true);
        //settingsPanel.SetActive(false);

        //Updates the background based on the number of stages completed
        UpdateLevelMapBackground();

        for(int i = 0; i < 45; i++)
        {
            stageButtons[i].interactable = false;
        }

        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNum = i;
            //Limits to 15 stages for now
            if(stageNum < 30)
            {
                // LIMITS STUDENTS TO FIRST 15 STAGES
                stageButtons[i].interactable = stageNum <= StaticData.numStageDone;
            }
            
            stageButtons[i].onClick.AddListener(() =>
            {
                // Use the latest confirmed speed
                ShowStageInfo(stageNum);
                soundEffectsManager.playStickerSounds();
            });
        }

        randButton.onClick.AddListener(() =>
            LoadStage(31)
            
        );

        yesButton.onClick.AddListener(ConfirmStageSelection);
        noButton.onClick.AddListener(() => stageInfoPanel.SetActive(false));
        
        confirmSpeedPanelButton.onClick.AddListener(() =>
        {
            settingsPanel.SetActive(true);
            soundEffectsManager.playStickerSounds();
        });

        leftButton.onClick.AddListener(() =>
        {
            currentLevelMap--;
            changeLevelMap(currentLevelMap);
            soundEffectsManager.playStickerSounds();
        });

        rightButton.onClick.AddListener(() =>
        {
            currentLevelMap++;
            changeLevelMap(currentLevelMap);
            soundEffectsManager.playStickerSounds();
        });

        //increaseButton.onClick.AddListener(IncreaseSpeed);
        //decreaseButton.onClick.AddListener(DecreaseSpeed);
        //confirmSpeedButton.onClick.AddListener(ConfirmSpeed);
    }

    private void changeLevelMap(int index)
    {
        switch (index)
        {
            case 0:
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(true);
                rightButton.interactable = true;
                levelMap1.gameObject.SetActive(true);
                levelMap2.gameObject.SetActive(false);
                levelMap3.gameObject.SetActive(false);
                randButton.gameObject.SetActive(false);
                break;
            case 1:
                leftButton.gameObject.SetActive(true);
                leftButton.interactable = true;
                rightButton.gameObject.SetActive(false);
                levelMap1.gameObject.SetActive(false);
                levelMap2.gameObject.SetActive(true);
                levelMap3.gameObject.SetActive(false);
                randButton.gameObject.SetActive(false);
                break;
            
            case 2:
                leftButton.interactable = true;
                leftButton.gameObject.SetActive(true);
                rightButton.interactable = false;
                rightButton.gameObject.SetActive(false);
                levelMap1.gameObject.SetActive(false);
                levelMap2.gameObject.SetActive(false);
                randButton.gameObject.SetActive(true);
                //levelMap3.gameObject.SetActive(true);
                break;
            
        }
    }
    

    private void UpdateLevelMapBackground()
    {
        if (StaticData.numStageDone < 15)
        {
            int index = Mathf.Clamp(StaticData.numStageDone, 0, levelMapSprites.Length - 1);

            levelMap1.sprite = levelMapSprites[index];
            levelMap2.sprite = levelMapSprites2[0];
            levelMap3.sprite = levelMapSprites3[0];
        }

        else if (StaticData.numStageDone < 30)
        {
            int index = StaticData.numStageDone % 15;

            levelMap1.sprite = levelMapSprites[15];
            levelMap2.sprite = levelMapSprites2[index];
            levelMap3.sprite = levelMapSprites3[0];
        }
        else if (StaticData.numStageDone < 45)
        {
            int index = StaticData.numStageDone % 15;

            levelMap1.sprite = levelMapSprites[15];
            levelMap2.sprite = levelMapSprites2[15];
            levelMap3.sprite = levelMapSprites3[index];
        }
        else
        {
            levelMap1.sprite = levelMapSprites[15];
            levelMap2.sprite = levelMapSprites2[15];
            levelMap3.sprite = levelMapSprites3[15];
        }


        // For button labels
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNum = i;
            // Set the button text to the stage number (1-based index)
            TMP_Text buttonLabel = stageButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonLabel != null)
            {
                buttonLabel.text = (stageNum + 1).ToString();
            }

            if (stageNum <= StaticData.numStageDone)
            {
                buttonLabel.color = Color.white; // normal white text for unlocked
            }
            else
            {
                buttonLabel.color = new Color(0.2f, 0.2f, 0.2f);  // greyed out text for locked
            }

            if (stageNum == StaticData.numStageDone)
            {
                // Stop any previous coroutine to prevent duplicates
                StopCoroutine(nameof(PulseText));
                StartCoroutine(PulseText(buttonLabel, buttonLabel.color));
            }
        } 
    }

    void ShowStageInfo(int stageNum)
    {
        selectedStageNum = stageNum;

        if (stageNum == StaticData.numStageDone)
        {
            livesText.text = "N/A";
            restartsText.text = "N/A";
            timeText.text = "N/A";
        }
        else
        {
            livesText.text = $"{StaticData.stageLives[stageNum]}";
            restartsText.text = $"{StaticData.stageRestarts[stageNum]}";
            timeText.text = $"{Mathf.Round(StaticData.stageTime[stageNum] * 100) / 100.0}s";
        }

        int numStars = StaticData.stageStars[stageNum];
        for (int i = 0; i < starObjects.Length; i++)
        {
            if (i < numStars)
                starObjects[i].sprite = fullStarSprite;
            else
                starObjects[i].sprite = emptyStarSprite;
        }

        stageInfoPanel.SetActive(true);
    }

    void ConfirmStageSelection()
    {
        stageInfoPanel.SetActive(false);
        speedPanel.SetActive(false);

        DataPersistenceManager.Instance.LoadGame();
        StaticData.cycleInterval = confirmedSpeed;
        StaticData.cycleLeniency = confirmedSpeed - confirmedSpeed / 4;
        StaticData.stageNum = selectedStageNum;

        if (selectedStageNum == 0 && StaticData.numStageDone == 0)
        {
            Debug.Log("Loading Speed Calibration");
            SceneManager.LoadScene("HO_SpeedCalibration");
        }
        else
            LoadStage(selectedStageNum);
    }

    private IEnumerator PulseText(TMP_Text text, Color baseColor)
    {
        float pulseSpeed = 2f; 
        float scaleAmount = 2f; 

        Vector3 originalScale = text.transform.localScale;
        while (true)
        {
            
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; 
            text.transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleAmount, t);

            text.color = Color.Lerp(baseColor, Color.yellow, t * 0.3f);

            yield return null;
        }
    }

    public void LoadStage(int stageNum)
    {
        
        StaticData.stageNum = stageNum;
    
        Debug.Log("Loading Stage: " + StaticData.stageNum);

        if (stageNum < 15)
        {
            // SceneManager.LoadScene("HO_BotTennisScene");
            //SceneManager.LoadScene("HO_BotFightScene");
            LoadingScreenManager.Instance.SwitchtoSceneMath(2);
        }
        else if (stageNum < 31)
        {
            //SceneManager.LoadScene("HO_BotTennisScene");
            LoadingScreenManager.Instance.SwitchtoSceneMath(3);
        }

        else if (stageNum == 31)
        {
            LoadingScreenManager.Instance.SwitchtoSceneMath(Random.Range(2, 4));
        }
        /*
        else if (stageNum <= 45)
        {
            SceneManager.LoadScene("HO_BotFightScene");
        }  
        */
    }

    public void LoadData(GameData data)
    {
        confirmedSpeed = data.stageSpeed;
        Debug.Log("[StageDataLoader] Data loaded into StaticData");
    }

    public void SaveData(ref GameData data)
    {
        data.stageSpeed = settingsManager.getSelectedStageSpeed();
        data.language = settingsManager.getSelectedLanguage();
        Debug.Log("Speed" + settingsManager.getSelectedStageSpeed());
    }
}

