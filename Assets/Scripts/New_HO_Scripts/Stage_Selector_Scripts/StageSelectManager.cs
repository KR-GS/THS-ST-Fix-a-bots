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
    private (int max, float cycInt, float cycLen, int prePressed, bool formSeen, bool lockCoef, bool lockConst, int coef, int constant) selectedConfig;

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

        UpdateSpeedDisplay();

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

        randButton.onClick.AddListener(() =>
            LoadStage(5, 25, 1f, 0.6f, 0, false, false, false, 0, 0, true)
        );

        yesButton.onClick.AddListener(ConfirmStageSelection);
        noButton.onClick.AddListener(() => stageInfoPanel.SetActive(false));
        confirmSpeedPanelButton.onClick.AddListener(() => speedPanel.SetActive(true));
        increaseButton.onClick.AddListener(IncreaseSpeed);
        decreaseButton.onClick.AddListener(DecreaseSpeed);
        confirmSpeedButton.onClick.AddListener(ConfirmSpeed);
    }

    private (int, float, float, int, bool, bool, bool, int, int)[] GetStageConfigs()
    {
        return new (int, float, float, int, bool, bool, bool, int, int)[]
        {
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, true,  true,  true,  2,  2),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, false, false, true,  3, -2),
            (20, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 3, false, false, false, 3, -1),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 0, true,  true,  true,  3,  1),
            (25, confirmedSpeed, confirmedSpeed - confirmedSpeed / 4, 0, false, false, false, 4, -3)
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

    void ShowStageInfo(int stageNum, (int, float, float, int, bool, bool, bool, int, int) config)
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
            cfg.coef, cfg.constant, false);
    }

    public void LoadStage(int stageNum, int max, float cycInt, float cycLen, int prePressed, bool formSeen, bool lockCoef,
        bool lockConst, int coef, int constant, bool randSeq)
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
        StaticData.constant = constant;
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

