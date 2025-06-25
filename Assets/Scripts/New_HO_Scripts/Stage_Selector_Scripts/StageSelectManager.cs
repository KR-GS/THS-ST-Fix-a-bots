using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StageSelectManager : MonoBehaviour
{
    public Button[] stageButtons;
    public Button randButton;

    [Header("Stage Info UI")]
    public GameObject stageInfoPanel;
    public TMP_Text livesText, restartsText, timeText;
    public Button yesButton, noButton;

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

        //LoadGame() here for StaticData.numStageDone, StaticData.stageLives[], StaticData.stageRestarts[], StaticData.stageTime[]
        stageInfoPanel.SetActive(false);
        var stageConfigs = new (int max, float cycInt, float cycLen, int prePressed,
        bool formSeen, bool lockCoef, bool lockConst, int coef, int constant)[]
        {
            (25, 1f, 0.6f, 3, true,  true,  true,  2,  2),
            (25, 1f, 0.6f, 3, false, false, true,  3, -2),
            (25, 1f, 0.6f, 3, false, false, false, 3, -1),
            (25, 1f, 0.6f, 0, true,  true,  true,  3,  1),
            (25, 1f, 0.6f, 0, false, false, false, 4, -3)
        };

        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNum = i;
            var cfg = stageConfigs[i];

            stageButtons[i].interactable = stageNum <= StaticData.numStageDone;

            stageButtons[i].onClick.AddListener(() => ShowStageInfo(stageNum, cfg));
        }

        randButton.onClick.AddListener(() => LoadStage(5, 25, 1f, 0.6f, 0, false, false, false, 0, 0, true));

        yesButton.onClick.AddListener(ConfirmStageSelection);
        noButton.onClick.AddListener(() => stageInfoPanel.SetActive(false));
    }

    void ShowStageInfo(int stageNum, (int, float, float, int, bool, bool, bool, int, int) config)
    {
        selectedStageNum = stageNum;
        selectedConfig = config;

        // Safety check
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
            timeText.text = $"{Mathf.Round(StaticData.stageTime[stageNum]*100)/100.0}s";
        }

        stageInfoPanel.SetActive(true);
    }

    void ConfirmStageSelection()
    {
        var cfg = selectedConfig;

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
}
