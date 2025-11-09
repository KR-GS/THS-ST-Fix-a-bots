using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Burst.CompilerServices;

public class SequenceGameManager : MonoBehaviour
{
    [Header("Raycasts")]
    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;

    [Header("UI & Prefabs")]
    public GameObject timePeriodButtonPrefab, matchText, correctText;
    public Transform buttonsParent, buttonsParent2;   
    public TextMeshProUGUI feedbackText, formulaText, timerText, livesText, restartText;
    public Button nextStageButton, restartStageButton, pauseButton;
    public FormulaInputPanel formulaPanel;
    public GameTimer gameTimer;
    public HealthBar healthBar;
    public Sprite unpressedSprite;

    [Header("Restart Panel")]
    public GameObject pausePanel;
    public Button restartGameButton, exitButton;
    public TextMeshProUGUI panelText;

    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button settingsExitButton;
    public Button settingsConfirmButton;

    [Header("Central Animator")]
    public Animator statusAnimator;

    [Header("Swipe Direction Panel")]
    public GameObject swipeDirectionPanel; 
    public GameObject directionArrow;

    [Header("Settings")]
    private int maxNumber = 25;
    private float cycleInterval = 1, cycleLeniency = 0.4f;
    private int prePressedCount = 0, stageNum = 0;
    private bool isFormulaSeen = true, isRandomSequence = true, isStageFinished = false;

    [Header("Active scene name")]
    private string sceneName;

    [Header("Audio Files")]
    public SoundEffectsManager soundEffectsManager;

    [Header("Swipes")]
    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private List<string> expectedSwipeSequence;
    private int currentSwipeIndex = 0;
    private float minSwipeDistance = 10f; 

    private HOStageData stageData;
    private Sequence currentSequence;
    public FormulaInputPanel formulaInputPanel;
    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();
    private List<TimePeriodButton> buttons2 = new List<TimePeriodButton>();
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
        stageNum = StaticData.stageNum;
        maxNumber = StaticData.maxNumber[stageNum];
        Debug.Log("Max Number: " + StaticData.maxNumber[stageNum] + " for stage " + (StaticData.stageNum));
        cycleInterval = StaticData.cycleInterval;
        cycleLeniency = StaticData.cycleLeniency;
        prePressedCount = StaticData.prePressedCount[stageNum];
        isFormulaSeen = StaticData.isFormulaSeen[stageNum];
        isRandomSequence = StaticData.isRandomSequence[stageNum];
        stageNum = StaticData.stageNum;
        formulaInputPanel.SetLockCoefficient(StaticData.lockCoefficient[stageNum]);
        formulaInputPanel.SetLockConstant(StaticData.lockConstant[stageNum]);
        expectedSwipeSequence = StaticData.stageSwipes[stageNum];
        
    }

    void InitilizeStageData()
    {
        stageData = new HOStageData();
        stageData.SetStageNum(StaticData.stageNum);
        stageData.SetNumRestarts(0);
        stageData.SetElapsedTime(0f);
        stageData.SetNumLives(5);
        Time.timeScale = 1;
    }

    void InitializeStageUi()
    {
        //formulaText.gameObject.SetActive(isFormulaSeen);
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
        settingsConfirmButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        settingsExitButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        exitButton.onClick.AddListener(() => ExitGame());
        restartGameButton.onClick.AddListener(() => RestartGame());

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
        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();
    }

    void LostGame()
    {
        Time.timeScale = 0;
        gameTimer.StopTimer();
        canTap = false;
        pausePanel.SetActive(true);
        pausePanel.transform.SetAsLastSibling();
        panelText.text = "You Lost! Continue?";
    }

    private void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator ResumeGame()
    {
        if (nextStage)
        {
            settingsPanel.SetActive(false);
            gameTimer.ResumeTimer();
        }
        if (!nextStage && !isCycling)
        {
            settingsPanel.SetActive(false);
            if (!gameTimer.GetIsRunning())
            {
                gameTimer.ResumeTimer();
            }
            canTap = true;
        }
        if (!nextStage && isCycling)
        {
            isCycling = false;
            settingsPanel.SetActive(false);
            restartStageButton.enabled = false;
            feedbackText.text = "3";
            yield return new WaitForSeconds(1);
            feedbackText.text = "2";
            yield return new WaitForSeconds(1);
            feedbackText.text = "1";
            yield return new WaitForSeconds(1);
            gameTimer.ResumeTimer();
            isCycling = true;
            restartStageButton.enabled = true;
            canTap = true;
        }
        yield return null;
    }

    void ExitGame() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Stage_Select");
    }

    void SetArrowDirection(string direction)
    {
        Vector3 rotation = Vector3.zero;
        
        switch (direction)
        {
            case "Up":
                rotation = new Vector3(0, 0, 0); 
                break;
            case "Down":
                rotation = new Vector3(0, 0, 180); 
                break;
            case "Left":
                rotation = new Vector3(0, 0, 90);
                break;
            case "Right":
                rotation = new Vector3(0, 0, -90); 
                break;
        }
        
        directionArrow.transform.rotation = Quaternion.Euler(rotation);
    }

    void Awake()
    {
        GetData();
        sceneName= SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: "+ sceneName);
        InitilizeStageData();
        InitializeStageUi();
        SetupButtons();
    }

    void Start()
    {
        StaticData.isOnHigherOrderGame = true;
        //StaticData.isOnHigherOrder = false;
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
            LostGame();
        }
    }

    // Creates buttons, destroys previous buttons as well
    void SetupButtons()
    {
        foreach (Transform child in buttonsParent)
            Destroy(child.gameObject);
        buttons.Clear();

        for (int i = StaticData.constant[stageNum] + 1; i <= maxNumber + StaticData.constant[stageNum]; i++)
        {
            GameObject go = Instantiate(timePeriodButtonPrefab, buttonsParent);
            TimePeriodButton btn = go.GetComponent<TimePeriodButton>();

            btn.ButtonNumber = i;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
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
            currentSequence = new Sequence(maxNumber + StaticData.constant[stageNum], StaticData.coefficient[stageNum], StaticData.constant[stageNum]);
        }
        formulaText.text = $"Rule: {currentSequence.FormulaString}";

        pressedNumbers.Clear();

        // Pre-press first n numbers in the sequence, mark them as selected
        for (int i = 0; i < Mathf.Clamp(prePressedCount, 0, currentSequence.Numbers.Count); i++)
        {
            int num = currentSequence.Numbers[i];
            pressedNumbers.Add(num);
            buttons[num - 1 - StaticData.constant[stageNum]].SetGreen();
            buttons[num - 1 - StaticData.constant[stageNum]].SetSelected(true);
            buttons[num - 1 - StaticData.constant[stageNum]].SetPreSelected(true);
        }

        currentCycleIndex = 0;
        //isCycling = true;

        feedbackText.text = "Watch the sequence! Swipe the screen when the highlighted number is in the sequence.";
    }

    // Just to make time for the cycling
    IEnumerator DelayedStartCycle()
    {
        // More time to give the students time to read the tutorial
        if (stageNum <= 3)
        {
            yield return new WaitForSeconds(5f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        
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
                statusAnimator.SetBool("MissTrigger", false);
                statusAnimator.SetBool("AnticipateTrigger", false);
                statusAnimator.SetBool("HitTrigger", false);
                statusAnimator.SetBool("WrongTrigger", false);
                statusAnimator.SetBool("IdleTrigger", true);
            }
            
        
            HighlightButton(currentCycleIndex);

            Debug.Log("Cycle index: " + currentCycleIndex);

            int btnNumber = buttons[currentCycleIndex].ButtonNumber;
            bool inSequence = currentSequence.Numbers.Contains(btnNumber);

            Debug.Log($"Button Number: {btnNumber}, In Sequence: {inSequence}");

            // Show/hide swipe direction panel based on inSequence
            if (inSequence && currentSwipeIndex < expectedSwipeSequence.Count && !(buttons[currentCycleIndex].GetPreSelected() || buttons[currentCycleIndex].GetWasSelected()))
            {
                // Hint only shows if hintSeen is true
                if (StaticData.hintSeen[stageNum])
                {
                    swipeDirectionPanel.SetActive(true);
                    string expectedDirection = expectedSwipeSequence[currentSwipeIndex];
                    SetArrowDirection(expectedDirection);
                }
                
            }
            else
            {
                swipeDirectionPanel.SetActive(false);
            }

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
            statusAnimator.SetBool("IdleTrigger", true);
            soundEffectsManager.playIdleSound();

            while (timer < cycleInterval && currentCycleIndex >= 0)
            {
                if (hasNotClicked)
                {
                    string swipeDir = DetectSwipe();

                    if (swipeDir != null && canTap && !IsPointerOverInteractableUi())
                    {
                        Debug.Log("Swipe detected: " + swipeDir);
                        // Hide the swipe direction panel when user swipes
                        swipeDirectionPanel.SetActive(false);
                        HandleUserSwipe(swipeDir, btnNumber, inSequence, timer);
                        hasNotClicked = false;
                    }

                    if (!inSequence && hasNotClicked)
                    {
                        statusAnimator.SetBool("IdleTrigger", true);
                    }
                    else if (inSequence && hasNotClicked)
                    {
                        statusAnimator.SetBool("IdleTrigger", false);
                        
                        // Anticipation animation based on expected swipe direction
                        /*
                        string expectedDirection = expectedSwipeSequence[currentSwipeIndex];
                        if (expectedDirection == "Up" || expectedDirection == "Down")
                        {
                            statusAnimator.SetBool("VerticalTrigger", true);
                        }
                        else
                        {
                            statusAnimator.SetBool("HorizontalTrigger", true);
                        }
                        */

                        statusAnimator.SetBool("AnticipateTrigger", true);
                        // If the Sequence was pre pressed, automatically plays hit animation
                        if ((buttons[currentCycleIndex].GetPreSelected() || buttons[currentCycleIndex].GetWasSelected()) && timer > 0.01f)
                        {
                            statusAnimator.SetBool("AnticipateTrigger", false);

                            // Hit animation based on expected swipe direction
                            /*
                            string expectedDirection = expectedSwipeSequence[currentSwipeIndex];
                            if (expectedDirection == "Up" || expectedDirection == "Down")
                            {
                                statusAnimator.SetBool("VerticalTrigger", true);
                            }
                            else
                            {
                                statusAnimator.SetBool("HorizontalTrigger", true);
                            }
                            */  

                            statusAnimator.SetBool("HitTrigger", true);
                            soundEffectsManager.playHitSound();
                            // Hide panel when auto-completing pre-selected
                            swipeDirectionPanel.SetActive(false);
                            hasNotClicked = false;
                        }
                        // if the player misses, plays miss animation
                        if (timer > cycleLeniency && !gotRight)
                        {
                            feedbackText.text = "You missed!";
                            statusAnimator.SetBool("AnticipateTrigger", false);

                            // Miss animation based on expected swipe direction
                            /*
                            string expectedDirection = expectedSwipeSequence[currentSwipeIndex];
                            if (expectedDirection == "Up" || expectedDirection == "Down")
                            {
                                statusAnimator.SetBool("VerticalTrigger", true);
                            }
                            else
                            {
                                statusAnimator.SetBool("HorizontalTrigger", true);
                            }
                            */

                            statusAnimator.SetBool("MissTrigger", true);
                            soundEffectsManager.playMissSound();
                            isCorrect = false;
                            currentSwipeIndex++;
                            // Hide panel on miss
                            swipeDirectionPanel.SetActive(false);
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
                    /*
                    if ((Input.GetMouseButtonDown(0) || IsScreenTapped()) && canTap && !IsPointerOverInteractableUi())
                    {
                        Debug.Log("Clicked: " + gameObject.name);
                        HandleUserTap(btnNumber, inSequence, timer);
                        hasNotClicked = false;
                    }
                    */
                }
                if (isCycling)
                {
                    timer += Time.deltaTime;
                }
                yield return null;
            }

            // Hide panel when cycle time is up
            swipeDirectionPanel.SetActive(false);

            // After cycle of 25 buttons, check if sequence complete
            if (currentCycleIndex == maxNumber - 1)
            {
                statusAnimator.SetBool("MissTrigger", false);
                statusAnimator.SetBool("AnticipateTrigger", false);
                statusAnimator.SetBool("HitTrigger", false);
                statusAnimator.SetBool("WrongTrigger", false);
                statusAnimator.SetBool("IdleTrigger", true);

                Debug.Log("pressedNumbers Numbers = " + pressedNumbers);
                Debug.Log("Current Sequence = " + currentSequence.Numbers);

                if (CheckSequenceComplete() && isCorrect)
                {
                    feedbackText.text = "Great job! Sequence completed!";
                    isStageFinished = true;
                    nextStageButton.gameObject.SetActive(true);
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
        foreach (var btn in buttons)
        {
            btn.SetHighlighted(false);
            btn.SetHeight(false);
            if(btn.GetWasSelected())
            {
                btn.SetGreen();
            }
            else if (btn.GetPreSelected())
            {
                btn.SetGreen();
            }
        }
        buttons[index].SetHighlighted(true);
        buttons[index].SetHeight(true);
    }

    //Detects Swipe
    private string DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                swipeEndPos = touch.position;
                Vector2 swipeDelta = swipeEndPos - swipeStartPos;

                if (swipeDelta.magnitude >= minSwipeDistance)
                {
                    float x = swipeDelta.x;
                    float y = swipeDelta.y;

                    if (Mathf.Abs(x) > Mathf.Abs(y))
                    {
                        return x > 0 ? "Right" : "Left";
                    }
                    else
                    {
                        return y > 0 ? "Up" : "Down";
                    }
                }
            }
        }
        #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                swipeStartPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                swipeEndPos = (Vector2)Input.mousePosition;
                Vector2 swipeDelta = swipeEndPos - swipeStartPos;

                if (swipeDelta.magnitude >= minSwipeDistance)
                {
                    float x = swipeDelta.x;
                    float y = swipeDelta.y;

                    if (Mathf.Abs(x) > Mathf.Abs(y))
                    {
                        return x > 0 ? "Right" : "Left";
                    }
                    else
                    {
                        return y > 0 ? "Up" : "Down";
                    }
                }
            }
        #endif
        
        return null;
    }

    void HandleUserSwipe(string direction, int btnNumber, bool inSequence, float timer)
    {
        string expected = expectedSwipeSequence[currentSwipeIndex];
        bool gotSwipeCorrect = false;

        // Makes sure that Up and Down are interchangeable, same for Left and Right
        if (expected == "Up" || expected == "Down")
        {
            if (direction == "Up" || direction == "Down")
            {
                gotSwipeCorrect = true;
            }
        }
        else if (expected == "Left" || expected == "Right")
        {
            if (direction == "Left" || direction == "Right")
            {
                gotSwipeCorrect = true;
            }
        }

        if (pressedNumbers.Contains(btnNumber))
        {
            feedbackText.text = $"You already swiped for {btnNumber}.";
            return;
        }

        //Early swipes
        else if (currentSequence.Numbers.Contains(currentCycleIndex) && timer >= cycleLeniency / 2 && gotSwipeCorrect)
        {
            buttons[currentCycleIndex].SetGreen();
            buttons[currentCycleIndex].SetWasSelected(true);
            pressedNumbers.Add(btnNumber);
            buttons[currentCycleIndex].SetSelected(true);
            feedbackText.text = $"Correct swipe {direction} for: {btnNumber}!";
            currentSwipeIndex++;
            gotRight = true;

        }

        //Swipe not in sequence
        else if (!inSequence)
        {
            buttons[currentCycleIndex].SetHighlighted(true);
            feedbackText.text = $"Wrong swipe! {btnNumber} is not in the sequence.";
            statusAnimator.SetBool("IdleTrigger", false);
            statusAnimator.SetBool("WrongTrigger", true);
            soundEffectsManager.playMissSound();
            isCorrect = false;

            //Only decrease health if stage not finished
            if (!isStageFinished)
            {
                stageData.SetNumLives(stageData.GetNumLives() - 1);
                livesText.text = $"{stageData.GetNumLives()}";
                healthBar.SetHealth(stageData.GetNumLives());
            }
        }

        //Correct Swipes
        else if (inSequence)
        {
            if (currentSwipeIndex < expectedSwipeSequence.Count)
            {

                if (gotSwipeCorrect)
                {
                    buttons[currentCycleIndex].SetGreen();
                    buttons[currentCycleIndex].SetWasSelected(true);
                    pressedNumbers.Add(btnNumber);
                    buttons[currentCycleIndex].SetSelected(true);

                    feedbackText.text = $"Correct swipe {direction} for {btnNumber}!";
                    statusAnimator.SetBool("IdleTrigger", false);
                    
                    // Add different animations based on swipe direction
                    /*
                    if (expected == "Up" || expected == "Down")
                    {

                    }
                    else if (expected == "Left" || expected == "Right")
                    {

                    }
                    */

                    statusAnimator.SetBool("HitTrigger", true);
                    soundEffectsManager.playHitSound();
                    gotRight = true;

                    currentSwipeIndex++;
                }

                //Wrong Swipes But in Sequence
                else
                {
                    buttons[currentCycleIndex].SetHighlighted(true);
                    feedbackText.text = $"Wrong swipe!";
                    statusAnimator.SetBool("IdleTrigger", false);

                    // Add different miss animations based on swipe direction
                    /*
                    if (expected == "Up" || expected == "Down")
                    {

                    }
                    else if (expected == "Left" || expected == "Right")
                    {

                    }
                    */

                    statusAnimator.SetBool("MissTrigger", true);
                    soundEffectsManager.playMissSound();
                    isCorrect = false;
                    currentSwipeIndex++;

                    //Only decrease health if stage not finished
                    if (!isStageFinished)
                    {
                        stageData.SetNumLives(stageData.GetNumLives() - 1);
                        livesText.text = $"{stageData.GetNumLives()}";
                        healthBar.SetHealth(stageData.GetNumLives());
                    }
                }
            }

        }
    }

    void HandleUserTap(int btnNumber, bool inSequence, float timer)
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
        else if (currentSequence.Numbers.Contains(btnNumber + 1) && timer >= cycleLeniency/2)
        {
            buttons[btnNumber].SetGreen();
            buttons[btnNumber].SetWasSelected(true);
            pressedNumbers.Add(btnNumber + 1);
            buttons[btnNumber].SetSelected(true);
            feedbackText.text = $"You pressed the right number: {btnNumber + 1}!";
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
        currentSwipeIndex = 0;
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
            buttons[num - 1 - StaticData.constant[stageNum]].SetGreen();
            buttons[num - 1 - StaticData.constant[stageNum]].SetSelected(true);
        }

        if(!isStageFinished){
            stageData.SetNumRestarts(stageData.GetNumRestarts() + 1);
            restartText.text = $"{stageData.GetNumRestarts()}";
        } 

        statusAnimator.SetBool("MissTrigger", false);
        statusAnimator.SetBool("AnticipateTrigger", false);
        statusAnimator.SetBool("HitTrigger", false);
        statusAnimator.SetBool("WrongTrigger", false);
        statusAnimator.SetBool("IdleTrigger", true);
        
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
        {
            btn.SetHighlighted(false);
            btn.SetHeight(false);
            if(currentSequence.Numbers.Contains(btn.ButtonNumber))
            {
                btn.SetGreen();
            }
        }

        foreach (Transform child in buttonsParent2)
            Destroy(child.gameObject);
        buttons2.Clear();

        for (int i = StaticData.constant[stageNum] + 1; i <= maxNumber + StaticData.constant[stageNum]; i++)
        {
            GameObject go2 = Instantiate(timePeriodButtonPrefab, buttonsParent2);
            TimePeriodButton btn2 = go2.GetComponent<TimePeriodButton>();

            btn2.ButtonNumber = i;
            btn2.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            btn2.SetHighlighted(false);
            buttons2.Add(btn2);
        }
        
        /*

        if (!StaticData.refSeen)
        {
            matchText.SetActive(false);
            buttonsParent2.transform.localScale = new Vector3(0, 0, 0);
            correctText.transform.localPosition = new Vector3(0, 100, 0);
        }
        else
        {
            buttonsParent.localPosition = new Vector3(0, -500, 0);
        }*/

        // Show formula panel with current sequence
        statusAnimator.enabled = false;
        formulaPanel.gameObject.SetActive(true);
        formulaPanel.ShowPanel(currentSequence, gameTimer, stageData, buttons2);
    }
}
