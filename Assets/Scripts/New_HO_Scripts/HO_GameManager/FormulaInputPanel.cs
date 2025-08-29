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
    public static FormulaInputPanel Instance;

    [Header("UI References")]
    public TMP_Text feedbackText;
    public GameObject linePrefab, horizontalLinePrefab, yellowLinePrefab, continuePanel;
    public RectTransform buttonContainer;
    private List<GameObject> activeLines = new List<GameObject>();
    public Button submitButton, continueButton;

    [Header("Block System")]
    public BlockManager blockManager;

    [Header("Lock Settings")]
    private bool lockCoefficient = false;
    private bool lockConstant = false;

    private int currentCoef = 1, currentConst = 0, numStars;

    [Header("Tutorial")]
    public FormulaInputTutorial formulaInputTutorial;

    private int i = 0;
    private Sequence targetSequence;
    private GameTimer gameTimer;
    private HOStageData stageData;

    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();
    private string stageStringAttempt = "";

    private DataPersistenceManager dpm;

    private void Awake()
    {
        Instance = this;
    }

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

        for (i = 1; i <= currentConst; i++)
        {
            if (i - 1 >= buttons.Count) break;

            TimePeriodButton button = buttons[i - 1];
            RectTransform btnRect = button.GetComponent<RectTransform>();

            // Create line
            GameObject line = Instantiate(yellowLinePrefab, btnRect);
            activeLines.Add(line);

            RectTransform lineRect = line.GetComponent<RectTransform>();

            lineRect.SetAsLastSibling();
            lineRect.anchoredPosition = new Vector3(0, 60f, 0);
        }

        if (coef <= 1) return;

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
            if (!linePositions.Contains(val) && val < linePositions[linePositions.Count - 1] && val > 0)
            {
                horizontalPositions.Add(val);
            }
            val++;
        }

        //spawning of vertical lines
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

        //spawning of horizontal lines
        foreach (int pos in horizontalPositions)
        {
            if (pos < 1 || pos > buttons.Count || pos == 1) continue;

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
        if (buttons.Count == 0 || currentCoef == 0)
        {
            foreach (var btn in buttons)
                btn.SetHighlighted(false);
                
            ShowLinesForCoefficient(currentCoef + currentConst, currentCoef);
            return;
        }

        

        List<int> predictedSequence = new Sequence(buttons.Count, currentCoef, currentConst).Numbers;

        foreach (var btn in buttons)
            btn.SetHighlighted(false);

        //shows the coef 
        foreach (int num in predictedSequence)
        {
            if (num >= 1 && num <= buttons.Count)
            {
                buttons[num - 1].SetBlue();
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

        SetupBlocks();
        ApplyLockSettings();
        feedbackText.text = "Drag blocks to build the formula and submit.";
    }

    private void SetupBlocks()
    {
        if (blockManager == null)
        {
            Debug.LogError("FormulaInputPanel: BlockManager not assigned!");
            return;
        }

        blockManager.CreateAllBlocks();

        // Organize blocks in container
        blockManager.OrganizeBlocks();
    }

    private void ApplyLockSettings()
    {
        if (lockCoefficient)
        {
            // Find the n block and the correct coefficient block
            FormulaBlock nBlock = blockManager.FindBlock(BlockType.Variable, 0, "n");
            FormulaBlock correctCoefBlock = blockManager.FindBlock(BlockType.Coefficient, targetSequence.Coefficient);

            if (nBlock != null && correctCoefBlock != null && nBlock.leftSnapSlot != null)
            {
                correctCoefBlock.ConnectToSlot(nBlock.leftSnapSlot);
                correctCoefBlock.SetLocked(true);
            }
        }

        if (lockConstant)
        {
            // Find the n block and create the sign->constant chain
            FormulaBlock nBlock = blockManager.FindBlock(BlockType.Variable, 0, "n");

            int absConstant = Mathf.Abs(targetSequence.Constant);
            FormulaBlock correctConstBlock = blockManager.FindBlock(BlockType.Constant, absConstant);

            string signSymbol = targetSequence.Constant >= 0 ? "+" : "-";
            FormulaBlock correctSignBlock = blockManager.FindBlock(BlockType.Sign, 0, signSymbol);

            if (nBlock != null && correctSignBlock != null && nBlock.rightSnapSlot != null)
            {
                // Connect sign to n block
                correctSignBlock.ConnectToSlot(nBlock.rightSnapSlot);
                correctSignBlock.SetLocked(true);

                // Connect constant to sign block
                if (correctConstBlock != null && correctSignBlock.rightSnapSlot != null)
                {
                    correctConstBlock.ConnectToSlot(correctSignBlock.rightSnapSlot);
                    correctConstBlock.SetLocked(true);
                }
            }
        }
    }

    public void OnBlockConnected(FormulaBlock connectedBlock, FormulaBlock targetBlock, SnapPosition position)
    {
        UpdateCurrentFormula();
        UpdateButtonHighlights();

        // Visual feedback
        Debug.Log($"Block {connectedBlock.blockType} connected to {targetBlock.blockType} on {position} side");
    }

    public void OnBlockDisconnected(FormulaBlock disconnectedBlock, FormulaBlock fromBlock)
    {
        UpdateCurrentFormula();
        UpdateButtonHighlights();

        Debug.Log($"Block {disconnectedBlock.blockType} disconnected from {fromBlock.blockType}");
    }

    private void UpdateCurrentFormula()
    {
        // Find the variable block (n) to get the complete formula
        FormulaBlock nBlock = blockManager.FindBlock(BlockType.Variable, 0, "n");

        if (nBlock == null)
        {
            currentCoef = 0;
            currentConst = 0;
            return;
        }

        // Get coefficient from left connection
        if (nBlock.HasLeftConnection)
        {
            currentCoef = nBlock.LeftConnectedBlock.value;
        }
        else
        {
            currentCoef = 0;
        }

        // Get constant and sign from right connections
        if (nBlock.HasRightConnection && nBlock.RightConnectedBlock.HasRightConnection)
        {
            FormulaBlock signBlock = nBlock.RightConnectedBlock;
            FormulaBlock constantBlock = signBlock.RightConnectedBlock;

            int signMultiplier = signBlock.symbol == "+" ? 1 : -1;
            currentConst = constantBlock.value * signMultiplier;
        }
        else
        {
            currentConst = 0;
        }

        Debug.Log($"Current Formula: {currentCoef}n {(currentConst >= 0 ? "+" : "")} {currentConst}");
    }

    private void LoadStageSelectScene()
    {
        SceneManager.LoadScene("Stage_Select");
    }

    private void Start()
    {
        /*
        if (formulaInputTutorial != null)
        {
            formulaInputTutorial.ShowTutorial(StaticData.tutorialType);
        }
        */

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() =>
            {
                LoadStageSelectScene();
            });
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(ValidateFormula);
        }
    }

    private void ValidateFormula()
    {
        // Update formula from current blocks
        UpdateCurrentFormula();

        // Check if formula is complete using the n block
        FormulaBlock nBlock = blockManager.FindBlock(BlockType.Variable, 0, "n");
        if (!IsFormulaComplete(nBlock))
        {
            feedbackText.text = "Please complete the formula by connecting all required blocks.";
            return;
        }

        string sign = currentConst >= 0 ? "+" : "-";
        int absConst = Mathf.Abs(currentConst);
        string attempt = $"{currentCoef}n {sign} {absConst}";

        if (!string.IsNullOrEmpty(stageStringAttempt))
            stageStringAttempt += ", ";

        stageStringAttempt += attempt;

        StaticData.EnsureStageListSizes();

        if (currentCoef != targetSequence.Coefficient && currentConst != targetSequence.Constant)
        {
            feedbackText.text = "Both coefficient and constant are wrong";
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
            feedbackText.text = "Perfect! Both coefficient and constant are correct!";

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

                continuePanel.SetActive(true);
                continuePanel.transform.SetAsLastSibling();
            }
        }
    }

    private bool IsFormulaComplete(FormulaBlock nBlock)
    {
        if (nBlock == null) return false;

        // Check the complete chain: Coefficient -> n -> Sign -> Constant
        bool hasCoefficient = nBlock.HasLeftConnection;
        bool hasSign = nBlock.HasRightConnection;
        bool hasConstant = hasSign && nBlock.RightConnectedBlock.HasRightConnection;

        return hasCoefficient && hasSign && hasConstant;
    }

    public void ResetFormula()
    {
        if (blockManager != null)
        {
            blockManager.ResetAllBlocks();
        }

        // Reset current formula values
        currentCoef = 0;
        currentConst = 0;

        // Clear visual elements
        foreach (var line in activeLines)
        {
            Destroy(line);
        }
        activeLines.Clear();

        // Reset button highlights
        foreach (var btn in buttons)
        {
            btn.SetHighlighted(false);
        }

        feedbackText.text = "Formula reset. Drag blocks to connect them and build the formula.";
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

        Debug.Log("[FormulaInputPanel] Data saved from StaticData");
    }


    private void OnDestroy()
    {
        // Clean up any remaining lines
        foreach (var line in activeLines)
        {
            if (line != null)
                Destroy(line);
        }
        activeLines.Clear();
    }
}