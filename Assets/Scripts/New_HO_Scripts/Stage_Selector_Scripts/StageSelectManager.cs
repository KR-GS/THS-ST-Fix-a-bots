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
    public GameObject stageInfoPanel, speedPanel;
    public TMP_Text livesText, restartsText, timeText, speedText;
    public Button yesButton, noButton, decreaseButton, increaseButton, confirmSpeedButton, confirmSpeedPanelButton;

    private string[] speedLabels = { "Slowest", "Slow", "Average", "Fast", "Fastest" };
    private float[] speedValues = { 2f, 1.5f, 1f, 0.75f, 0.5f };
    private int currentSpeedIndex = 2;
    private float confirmedSpeed = 1f;

    private int selectedStageNum;
    private (int max, float cycInt, float cycLen, int prePressed, bool formSeen, bool lockCoef, bool lockConst, bool refSeen, int coef, int constant, int tutorial) selectedConfig;

    IEnumerator Start()
    {
        StaticData.isOnHigherOrder = true;
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

        UpdateSpeedDisplay();

        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNum = i;
            stageButtons[i].interactable = stageNum <= StaticData.numStageDone;

            if (i < StaticData.numStageDone)
            {
                var color = stageButtons[i].targetGraphic.color;
                color.a = 125; 
                stageButtons[i].targetGraphic.color = color;
            }
            

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
        confirmSpeedPanelButton.onClick.AddListener(() => speedPanel.SetActive(true));
        increaseButton.onClick.AddListener(IncreaseSpeed);
        decreaseButton.onClick.AddListener(DecreaseSpeed);
        confirmSpeedButton.onClick.AddListener(ConfirmSpeed);
    }

    private (int, float, float, int, bool, bool, bool, bool, int, int, int)[] GetStageConfigs()
    //num of buttons, speed, leniency, prepressed, is the formula seen, is coef locked, is const locked, isrefseen, coef, constant, tutorial
    {
        return new (int, float, float, int, bool, bool, bool, bool, int, int, int)[]
        {
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, true,  true,  true, true, 2, 3, 0),
            (15, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 2, false, false, true, true, 3, 1, 1),
            (18, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 0, false, false, true, true, 4, 1, 1),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, false, true, false, false, 3, -1, 2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 1, false, true, false, true, 4, -3, 2),
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
        data.stageSpeed = confirmedSpeed;
        Debug.Log("[StageDataLoader] Data saved from StaticData");
    }
}

