using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class FormulaAttempt
{
    public int coefficient;
    public int constant;

    public FormulaAttempt(int coef, int consta)
    {
        coefficient = coef;
        constant = consta;
    }
}

public class FormulaInputPanel : MonoBehaviour, IDataPersistence
{


    [Header("UI References")]
    public TMP_Text coefficientText, constantText, feedbackText;
    public TextMeshProUGUI signText;

    public Button coefUpButton, coefDownButton, constUpButton, constDownButton, submitButton;

    [Header("Lock Settings")]
    private bool lockCoefficient = false, lockConstant = false;

    private int currentCoef = 1, currentConst = 0;

    private int i = 0;
    private Sequence targetSequence;
    private GameTimer gameTimer;
    private HOStageData stageData;

    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();

    private FormulaAttemptWrapper stageFormulaAttempts;

    private string stageStringAttempt = "";

    private DataPersistenceManager dpm;

    public void SetLockConstant(bool constant)
    {
        lockConstant = constant;
    }
    public void SetLockCoefficient(bool coef)
    {
        lockCoefficient = coef;
    }

    public void StoreStageData()
    {
        //if haven't done the stage before, just store everything
        if (StaticData.numStageDone == stageData.GetStageNum())
        {
            StaticData.stageTime[stageData.GetStageNum()] = stageData.GetElapsedTime();
            StaticData.stageLives[stageData.GetStageNum()] = stageData.GetNumLives();
            StaticData.stageRestarts[stageData.GetStageNum()] = stageData.GetNumRestarts();
            StaticData.formulaAttempts[stageData.GetStageNum()] = stageFormulaAttempts;
            StaticData.numStageDone = stageData.GetStageNum() + 1;
        }
        //else, check if it is better before storing
        else
        {
            if (StaticData.stageTime[stageData.GetStageNum()] > stageData.GetElapsedTime())
            {
                StaticData.stageTime[stageData.GetStageNum()] = stageData.GetElapsedTime();
            }
            if (StaticData.stageLives[stageData.GetStageNum()] < stageData.GetNumLives())
            {
                StaticData.stageLives[stageData.GetStageNum()] = stageData.GetNumLives();
            }
            if (StaticData.stageRestarts[stageData.GetStageNum()] > stageData.GetNumRestarts())
            {
                StaticData.stageRestarts[stageData.GetStageNum()] = stageData.GetNumRestarts();
            }
            
        }

    }


    private void UpdateButtonHighlights()
    {
        // Generate new sequence based on current input
        List<int> predictedSequence = new Sequence(buttons.Count, currentCoef, currentConst).Numbers;

        foreach (var btn in buttons)
            btn.SetHighlighted(false);

        // Highlight buttons in the new sequence
        foreach (int number in predictedSequence)
        {
            if (number >= 1 && number <= buttons.Count)
            {
                buttons[number - 1].SetHighlighted(true);
            }
        }

        foreach (int num in targetSequence.Numbers)
        {
            buttons[num - 1].SetGreen();
        }
    }

    public void ShowPanel(Sequence sequence, GameTimer gameTimer1, HOStageData sd, List<TimePeriodButton> btns)
    {
        buttons = btns;
        stageData = sd;
        gameTimer = gameTimer1;
        targetSequence = sequence;

        currentCoef = 0;
        currentConst = 0;

        ApplyLockSettings();
        UpdateFormulaText();
        feedbackText.text = "Adjust the formula and submit.";

    }

    private void ApplyLockSettings()
    {
        if (lockCoefficient)
        {
            coefUpButton.transform.localScale = new Vector3(0, 0, 0);
            coefDownButton.transform.localScale = new Vector3(0, 0, 0);
            currentCoef = targetSequence.Coefficient;
        }
        if (lockConstant)
        {
            constUpButton.transform.localScale = new Vector3(0, 0, 0);
            constDownButton.transform.localScale = new Vector3(0, 0, 0);
            currentConst = targetSequence.Constant;
        }

    }

    private void Start()
    {
        coefUpButton.onClick.AddListener(() => { currentCoef += 1; UpdateFormulaText(); });
        coefDownButton.onClick.AddListener(() =>
        {
            if (currentCoef > 1)
            {
                currentCoef -= 1;
                UpdateFormulaText();
            }
        });

        constUpButton.onClick.AddListener(() => { currentConst += 1; UpdateFormulaText(); });
        constDownButton.onClick.AddListener(() => { currentConst -= 1; UpdateFormulaText(); });

        submitButton.onClick.AddListener(ValidateFormula);
    }

    private void UpdateFormulaText()
    {
        coefficientText.text = $"{currentCoef}";
        signText.text = currentConst >= 0 ? "+" : "-";
        constantText.text = currentConst >= 0 ? $"{currentConst}" : $"{-currentConst}";
        if (currentCoef != 0)
        {
            UpdateButtonHighlights();
        }
    }

    private void ValidateFormula()
    {

        string sign = currentConst >= 0 ? "+" : "-";
        int absConst = Mathf.Abs(currentConst);
        string attempt = $"{currentCoef}n {sign} {absConst}";

        if (!string.IsNullOrEmpty(stageStringAttempt))
            stageStringAttempt += ", ";

        stageStringAttempt += attempt;
        
        stageFormulaAttempts = new FormulaAttemptWrapper{
            Attempt = stageStringAttempt
        };

        StaticData.EnsureStageListSizes();
        StaticData.formulaAttempts[stageData.GetStageNum()] = stageFormulaAttempts;

        Debug.Log($"[Formula] Current attempts: {StaticData.formulaAttempts[stageData.GetStageNum()]}");


        if (currentCoef != targetSequence.Coefficient && currentConst != targetSequence.Constant)
        {
            feedbackText.text = "Both are wrong";
        }
        else if (currentCoef != targetSequence.Coefficient && currentConst == targetSequence.Constant)
        {
            feedbackText.text = "Coefficient is wrong";
        }
        else if (currentCoef == targetSequence.Coefficient && currentConst != targetSequence.Constant)
        {
            feedbackText.text = "Constant is wrong";
        }
        else if (currentCoef == targetSequence.Coefficient && currentConst == targetSequence.Constant)
        {
            feedbackText.text = "Both are right";

            if (StaticData.isRandomSequence)
            {
                SceneManager.LoadScene("Ho_BotFight");
            }
            else
            {
                gameTimer.StopTimer();
                stageData.SetElapsedTime(gameTimer.GetElapsedTime());
                StoreStageData();
                DataPersistenceManager.Instance.SaveGame();
                SceneManager.LoadScene("Stage_Select");
            }
        }
    }

    public void LoadData(GameData data)
    {
        StaticData.stageLives = new System.Collections.Generic.List<int>(data.lives);
        StaticData.stageRestarts = new System.Collections.Generic.List<int>(data.restarts);
        StaticData.stageTime = new System.Collections.Generic.List<float>(data.stageTimes);
        StaticData.formulaAttempts = new List<FormulaAttemptWrapper>(data.formulaAttempts);
        
        StaticData.numStageDone = data.stageDone;

        Debug.Log("[StageDataLoader] Data loaded into StaticData");
    }

    public void SaveData(ref GameData data)
    {
        StaticData.EnsureStageListSizes();

        data.lives = new System.Collections.Generic.List<int>(StaticData.stageLives);
        data.restarts = new System.Collections.Generic.List<int>(StaticData.stageRestarts);
        data.stageTimes = new System.Collections.Generic.List<float>(StaticData.stageTime);
        data.formulaAttempts = new System.Collections.Generic.List<FormulaAttemptWrapper>(StaticData.formulaAttempts);
        data.stageDone = StaticData.numStageDone;

        Debug.Log("[StageDataLoader] Data saved from StaticData");
    }
}