using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StageSelectManager : MonoBehaviour, IDataPersistence
{
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

    private string[] speedLabels = { "Slowest", "Slow", "Average", "Fast", "Fastest" };
    private float[] speedValues = { 2f, 1.5f, 1f, 0.75f, 0.5f };
    private int currentSpeedIndex = 2;
    private float confirmedSpeed = 1f;

    private int currentLevelMap = 0;

    private int selectedStageNum;
    private (int max, float cycInt, float cycLen, int prePressed, bool formSeen, bool lockCoef, bool lockConst, bool refSeen, int coef, int constant, int tutorial) selectedConfig;

    private void Awake()
    {
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
        StaticData.isOnHigherOrder = true;
        StaticData.isOnHigherOrderGame = false;
        StaticData.isOnLowerOrder = false;

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

        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNum = i;
            stageButtons[i].interactable = stageNum <= StaticData.numStageDone;


            stageButtons[i].onClick.AddListener(() =>
            {
                var configs = GetStageConfigs(); // Use the latest confirmed speed
                ShowStageInfo(stageNum, configs[stageNum]);
            });
        }

        /*randButton.onClick.AddListener(() =>
            LoadStage(5, 25, 1f, 0.6f, 0, false, false, false, false, 0, 0, true)
        );*/

        yesButton.onClick.AddListener(ConfirmStageSelection);
        noButton.onClick.AddListener(() => stageInfoPanel.SetActive(false));
        confirmSpeedPanelButton.onClick.AddListener(() => settingsPanel.SetActive(true));

        leftButton.onClick.AddListener(() =>
        {
            currentLevelMap--;
            changeLevelMap(currentLevelMap);
        });

        rightButton.onClick.AddListener(() =>
        {
            currentLevelMap++;
            changeLevelMap(currentLevelMap);
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
                leftButton.interactable = false;
                leftButton.gameObject.SetActive(false);
                levelMap1.gameObject.SetActive(true);
                levelMap2.gameObject.SetActive(false);
                levelMap3.gameObject.SetActive(false);
                break;
            case 1:
                leftButton.interactable = true;
                leftButton.gameObject.SetActive(true);
                levelMap1.gameObject.SetActive(false);
                levelMap2.gameObject.SetActive(true);
                levelMap3.gameObject.SetActive(false);
                break;
            case 2:
                leftButton.interactable = true;
                leftButton.gameObject.SetActive(true);
                levelMap1.gameObject.SetActive(false);
                levelMap2.gameObject.SetActive(false);
                levelMap3.gameObject.SetActive(true);
                break;
        }
    }
    

    private void UpdateLevelMapBackground()
    {
        if (StaticData.numStageDone <= 15)
        {
            int index = Mathf.Clamp(StaticData.numStageDone, 0, levelMapSprites.Length - 1);

            levelMap1.sprite = levelMapSprites[index];
            levelMap2.sprite = levelMapSprites2[0];
            levelMap3.sprite = levelMapSprites3[0];
        }
        
        else if (StaticData.numStageDone <= 30)
        {
            int index = Mathf.Clamp(StaticData.numStageDone - 16, 0, levelMapSprites2.Length - 1);

            levelMap1.sprite = levelMapSprites[15];
            levelMap2.sprite = levelMapSprites2[index];
            levelMap3.sprite = levelMapSprites3[0];
        }
        else
        {
            int index = Mathf.Clamp(StaticData.numStageDone - 31, 0, levelMapSprites3.Length - 1);

            levelMap1.sprite = levelMapSprites[15];
            levelMap2.sprite = levelMapSprites2[15];
            levelMap3.sprite = levelMapSprites3[index];
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
        }
    }

    // IF YOU WANT TO ADD MORE STAGES, DO IT HERE, JUST COPY THE FORMAT
    private (int, float, float, int, bool, bool, bool, bool, int, int, int)[] GetStageConfigs()
    //num of buttons, speed, leniency, prepressed, is the formula seen, is coef locked, is const locked, isrefseen, coef, constant, tutorial(NOT USED),
    {
        return new (int, float, float, int, bool, bool, bool, bool, int, int, int)[]
        {
            // SCENARIO 1
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, true,  true,  true, true, 2, 3, 0),
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, true, true, 3, 1, 1),
            (18, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 0, false, false, true, true, 4, 1, 1),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, false, true, false, false, 3, -1, 2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, true, 4, -3, 2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, false, 3, 2, 0),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, true, true, 3, -2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, false, false, 3, -1, 3),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, true, 4, 2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, false, 5, -3, 3),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, false, 3, 2, 0),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, true, true, 3, -2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, false, false, 3, -1, 3),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, true, 4, 2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, false, 5, -3, 3),

            // SCENARIO 2
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, true,  true,  true, true, 2, 3, 0),
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, true, true, 3, 1, 1),
            (18, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 0, false, false, true, true, 4, 1, 1),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, false, true, false, false, 3, -1, 2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, true, 4, -3, 2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, false, 3, 2, 0),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, true, true, 3, -2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, false, false, 3, -1, 3),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, true, 4, 2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, false, 5, -3, 3),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, false, 3, 2, 0),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, true, true, 3, -2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, false, false, 3, -1, 3),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, true, 4, 2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, false, 5, -3, 3),

            // SCENARIO 3
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, true,  true,  true, true, 2, 3, 0),
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, true, true, 3, 1, 1),
            (18, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 0, false, false, true, true, 4, 1, 1),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, false, true, false, false, 3, -1, 2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, true, 4, -3, 2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, false, 3, 2, 0),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, true, true, 3, -2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, false, false, 3, -1, 3),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, true, 4, 2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, false, 5, -3, 3),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, false, 3, 2, 0),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, true, true, 3, -2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, false, false, 3, -1, 3),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, true, 4, 2, 0),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, false, false, false, 5, -3, 3)
        };
    }

    void IncreaseSpeed()
    {
        if (currentSpeedIndex < speedLabels.Length - 1)
        {
            currentSpeedIndex++;
            UpdateSpeedDisplay();
        }
    }

    void DecreaseSpeed()
    {
        if (currentSpeedIndex > 0)
        {
            currentSpeedIndex--;
            UpdateSpeedDisplay();
        }
    }

    void UpdateSpeedDisplay()
    {
        speedText.text = speedLabels[currentSpeedIndex];
    }

    void ConfirmSpeed()
    {
        confirmedSpeed = speedValues[currentSpeedIndex];
        speedPanel.SetActive(false);
        DataPersistenceManager.Instance.SaveGame();
        Debug.Log("Speed confirmed: " + confirmedSpeed);
    }

    void ShowStageInfo(int stageNum, (int, float, float, int, bool, bool, bool, bool, int, int, int) config)
    {
        selectedStageNum = stageNum;
        selectedConfig = config;

        if (stageNum == StaticData.stageLives.Count)
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

        stageInfoPanel.SetActive(true);
    }

    void ConfirmStageSelection()
    {
        var cfg = selectedConfig;

        stageInfoPanel.SetActive(false);
        speedPanel.SetActive(false);

        LoadStage(selectedStageNum, cfg.max, cfg.cycInt, cfg.cycLen,
            cfg.prePressed, cfg.formSeen, cfg.lockCoef, cfg.lockConst,
            cfg.refSeen, cfg.coef, cfg.constant, cfg.tutorial, false);
    }

    public void LoadStage(int stageNum, int max, float cycInt, float cycLen, int prePressed, bool formSeen, bool lockCoef,
        bool lockConst, bool isRefSeen, int coef, int constant, int tutorial, bool randSeq)
    {
        StaticData.stageNum = stageNum;
        StaticData.maxNumber = max;
        StaticData.cycleInterval = cycInt;
        StaticData.cycleLeniency = cycLen;
        StaticData.prePressedCount = prePressed;
        StaticData.isFormulaSeen = formSeen;
        StaticData.lockCoefficient = lockConst;
        StaticData.lockConstant = lockCoef;
        StaticData.coefficient = coef;
        StaticData.refSeen = isRefSeen;
        StaticData.constant = constant;
        StaticData.tutorialType = tutorial;
        StaticData.isRandomSequence = randSeq;

        SceneManager.LoadScene("HO_BotFightScene");
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

