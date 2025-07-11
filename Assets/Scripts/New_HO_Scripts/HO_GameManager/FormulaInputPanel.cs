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

    public GameObject linePrefab, horizontalLinePrefab;
    public RectTransform buttonContainer;
    private List<GameObject> activeLines = new List<GameObject>();
    public TextMeshProUGUI signText;

    public Button coefUpButton, coefDownButton, constUpButton, constDownButton, submitButton;

    [Header("Lock Settings")]
    private bool lockCoefficient = false, lockConstant = false;

    private int currentCoef = 1, currentConst = 0, numStars;

    private int i = 0;
    private Sequence targetSequence;
    private GameTimer gameTimer;
    private HOStageData stageData;

    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();
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
        //STAR GENERATION
        if (stageData.GetNumLives() == 5 && stageData.GetElapsedTime() <= 60f && stageData.GetNumRestarts() == 0)
        {
            numStars = 3;
        }
        else if (stageData.GetNumLives() >= 2 && stageData.GetElapsedTime() <= 120f && stageData.GetNumRestarts() <= 3)
        {
            numStars = 2;
        }
        else
        {
            numStars = 1;
        }

        //if haven't done the stage before, just store everything
        if (StaticData.numStageDone == stageData.GetStageNum())
        {
            StaticData.stageTime[stageData.GetStageNum()] = stageData.GetElapsedTime();
            StaticData.stageLives[stageData.GetStageNum()] = stageData.GetNumLives();
            StaticData.stageRestarts[stageData.GetStageNum()] = stageData.GetNumRestarts();
            StaticData.formulaAttempts[stageData.GetStageNum()] = stageStringAttempt;
            StaticData.stageStars[stageData.GetStageNum()] = numStars;
            StaticData.numStageDone = stageData.GetStageNum() + 1;
        }
        //else, check if it is better before storing
        else
        {
            if (numStars >= StaticData.stageStars[stageData.GetStageNum()])
            {
                StaticData.stageStars[stageData.GetStageNum()] = numStars;
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
                StaticData.formulaAttempts[stageData.GetStageNum()] = stageStringAttempt;
                
            }
        }
    }

    private void ShowLinesForCoefficient(int startValue, int coef)
    {
        // Clear existing lines
        foreach (var line in activeLines)
            Destroy(line);
        activeLines.Clear();

        if (coef == 0) return;

        List<int> linePositions = new List<int>();
        List<int> horizontalPositions = new List<int>();
        int val = startValue;

        //getting the vert lines
        while (val <= buttons.Count)
        {
            linePositions.Add(val);
            val += coef;
        }

        val = startValue;

        //getting the horizontal lines
        while (val <= buttons.Count)
        {
            if (!linePositions.Contains(val) && val < linePositions[linePositions.Count - 1])
            {
                horizontalPositions.Add(val);
            }
            val++;
        }

        //spawning of horizontal lines
        foreach (int pos in linePositions)
        {
            if (pos < 1 || pos > buttons.Count) continue;

            TimePeriodButton button = buttons[pos - 1];
            RectTransform btnRect = button.GetComponent<RectTransform>();

            // Create line
            GameObject line = Instantiate(linePrefab, btnRect);
            activeLines.Add(line);

            RectTransform lineRect = line.GetComponent<RectTransform>();

            lineRect.SetAsLastSibling();
            lineRect.anchoredPosition = new Vector3(0, 40f, 0);
        }

        //spawning of vertical lines
        foreach (int pos in horizontalPositions)
        {
            if (pos < 1 || pos > buttons.Count) continue;

            TimePeriodButton button = buttons[pos - 1];
            RectTransform btnRect = button.GetComponent<RectTransform>();

            // Create line
            GameObject line = Instantiate(horizontalLinePrefab, btnRect);
            activeLines.Add(line);

            RectTransform horizontalLineRect = line.GetComponent<RectTransform>();

            horizontalLineRect.SetAsLastSibling();
            horizontalLineRect.anchoredPosition = new Vector3(0, 60f, 0);
        }
    }


    private void UpdateButtonHighlights()
    {
        List<int> predictedSequence = new Sequence(buttons.Count, currentCoef, currentConst).Numbers;

        foreach (var btn in buttons)
            btn.SetHighlighted(false);

        if (currentConst >= 1)
        {
            for (i = 0; i < currentConst; i++)
            {
                buttons[i].SetHighlighted(true);
            }
        }

        foreach (int num in predictedSequence)
        {
            if (num >= 1 && num <= buttons.Count)
            {
                buttons[num - 1].SetRed();
            }
        }

        foreach (int num in targetSequence.Numbers)
        {
            if (num >= 1 && num <= buttons.Count)
            {
                buttons[num - 1].SetGray();
            }
        }

        foreach (int num in predictedSequence)
        {
            if (targetSequence.Numbers.Contains(num) && num >= 1 && num <= buttons.Count)
            {
                buttons[num - 1].SetGreen();
            }
        }

        ShowLinesForCoefficient(currentCoef + currentConst, currentCoef);
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
        constDownButton.onClick.AddListener(() =>
        {
            if (currentConst > -currentCoef)
            {
                currentConst -= 1; UpdateFormulaText();
            }
        });

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

        StaticData.EnsureStageListSizes();
       /* StaticData.formulaAttempts[stageData.GetStageNum()] = stageFormulaAttempts;

        Debug.Log($"[Formula] Current attempts: {StaticData.formulaAttempts[stageData.GetStageNum()]}");*/


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

            Debug.Log("Stage Formula Attempts = " + stageStringAttempt);

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
    }

    public void SaveData(ref GameData data)
    {
        StaticData.EnsureStageListSizes();

        data.lives = new List<int>(StaticData.stageLives);
        data.restarts = new List<int>(StaticData.stageRestarts);
        data.stageTimes = new List<float>(StaticData.stageTime);
        data.formulaAttempts = new List<string>(StaticData.formulaAttempts);
        data.stageStars = new List<int>(StaticData.stageStars);
        data.stageDone = StaticData.numStageDone;

        Debug.Log("[StageDataLoader] Data saved from StaticData");
    }
}