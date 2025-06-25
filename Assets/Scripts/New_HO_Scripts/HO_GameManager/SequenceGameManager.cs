using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SequenceGameManager : MonoBehaviour
{
    [Header("Raycasts")]
    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;

    [Header("UI & Prefabs")]
    public GameObject timePeriodButtonPrefab;
    public Transform buttonsParent;   
    public TextMeshProUGUI feedbackText, formulaText, timerText, livesText, restartText;
    public Button nextStageButton, restartStageButton, pauseButton;
    public FormulaInputPanel formulaPanel;
    public GameTimer gameTimer;
    public HealthBar healthBar;
    public Sprite unpressedSprite;

    [Header("Pause Panel")]
    public GameObject pausePanel;
    public Button resumeButton, exitButton;
    public TextMeshProUGUI panelText;

    [Header("Central Animator")]
    public Animator statusAnimator;

    [Header("Settings")]
    private int maxNumber = 25;
    private float cycleInterval = 1, cycleLeniency = 0.4f;
    private int prePressedCount = 0, stageNum = 0;
    private bool isFormulaSeen = true, isRandomSequence = true, isStageFinished = false;

    [Header("Active scene name")]
    private string sceneName;

    [Header("Audio Files")]
    public SoundEffectsManager soundEffectsManager;
    private HOStageData stageData;
    private Sequence currentSequence;
    public FormulaInputPanel formulaInputPanel;
    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();
    private int currentCycleIndex = 0;
    private List<int> pressedNumbers = new List<int>();
    private bool isCycling = false, isCorrect = true, canTap = true, isStart = true,
    gotRight = false, wasRestartButtonPressed = false, nextStage = false;
    private float timer;

    // Basically checks if the pointer/mouse is above an interactable UI
    private bool IsPointerOverInteractableUi()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if ((result.gameObject.GetComponent<Button>() != null || result.gameObject.GetComponent<Toggle>() != null) && !result.gameObject.GetComponent<TimePeriodButton>()) 
            {
                return true;
            }
        }

        return false;
    }
    bool IsScreenTapped()
    {
    #if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
    #elif UNITY_ANDROID || UNITY_IOS
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    #else
        return false;
    #endif
    }

    void GetData()
    {
        maxNumber = StaticData.maxNumber;
        cycleInterval = StaticData.cycleInterval;
        cycleLeniency = StaticData.cycleLeniency;
        prePressedCount = StaticData.prePressedCount;
        isFormulaSeen = StaticData.isFormulaSeen;
        isRandomSequence = StaticData.isRandomSequence;
        stageNum = StaticData.stageNum;
        formulaInputPanel.SetLockCoefficient(StaticData.lockCoefficient);
        formulaInputPanel.SetLockConstant(StaticData.lockConstant);
    }

    void InitilizeStageData()
    {
        stageData = new HOStageData();
        stageData.SetStageNum(StaticData.stageNum);
        stageData.SetNumRestarts(0);
        stageData.SetElapsedTime(0f);
        stageData.SetNumLives(5);
    }

    void InitializeStageUi()
    {
        formulaText.gameObject.SetActive(isFormulaSeen);
        isCycling = false;

        formulaPanel.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        nextStageButton.gameObject.SetActive(false);

        restartStageButton.enabled = false;
        restartStageButton.onClick.AddListener(() =>
        {
            isCycling = false;
            wasRestartButtonPressed = true;
            ResetSequence();
        });

        //pauseButton.enabled = false;
        pauseButton.onClick.AddListener(() => PauseGame());
        resumeButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        exitButton.onClick.AddListener(() => ExitGame());

        feedbackText.text = "Please tap screen to start game";
        livesText.text = $"{stageData.GetNumLives()}";
        healthBar.SetMaxHealth(stageData.GetNumLives());
        restartText.text = $"{stageData.GetNumRestarts()}";

    }

    void PauseGame()
    {
        Time.timeScale = 0;
        gameTimer.StopTimer();
        canTap = false;
        pausePanel.SetActive(true);
        if (stageData.GetNumLives() <= 0)
        {
            panelText.text = "You Lost! Continue?";
        }
        else
        {
            panelText.text = "Game Paused";
        }
    }

    IEnumerator ResumeGame()
    {
        if (stageData.GetNumLives() <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
        if (nextStage)
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            gameTimer.ResumeTimer();
            canTap = true;
        }
        if (!nextStage && !isCycling)
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            if (!gameTimer.GetIsRunning())
            {
                gameTimer.ResumeTimer();
            }
            canTap = true;
        }
        if (!nextStage && isCycling)
        {
            Time.timeScale = 1;
            isCycling = false;
            pausePanel.SetActive(false);
            restartStageButton.enabled = false;
            feedbackText.text = "3";
            yield return new WaitForSeconds(1);
            feedbackText.text = "2";
            yield return new WaitForSeconds(1);
            feedbackText.text = "1";
            yield return new WaitForSeconds(1);
            gameTimer.ResumeTimer();
            //pauseButton.enabled = true;
            isCycling = true;
            restartStageButton.enabled = true;
            canTap = true;
        }
        yield return null;
    }

    void ExitGame() {
        SceneManager.LoadScene("Stage_Select");
    }
  
    void Start()
    {
        GetData();
        sceneName= SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: "+ sceneName);
        InitilizeStageData();
        InitializeStageUi();
        SetupButtons();
    }

    void Update()
    {

        if ((Input.GetMouseButtonDown(0) ||IsScreenTapped()) && !IsPointerOverInteractableUi() && isStart)
        {
            isStart = false;
            StartNewSequence();
            StartCoroutine(DelayedStartCycle());
        }
        if (stageData.GetNumLives() <= 0)
        {
            isCycling = false;
            PauseGame();
        }
    }

    // Creates buttons, destroys previous buttons as well
    void SetupButtons()
    {
        foreach (Transform child in buttonsParent)
            Destroy(child.gameObject);
        buttons.Clear();

        for (int i = 1; i <= maxNumber; i++)
        {
            GameObject go = Instantiate(timePeriodButtonPrefab, buttonsParent);
            TimePeriodButton btn = go.GetComponent<TimePeriodButton>();
            btn.ButtonNumber = i;
            btn.SetHighlighted(false);
            buttons.Add(btn);
        }
    }

    // Generates new sequence and preselects them on the time periods based on settings
    void StartNewSequence()
    {
        canTap = true;
        if (isRandomSequence)
        {
            currentSequence = new Sequence(maxNumber);
        }
        else
        {
            currentSequence = new Sequence(maxNumber, StaticData.coefficient, StaticData.constant);
        }
        formulaText.text = $"Rule: {currentSequence.FormulaString}";

        pressedNumbers.Clear();

        // Pre-press first n numbers in the sequence, mark them as selected
        for (int i = 0; i < Mathf.Clamp(prePressedCount, 0, currentSequence.Numbers.Count); i++)
        {
            int num = currentSequence.Numbers[i];
            pressedNumbers.Add(num);
            buttons[num - 1].SetGreen();
            buttons[num - 1].SetSelected(true);
            buttons[num - 1].SetPreSelected(true);
        }

        currentCycleIndex = 0;
        //isCycling = true;

        feedbackText.text = "Watch the sequence! Tap the screen when the highlighted number is in the sequence.";
    }

    // Just to make time for the cycling
    IEnumerator DelayedStartCycle()
    {
        yield return new WaitForSeconds(2f);
        feedbackText.text = "3";
        yield return new WaitForSeconds(1f);
        feedbackText.text = "2";
        yield return new WaitForSeconds(1f);
        feedbackText.text = "1";
        yield return new WaitForSeconds(1f);
        feedbackText.text = "Go!";
        StartCoroutine(CycleButtons());
        yield return new WaitForSeconds(1f);
        isCycling = true;
        gameTimer.StartTimer();
        restartStageButton.enabled = true;
        //pauseButton.enabled = true;
        yield return null;
    }

    IEnumerator RestartCycle()
    {
        canTap = false;
        isCycling = false;
        feedbackText.text = "Restarting Stage...";
        yield return new WaitForSeconds(1f);
        feedbackText.text = "3";
        yield return new WaitForSeconds(1f);
        feedbackText.text = "2";
        yield return new WaitForSeconds(1f);
        feedbackText.text = "1";
        yield return new WaitForSeconds(1f);
        feedbackText.text = "Go!";
        //pauseButton.enabled = true;
        restartStageButton.enabled = true;
        isCycling = true;
        if (wasRestartButtonPressed)
        {
            currentCycleIndex = -1;
        }
        else
        {
            currentCycleIndex = 0;
        }
        wasRestartButtonPressed = false;
        canTap = true;
        isCorrect = true;
        yield return null;
    }

    // Main loop for the cycling
    IEnumerator CycleButtons()
    {
        while (true)
        {
            while (!isCycling)
            {
                yield return null;
                continue;
            }

            // Makes sure player has time to start
            
            if (currentCycleIndex == 0)
            {
                yield return new WaitForSeconds(1f);
            }
            
        
            HighlightButton(currentCycleIndex);

            Debug.Log("Cycle index: " + currentCycleIndex);

            int btnNumber = currentCycleIndex + 1;
            bool inSequence = currentSequence.Numbers.Contains(btnNumber);

            // Show feedback for cycle step
            feedbackText.text = inSequence ? $"Number {btnNumber} is part of the sequence." : $"Number {btnNumber} is NOT part of the sequence.";

            // Wait for cycleInterval seconds and listen for input 
            timer = 0f;

            bool hasNotClicked = true;

            gotRight = false;

            statusAnimator.SetBool("MissTrigger", false);
            statusAnimator.SetBool("AnticipateTrigger", false);
            statusAnimator.SetBool("HitTrigger", false);
            statusAnimator.SetBool("WrongTrigger", false);
            soundEffectsManager.playIdleSound();

            while (timer < cycleInterval && currentCycleIndex >= 0)
            {
                if (hasNotClicked)
                {
                    if (!inSequence)
                    {
                        statusAnimator.SetBool("IdleTrigger", true);
                    }
                    else if (inSequence)
                    {
                        statusAnimator.SetBool("IdleTrigger", false);
                        statusAnimator.SetBool("AnticipateTrigger", true);
                        // If the Sequence was pre pressed, automatically plays hit animation
                        if ((buttons[btnNumber - 1].GetPreSelected() || buttons[btnNumber - 1].GetWasSelected()) && timer > 0.10f)
                        {
                            statusAnimator.SetBool("AnticipateTrigger", false);
                            statusAnimator.SetBool("HitTrigger", true);
                            soundEffectsManager.playHitSound();
                            hasNotClicked = false;
                        }
                        // if the player misses, plays miss animation
                        if (timer > cycleLeniency && !gotRight)
                        {
                            feedbackText.text = "You missed!";
                            statusAnimator.SetBool("AnticipateTrigger", false);
                            statusAnimator.SetBool("MissTrigger", true);
                            soundEffectsManager.playMissSound();
                            isCorrect = false;
                            if (!isStageFinished)
                            {
                                stageData.SetNumLives(stageData.GetNumLives() - 1);
                                livesText.text = $"{stageData.GetNumLives()}";
                                healthBar.SetHealth(stageData.GetNumLives());
                            }
                            hasNotClicked = false;
                        }
                    }
                    // Listens to player tapping the screen
                    if ((Input.GetMouseButtonDown(0) ||IsScreenTapped()) && canTap && !IsPointerOverInteractableUi())
                    {
                        Debug.Log("Clicked: " + gameObject.name);
                        HandleUserTap(btnNumber, inSequence);
                        hasNotClicked = false;
                    }
                }
                if (isCycling)
                {
                    timer += Time.deltaTime;
                }
                yield return null;
            }

            // After cycle of 25 buttons, check if sequence complete
            if (currentCycleIndex == maxNumber - 1)
            {
                if (CheckSequenceComplete() && isCorrect)
                {
                    feedbackText.text = "Great job! Sequence completed!";
                    nextStageButton.gameObject.SetActive(true);
                    isStageFinished = true;
                    nextStageButton.onClick.AddListener(() => OnNextStageButtonClicked());
                    canTap = false;
                }
                else
                {
                    Debug.Log("Wrong Sequence");
                    feedbackText.text = "Sequence not complete or wrong taps. Restarting...";
                    ResetSequence();
                }
            }

            currentCycleIndex = (currentCycleIndex + 1) % maxNumber;
        }
    }

    void HighlightButton(int index)
    {
        
        if (index > 0 && !buttons[index - 1].GetSelected())
        {
            buttons[index - 1].SetHighlighted(false);
        }
        if (index > 0 && buttons[index - 1].GetSelected())
        {
            buttons[index - 1].SetGreen();
        }
        if (index == 0  && !buttons[maxNumber - 1].GetSelected())
        {
            buttons[maxNumber - 1].SetHighlighted(false);
        }
        if (index == 0  && buttons[maxNumber - 1].GetSelected())
        {
            buttons[maxNumber - 1].SetGreen();
        }
        buttons[index].SetHighlighted(true);
    }

    void HandleUserTap(int btnNumber, bool inSequence)
    {
        
        if (pressedNumbers.Contains(btnNumber))
        {
            feedbackText.text = $"You already pressed number {btnNumber}.";
            return;
        }

        if (inSequence)
        {
            buttons[btnNumber - 1].SetGreen();
            buttons[btnNumber - 1].SetWasSelected(true);
            pressedNumbers.Add(btnNumber);
            buttons[btnNumber - 1].SetSelected(true);
            feedbackText.text = $"You pressed the right number: {btnNumber}!";
            statusAnimator.SetBool("IdleTrigger", false);
            statusAnimator.SetBool("HitTrigger", true);
            soundEffectsManager.playHitSound();
            gotRight = true;
        }
        else
        {
            buttons[btnNumber - 1].SetSelected(true);
            feedbackText.text = $"Wrong button! {btnNumber} is not in the sequence.";
            statusAnimator.SetBool("IdleTrigger", false);
            statusAnimator.SetBool("WrongTrigger", true);  
            isCorrect = false;
            soundEffectsManager.playMissSound();
            if (!isStageFinished)
            {
                stageData.SetNumLives(stageData.GetNumLives() - 1);
                livesText.text = $"{stageData.GetNumLives()}";
                healthBar.SetHealth(stageData.GetNumLives());
                isCorrect = false;
            }
        }
    }

    bool CheckSequenceComplete()
    {
        // Player must have pressed all sequence numbers
        foreach (int seqNum in currentSequence.Numbers)
        {
            if (!pressedNumbers.Contains(seqNum))
                return false;
        }
        return true;
    }

    void ResetSequence()
    {
        restartStageButton.enabled = false;
        //pauseButton.enabled = false;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetHighlighted(false);
            buttons[i].SetSelected(false);
            buttons[i].SetWasSelected(false);
            buttons[i].GetComponent<Image>().sprite = unpressedSprite;
        }
        pressedNumbers.Clear();

        for (int i = 0; i < Mathf.Clamp(prePressedCount, 0, currentSequence.Numbers.Count); i++)
        {
            int num = currentSequence.Numbers[i];
            pressedNumbers.Add(num);
            buttons[num - 1].SetGreen();
            buttons[num - 1].SetSelected(true);
        }

        if(!isStageFinished){
            stageData.SetNumRestarts(stageData.GetNumRestarts() + 1);
            restartText.text = $"{stageData.GetNumRestarts()}";
        } 
        StartCoroutine(RestartCycle());
    }

    public void OnNextStageButtonClicked()
    {
        nextStageButton.gameObject.SetActive(false);
        nextStage = true;
        isCycling = false;
        StopAllCoroutines(); // stops the button cycling
        feedbackText.text = "";
        
        foreach (var btn in buttons)
            btn.SetHighlighted(false);

        foreach (int num in currentSequence.Numbers)
        {
            buttons[num - 1].SetGreen();
        }

        // Show formula panel with current sequence
        formulaPanel.gameObject.SetActive(true);
        formulaPanel.ShowPanel(currentSequence, gameTimer, stageData);
    }
}
