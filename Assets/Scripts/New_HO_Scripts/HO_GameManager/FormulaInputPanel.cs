using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;
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

    [Header("DynamicReevaler")]
    public Button revealDynamic;
    public GameObject buttonsParent;
    public GameObject buttonsParent2;

    [Header("UI References")]
    public TMP_Text feedbackText;
    public GameObject linePrefab, horizontalLinePrefab, yellowLinePrefab, continuePanel;
    public RectTransform buttonContainer;
    private List<GameObject> activeLines = new List<GameObject>();
    public Button submitButton, continueButton, restartButton;

    [Header("Block System")]
    public BlockManager blockManager;

    [Header("Lock Settings")]
    private bool lockCoefficient = false;
    private bool lockConstant = false;

    private int currentCoef = 1, currentConst = 0, numStars;

    [Header("Tutorial")]
    public FormulaInputTutorial formulaInputTutorial;

    [Header("EndScreen Animator")]
    public EndScreenAnimator endScreenAnimator;
    public GameObject stageCompletePanel;
    public GameObject livesTextObj, restartsTextObj, timeTextObj, livesTextHolder, restartsTextHolder, timeTextHolder;
    public TMP_Text livesText, restartsText, timeText;
    public GameObject star1, star2, star3;
    public Sprite fullStarSprite, emptyStarSprite;

    private int i = 0;
    private Sequence targetSequence;
    private GameTimer gameTimer;
    private HOStageData stageData;

    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();
    private string stageStringAttempt = "";

    private DataPersistenceManager dpm;

    public Sequence GetTargetSequence()
    {
        return targetSequence;
    }

    private void Awake()
    {
        Instance = this;
        submitButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        /*
        if (formulaInputTutorial != null)
        {
            formulaInputTutorial.ShowTutorial(StaticData.tutorialType);
        }
        */
        revealDynamic.onClick.AddListener(() => StartRevealProccess());
        buttonsParent2.SetActive(false);

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() =>
            {
                LoadStageSelectScene();
            });
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(ValidateFormula);
        }
    }

    private void Update()
    {   
        FormulaBlock nBlock = blockManager.FindBlock(BlockType.Variable, 0, "n");
        if (IsFormulaComplete(nBlock))
        {
            submitButton.gameObject.SetActive(true);
            buttonsParent.SetActive(false);
            buttonsParent2.SetActive(true);

            // Change color to indicate correct placement, only show based on stage level
            if (StaticData.stageFormulaHint[StaticData.stageNum])
            {
                if (FormulaInputPanel.Instance != null && StaticData.stageFormulaHint[StaticData.stageNum])
                {
                    var seq = targetSequence; // get the correct target

                    nBlock.SetColor(Color.green);

                    if (nBlock.LeftConnectedBlock != null)
                    {
                        // Check if coefficient matches
                        if (int.Parse(nBlock.LeftConnectedBlock.blockText.text) == seq.Coefficient)
                            nBlock.LeftConnectedBlock.SetColor(Color.green);
                    }
                    if (nBlock.RightConnectedBlock != null)
                    {
                        // For sign, check if this symbol is the correct one
                        string expectedSymbol = seq.Constant >= 0 ? "+" : "-";
                        if (nBlock.RightConnectedBlock.blockText.text == expectedSymbol)
                            nBlock.RightConnectedBlock.SetColor(Color.green);

                        if (nBlock.RightConnectedBlock.RightConnectedBlock != null)
                        {
                            // Check if constant matches
                            int absConstant = Mathf.Abs(seq.Constant);
                            if (int.Parse(nBlock.RightConnectedBlock.RightConnectedBlock.blockText.text) == absConstant)
                                nBlock.RightConnectedBlock.RightConnectedBlock.SetColor(Color.green);
                        }
                    }
                }
            }
        }
        else
        {
            if (nBlock.LeftConnectedBlock != null)
            {
                nBlock.LeftConnectedBlock.blockImage.color = nBlock.LeftConnectedBlock.blockColor;
            }
            if (nBlock.RightConnectedBlock != null)
            {
                nBlock.RightConnectedBlock.blockImage.color = nBlock.RightConnectedBlock.blockColor;

                if (nBlock.RightConnectedBlock.RightConnectedBlock != null)
                {
                    nBlock.RightConnectedBlock.RightConnectedBlock.blockImage.color = nBlock.RightConnectedBlock.RightConnectedBlock.blockColor;
                }
            }
            buttonsParent2.SetActive(false);
            buttonsParent.SetActive(true);
            submitButton.gameObject.SetActive(false);
        }
    }

    public void SetLockConstant(bool constant)
    {
        lockConstant = constant;
    }

    public void SetLockCoefficient(bool coef)
    {
        lockCoefficient = coef;
    }

    private void StartRevealProccess()
    {
        revealDynamic.interactable = false;
        StartCoroutine(ShowAndCooldown());
    }

    IEnumerator ShowAndCooldown()
    {
        
        // Show for 5 seconds
        buttonsParent.SetActive(false);
        buttonsParent2.SetActive(true);
        yield return new WaitForSeconds(5f);
        buttonsParent2.SetActive(false);
        buttonsParent.SetActive(true);

        // Extra 5 seconds cooldown
        yield return new WaitForSeconds(5f);

        revealDynamic.interactable = true;
        yield return null;
    }

    public void StoreStageData()
    {
        //STAR GENERATION
        if (stageData.GetNumLives() == 5 && stageData.GetElapsedTime() <= 75f && stageData.GetNumRestarts() == 0 && stageStringAttempt.Length <= 8)
        {
            numStars = 3;
        }
        else if (stageData.GetNumLives() >= 2 && stageData.GetElapsedTime() <= 150f && stageData.GetNumRestarts() <= 3)
        {
            numStars = 2;
        }
        else
        {
            numStars = 1;
        }

        //if haven't done the stage before, just store everything
        if (StaticData.numStageDone <= StaticData.stageNum)
        {
            Debug.Log("Storing stage data for the first time for stage " + stageData.GetStageNum());
            StaticData.stageTime[stageData.GetStageNum()] = stageData.GetElapsedTime();
            StaticData.stageLives[stageData.GetStageNum()] = stageData.GetNumLives();
            StaticData.stageRestarts[stageData.GetStageNum()] = stageData.GetNumRestarts();
            StaticData.formulaAttempts[stageData.GetStageNum()] = stageStringAttempt;
            StaticData.stageStars[stageData.GetStageNum()] = numStars;
            StaticData.numStageDone = StaticData.stageNum + 1;
            Debug.Log("Num stage done " + StaticData.numStageDone);
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
            livesText.text = $"{StaticData.stageLives[StaticData.stageNum]}";
            restartsText.text = $"{StaticData.stageRestarts[StaticData.stageNum]}";
            timeText.text = $"{Mathf.Round(StaticData.stageTime[StaticData.stageNum] * 100) / 100.0}s";
    }

    private void ShowLinesForCoefficient(int startValue, int coef, List<int> predictedSequence)
    {
        // Clear existing lines
        foreach (var line in activeLines)
            Destroy(line);
        activeLines.Clear();

        // Draw initial constant lines
        for (i = 1; i <= currentConst; i++)
        {
            TimePeriodButton button = buttons.Find(b => b.ButtonNumber == i);
            if (button == null) continue;

            RectTransform btnRect = button.GetComponent<RectTransform>();
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

        // Generate all vertical line positions 
        while (val <= predictedSequence[predictedSequence.Count - 1])
        {
            linePositions.Add(val);
            val += coef;
        }

        //Draw vertical lines 
        foreach (int pos in linePositions)
        {
            TimePeriodButton button = buttons.Find(b => b.ButtonNumber == pos);
            if (button == null) continue;

            RectTransform btnRect = button.GetComponent<RectTransform>();
            GameObject line = Instantiate(linePrefab, btnRect);
            activeLines.Add(line);

            RectTransform lineRect = line.GetComponent<RectTransform>();
            lineRect.SetAsLastSibling();
            lineRect.anchoredPosition = new Vector3(0, 40f, 0);
        }

        //Generate horizontal line positions
        for (int n = 0; n < linePositions.Count - 1; n++)
        {
            int from = linePositions[n];
            int to = linePositions[n + 1];

            // Add in-between positions for horizontal lines
            for (int h = from + 1; h < to && h <= predictedSequence[predictedSequence.Count - 1]; h++)
            {
                horizontalPositions.Add(h);
            }
        }

        //Draw Horizontal lines
        foreach (int pos in horizontalPositions)
        {
            TimePeriodButton button = buttons.Find(b => b.ButtonNumber == pos);
            if (button == null) continue;

            RectTransform btnRect = button.GetComponent<RectTransform>();
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

            ShowLinesForCoefficient(currentCoef + currentConst, currentCoef, null);
            return;
        }

        // Generate sequence numbers
        List<int> predictedSequence = new Sequence(buttons.Count + currentConst, currentCoef, currentConst).Numbers;

        // Clear all highlights first
        foreach (var btn in buttons)
            btn.SetHighlighted(false);

        // Highlight buttons whose ButtonNumber matches the generated sequence
        foreach (int num in predictedSequence)
        {
            TimePeriodButton button = buttons.Find(b => b.ButtonNumber == num);
            if (button != null)
                button.SetBlue();
        }

        ShowLinesForCoefficient(currentCoef + currentConst, currentCoef, predictedSequence);
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
        feedbackText.text = "I-Drag ang puzzle blocks para buoin ang formula at ipasa.";
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
        LoadingScreenManager.Instance.SwitchtoScene(1);
    }

    private void ValidateFormula()
    {
        // Update formula from current blocks
        UpdateCurrentFormula();

        // Check if formula is complete using the n block
        FormulaBlock nBlock = blockManager.FindBlock(BlockType.Variable, 0, "n");
        if (!IsFormulaComplete(nBlock))
        {
            feedbackText.text = "Tapusin muna ang formula sa pamamagitan ng pagdugtong ng mga puzzle piece.";
            return;
        }

        string sign = currentConst >= 0 ? "+" : "-";
        int absConst = Mathf.Abs(currentConst);
        string attempt = $"{currentCoef}n {sign} {absConst}";

        // Logs attempts to gamedata
        if (!string.IsNullOrEmpty(stageStringAttempt))
            stageStringAttempt += ", ";

        stageStringAttempt += attempt;

        StaticData.EnsureStageListSizes();

        if (currentCoef != targetSequence.Coefficient && currentConst != targetSequence.Constant)
        {
            feedbackText.text = "Hala! Parehong mali ang coefficient at constant.";
        }
        else if (currentCoef != targetSequence.Coefficient && currentConst == targetSequence.Constant)
        {
            feedbackText.text = "Hala! Mali ang coefficient.";
        }
        else if (currentCoef == targetSequence.Coefficient && currentConst != targetSequence.Constant)
        {
            feedbackText.text = "Hala! Mali ang constant.";
        }
        else if (currentCoef == targetSequence.Coefficient && currentConst == targetSequence.Constant)
        {
            feedbackText.text = "Yehey! Parehong tama ang constant at coefficient!";

            if (nBlock != null)
            {
                nBlock.SetColor(Color.green);

                if (nBlock.LeftConnectedBlock != null)
                    nBlock.LeftConnectedBlock.SetColor(Color.green);

                if (nBlock.RightConnectedBlock != null)
                {
                    nBlock.RightConnectedBlock.SetColor(Color.green);
                    if (nBlock.RightConnectedBlock.RightConnectedBlock != null)
                        nBlock.RightConnectedBlock.RightConnectedBlock.SetColor(Color.green);
                }
            }

            Debug.Log("Stage Formula Attempts = " + stageStringAttempt);

            if (StaticData.isRandomSequence[StaticData.stageNum])
            {
                SceneManager.LoadScene("Ho_BotFight");
            }
            else
            {
                gameTimer.StopTimer();
                stageData.SetElapsedTime(gameTimer.GetElapsedTime());
                Debug.Log("Saving Data");
                StoreStageData();
                DataPersistenceManager.Instance.SaveGame();

                endScreenAnimator.gameObject.SetActive(true);
                endScreenAnimator.transform.SetAsLastSibling();

                StartCoroutine(LoadEndScreenAnimation());

                //continuePanel.SetActive(true);
                //continuePanel.transform.SetAsLastSibling();
            }
        }
    }

    private IEnumerator LoadEndScreenAnimation()
    {
        // Wait before showing the panel
        yield return new WaitForSeconds(2f);

        stageCompletePanel.SetActive(true);
        stageCompletePanel.transform.SetAsLastSibling();

        IEnumerator PopElement(GameObject obj, float popScale = 1.2f, float duration = 0.2f)
        {
            obj.SetActive(true);
            obj.transform.localScale = Vector3.zero;

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(0f, popScale, timer / duration);
                obj.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            // Shrink slightly back to normal
            timer = 0f;
            while (timer < duration / 2f)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(popScale, 1f, timer / (duration / 2f));
                obj.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            obj.transform.localScale = Vector3.one;
        }

        // Update stats
        livesText.text = $"{stageData.GetNumLives()}";
        restartsText.text = $"{stageData.GetNumLives()}";
        timeText.text = $"{Math.Round(stageData.GetElapsedTime(),2)}s";

        // Pop in stats one by one
        yield return StartCoroutine(PopElement(livesTextObj));
        yield return StartCoroutine(PopElement(livesTextHolder));
        
        yield return StartCoroutine(PopElement(restartsTextObj));
        yield return StartCoroutine(PopElement(restartsTextHolder));
        
        yield return StartCoroutine(PopElement(timeTextObj));
        yield return StartCoroutine(PopElement(timeTextHolder));

        // Update stars
        int stars = numStars;
        star1.GetComponent<Image>().sprite = stars >= 1 ? fullStarSprite : emptyStarSprite;
        star2.GetComponent<Image>().sprite = stars >= 2 ? fullStarSprite : emptyStarSprite;
        star3.GetComponent<Image>().sprite = stars >= 3 ? fullStarSprite : emptyStarSprite;

        // Pop in stars one by one
        yield return StartCoroutine(PopElement(star1));
        yield return StartCoroutine(PopElement(star2));
        yield return StartCoroutine(PopElement(star3));

        yield return StartCoroutine(PopElement(restartButton.gameObject));
        yield return StartCoroutine(PopElement(continueButton.gameObject));
        
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