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

    [Header("Tutorial")]
    public GameObject tutorialManager;
    public GameObject tutorialManager2;

    public TutorialManager tm;
    public TutorialManager tm2;

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

    [Header("Endscreen Animator")]
    public EndScreenAnimator endScreenAnimator;

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

    private int passedPrePressed = 0;
    private float minSwipeDistance = 50f; 

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
            if ((result.gameObject.GetComponent<Button>() != null || result.gameObject.GetComponent<Toggle>() != null) && !result.gameObject.GetComponent<TimePeriodButton>() && !result.gameObject.GetComponent<TutorialManager>()) 
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
        isRandomSequence = StaticData.isRandomSequence[stageNum];
        if (isRandomSequence)
        {
            Debug.Log("Random Sequence is true");
            var dm = new HODifficultyManager();
            dm.DifficultyPicker(Random.Range(1, 4));
            prePressedCount = StaticData.prePressedCount[stageNum];
            isFormulaSeen = StaticData.isFormulaSeen[stageNum];
            formulaInputPanel.SetLockCoefficient(StaticData.lockCoefficient[stageNum]);
            formulaInputPanel.SetLockConstant(StaticData.lockConstant[stageNum]);
            expectedSwipeSequence = StaticData.stageSwipes[stageNum];    
        }
        else
        {
            maxNumber = StaticData.maxNumber[stageNum];
            Debug.Log("Max Number: " + StaticData.maxNumber[stageNum] + " for stage " + (StaticData.stageNum));
            prePressedCount = StaticData.prePressedCount[stageNum];
            isFormulaSeen = StaticData.isFormulaSeen[stageNum];
            formulaInputPanel.SetLockCoefficient(StaticData.lockCoefficient[stageNum]);
            formulaInputPanel.SetLockConstant(StaticData.lockConstant[stageNum]);
            expectedSwipeSequence = StaticData.stageSwipes[stageNum];            
        }
        cycleInterval = StaticData.cycleInterval;
        cycleLeniency = StaticData.cycleLeniency;
    }
    
    void InitializeRandomData()
    {
        stageData = new HOStageData();
        stageData.SetStageNum(StaticData.stageNum);
        stageData.SetNumRestarts(StaticData.stageRestarts[stageNum]);
        stageData.SetElapsedTime(StaticData.stageTime[stageNum]);
        stageData.SetNumLives(StaticData.stageLives[stageNum]);
        Time.timeScale = 1;
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
        settingsConfirmButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        settingsExitButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        exitButton.onClick.AddListener(() => ExitGame());
        restartGameButton.onClick.AddListener(() => RestartGame());

        feedbackText.text = "I-tap ang screen para magsimula!";
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
        gameTimer.StopTimer();
        canTap = false;

        endScreenAnimator.gameObject.SetActive(true);

        StartCoroutine(LoadEndScreenAnimation());
    }

    private IEnumerator LoadEndScreenAnimation()
    {
        yield return new WaitForSeconds(2f);

        pausePanel.SetActive(true);
        pausePanel.transform.SetAsLastSibling();
        panelText.text = "Sayang naubusan ka ng buhay! Gusto mo bang ulitin ang stage?";

        restartGameButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
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
        sceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: " + sceneName);
        Debug.Log("Stage Number: " + stageNum);
        Debug.Log("Number of stages done: " + StaticData.numStageDone);
        if (StaticData.numStageDone <= stageNum)
        {
            Debug.Log("Stage Never Been done before, should have a new save");
        }
        if(!isRandomSequence)
        {
            InitilizeStageData();
        }
        else
        {
            InitializeRandomData();
        }
        
        InitializeStageUi();
        SetupButtons();
        currentCycleIndex = 0;
        statusAnimator.SetBool("Idle_Trigger", true);
    }

    void Start()
    {
        StaticData.isOnHigherOrderGame = true;
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || IsScreenTapped()) && !IsPointerOverInteractableUi() && isStart)
        {
            isStart = false;

            if (StaticData.numStageDone == 0 && StaticData.stageNum == 0)
            {
                tm.OpenTutorial();
            }
            
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
            currentSequence = new Sequence(maxNumber + StaticData.constant[stageNum], StaticData.coefficient[stageNum], StaticData.constant[stageNum]);
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

        feedbackText.text = "Tignan ang sequence! I-Swipe ang screen kapag ang number ay nasa sequence.";
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
        feedbackText.text = "Inuulit ang stage...";
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

    public void ResetAnims()
    {
        statusAnimator.SetBool("Early_Trigger", false);
        statusAnimator.SetBool("Anticipate_Hori_Trigger", false);
        statusAnimator.SetBool("Hit_Hori_Trigger", false);
        statusAnimator.SetBool("Anticipate_Vert_Trigger", false);
        statusAnimator.SetBool("Hit_Vert_Trigger", false);
        statusAnimator.SetBool("Wrong_Trigger", false);
        statusAnimator.SetBool("Idle_Trigger", false);
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

            if (currentCycleIndex <= 0)
            {
                yield return new WaitForSeconds(1f);
                ResetAnims();
                statusAnimator.SetBool("Idle_Trigger", true);
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
            if (StaticData.stageNum < 3)
            {
                feedbackText.text = inSequence ? $"{btnNumber} ay parte ng sequence." : $"{btnNumber} ay HINDI parte ng sequence.";
            }
            else
            {
                feedbackText.text = "";
            }
            
            // Wait for cycleInterval seconds and listen for input 
            timer = 0f;

            bool hasNotClicked = true;

            gotRight = false;

            ResetAnims();
            //statusAnimator.SetBool("Idle_Trigger", true);
            soundEffectsManager.playIdleSound();

            while (timer < StaticData.cycleInterval && currentCycleIndex >= 0)
            {
                if (hasNotClicked)
                {
                    string swipeDir = DetectSwipe();

                    if (swipeDir != null && canTap && !IsPointerOverInteractableUi())
                    {
                        Debug.Log("Swipe detected: " + swipeDir);
                        // Hide the swipe direction panel when user swipes
                        isCycling = false;
                        swipeDirectionPanel.SetActive(false);
                        HandleUserSwipe(swipeDir, btnNumber, inSequence, timer);
                        hasNotClicked = false;
                    }

                    if (!inSequence && hasNotClicked)
                    {   
                        string expectedDirection = expectedSwipeSequence[currentSwipeIndex];
                        ResetAnims();
                        if (expectedDirection == "Up" || expectedDirection == "Down")
                        {
                            statusAnimator.SetBool("Anticipate_Vert_Trigger", true);
                        }
                        else
                        {
                            statusAnimator.SetBool("Anticipate_Hori_Trigger", true);
                        }             
                    }
                    else if (inSequence && hasNotClicked)
                    {
                        ResetAnims();
                        string expectedDirection = expectedSwipeSequence[currentSwipeIndex];
                        if (expectedDirection == "Up" || expectedDirection == "Down")
                        {
                            statusAnimator.SetBool("Anticipate_Vert_Trigger", true);
                        }
                        else
                        {
                            statusAnimator.SetBool("Anticipate_Hori_Trigger", true);
                        }
                    
                        // If the Sequence was prepressed, automatically plays hit animation
                        if ((buttons[currentCycleIndex].GetPreSelected() || buttons[currentCycleIndex].GetWasSelected()) && timer > 0.01f)
                        {
                            ResetAnims();
                            // Hit animation based on expected swipe direction
                            if (expectedDirection == "Up" || expectedDirection == "Down")
                            {
                                statusAnimator.SetBool("Hit_Vert_Trigger", true);
                                soundEffectsManager.playHitSound();
                            }
                            else
                            {
                                if (sceneName == "HO_BotFightScene")
                                {
                                    soundEffectsManager.playMissSound();
                                }
                                else
                                {
                                    soundEffectsManager.playHitSound();
                                }
                                statusAnimator.SetBool("Hit_Hori_Trigger", true);
                            }
                            
                            // Hide panel when auto-completing pre-selected
                            swipeDirectionPanel.SetActive(false);
                            hasNotClicked = false;
                            passedPrePressed++;
                            currentSwipeIndex++;
                        }
                        // if the player misses, plays miss animation
                        if (timer > StaticData.cycleLeniency && !gotRight)
                        {
                            feedbackText.text = "Hala mali! Hindi ka nakaswipe!";
                            ResetAnims();

                            // Miss animation based on expected swipe direction

                            if (expectedDirection == "Up" || expectedDirection == "Down")
                            {
                                if (sceneName == "HO_BotFightScene")
                                {
                                    soundEffectsManager.playMissSound();
                                    statusAnimator.SetBool("Wrong_Trigger", true);
                                }
                                else
                                {
                                    soundEffectsManager.playSwingSounds();
                                    statusAnimator.SetBool("Early_Trigger", true);
                                }
                                
                            }
                            else
                            {


                                if (sceneName == "HO_BotTennisSceneNew")
                                {
                                    soundEffectsManager.playMissSound();
                                    statusAnimator.SetBool("Wrong_Trigger", true);
                                }
                                else
                                {
                                    statusAnimator.SetBool("Early_Trigger", true);
                                    soundEffectsManager.playSwingSounds();
                                }
                            }
                            
                            buttons[currentCycleIndex].SetRed();
                            buttons[currentCycleIndex].toggleWrong();
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
                ResetAnims();
                statusAnimator.SetBool("Idle_Trigger", true);
                Debug.Log("pressedNumbers Numbers = " + pressedNumbers);
                Debug.Log("Current Sequence = " + currentSequence.Numbers);

                if (CheckSequenceComplete() && isCorrect)
                {
                    feedbackText.text = "Yehey! Natapos ang sequence!";
                    isStageFinished = true;
                    nextStageButton.gameObject.SetActive(true);
                    nextStageButton.onClick.AddListener(() => OnNextStageButtonClicked());
                    canTap = false;
                    currentSwipeIndex = 0;
                }
                else
                {
                    Debug.Log("Wrong Sequence");
                    feedbackText.text = "May mali sa sequence ata? Inuulit ang stage...";
                    yield return new WaitForSeconds(2f);
                    ResetSequence();
                }
                
                yield return new WaitForSeconds(0.5f);
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
            if(btn.GetWrong())
            {
                btn.SetRed();
            }
            if (btn.GetWasSelected())
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
        Debug.Log("Swiped at " + timer + " seconds. Cycle Leniency: " + StaticData.cycleLeniency);
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

        //Early swipes
/*
        else if (currentSequence.Numbers.Contains(buttons[currentCycleIndex].ButtonNumber + 1) && gotSwipeCorrect && buttons[currentCycleIndex + 1].GetPreSelected() == false)
        {
            buttons[currentCycleIndex + 1].SetGreen();
            buttons[currentCycleIndex + 1].SetWasSelected(true);
            pressedNumbers.Add(btnNumber + 1);
            buttons[currentCycleIndex + 1].SetSelected(true);
            feedbackText.text = $"Correct swipe {direction} for: {btnNumber + 1}!";
            currentSwipeIndex++;
            gotRight = true;

        }
*/
        //Swipe not in sequence
        if (!inSequence)
        {
            buttons[currentCycleIndex].SetRed();
            buttons[currentCycleIndex].toggleWrong();
            feedbackText.text = $"Maling swipes! {btnNumber} ay hindi nasa sequence.";
            ResetAnims();
            statusAnimator.SetBool("Early_Trigger", true);
            if (sceneName == "HO_Botfight")
            {
                soundEffectsManager.playMissSound();
            }
            else
            {
                soundEffectsManager.playSwingSounds();
            }
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
                ResetAnims();

                if (gotSwipeCorrect)
                {
                    buttons[currentCycleIndex].SetGreen();
                    buttons[currentCycleIndex].SetWasSelected(true);
                    pressedNumbers.Add(btnNumber);
                    buttons[currentCycleIndex].SetSelected(true);

                    feedbackText.text = $"Tamang swipe sa {btnNumber}!";

                    if (expected == "Up" || expected == "Down")
                    {
                        if (sceneName == "HO_BotFightScene")
                        {
                            statusAnimator.SetBool("Hit_Vert_Trigger", true);
                            soundEffectsManager.playHitSound();
                        }
                        else
                        {
                            statusAnimator.SetBool("Hit_Vert_Trigger", true);     
                            soundEffectsManager.playHitSound();                       
                        }
                        
                    }
                    else if (expected == "Left" || expected == "Right")
                    {
                        if (sceneName == "HO_BotFightScene")
                        {
                            soundEffectsManager.playMissSound();
                        }
                        else
                        {
                            soundEffectsManager.playHitSound();
                        }
                        statusAnimator.SetBool("Hit_Hori_Trigger", true);
                    }
                    
                    Debug.Log("SFX and animation should have played");
                    gotRight = true;
                    currentSwipeIndex++;
                }

                //Wrong Swipes But in Sequence
                else
                {
                    ResetAnims();
                    buttons[currentCycleIndex].SetHighlighted(true);
                    buttons[currentCycleIndex].SetRed();
                    buttons[currentCycleIndex].toggleWrong();
                    feedbackText.text = $"Maling swipe!";

                    if (expected == "Up" || expected == "Down")
                    {
                        if (sceneName == "HO_BotFightScene")
                        {
                            statusAnimator.SetBool("Wrong_Trigger", true);
                            soundEffectsManager.playMissSound();
                        }
                        else
                        {
                            statusAnimator.SetBool("Early_Trigger", true);
                            soundEffectsManager.playSwingSounds();
                        }
                        
                    }
                    else if (expected == "Left" || expected == "Right")
                    {
                        if (sceneName == "HO_BotFightScene")
                        {
                            statusAnimator.SetBool("Early_Trigger", true);
                            soundEffectsManager.playSwingSounds();
                        }
                        else
                        {
                            statusAnimator.SetBool("Early_Trigger", true);
                            soundEffectsManager.playSwingSounds();
                        }
                    }
                    
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
        isCycling = true;
    }

    /*
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
            statusAnimator.SetBool("Idle_Trigger", false);
            statusAnimator.SetBool("HitTrigger", true);
            soundEffectsManager.playHitSound();
            gotRight = true;
        }
        else if (currentSequence.Numbers.Contains(btnNumber + 1) && timer >= StaticData.cycleLeniency/2)
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
            statusAnimator.SetBool("Idle_Trigger", false);
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
    */

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
        passedPrePressed = 0;
        restartStageButton.enabled = false;
        //pauseButton.enabled = false;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetHighlighted(false);
            buttons[i].SetSelected(false);
            buttons[i].SetWasSelected(false);
            buttons[i].SetHeight(false);
            buttons[i].GetComponent<Image>().sprite = unpressedSprite;
            if(buttons[i].GetWrong())
                buttons[i].toggleWrong();
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
        tutorialManager.SetActive(false);
        tutorialManager2.SetActive(true);
        formulaPanel.ShowPanel(currentSequence, gameTimer, stageData, buttons2);

        if(StaticData.numStageDone == 0 && stageNum == 0)
        {
            tm2.OpenTutorial();
        }
    }
}
