using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEditor.Overlays;
using Unity.VisualScripting;

public class SpeedCalibrationManager : MonoBehaviour, IDataPersistence
{
    [Header("Swipes")]
    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private List<string> expectedSwipeSequence;
    private int currentSwipeIndex = 0;
    private float minSwipeDistance = 10f; 

    [Header("Swipe Direction Panel")]
    public GameObject swipeDirectionPanel; 
    public GameObject directionArrow;

    [Header("Raycasts")]
    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;

    [Header("UI & Prefabs")]
    public GameObject timePeriodButtonPrefab;
    public Transform buttonsParent;
    public TextMeshProUGUI feedbackText, speedText;
    public Button nextStageButton;
    public Button speedUpButton, speedDownButton;

    [Header("Animations")]
    public Animator statusAnimator;

    [Header("Settings")]
    public int maxNumber = 12;
    public int prePressedCount = 2;
    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();
    private Sequence currentSequence;
    private List<int> pressedNumbers = new List<int>();
    private bool isCycling = false, canTap = true;
    private int currentCycleIndex = 0;

    private readonly float[] speedValues = { 2f, 1.5f, 1.2f, 1f, 0.75f };
    private readonly string[] speedLabels = { "Slowest", "Slow", "Average", "Fast", "Fastest" };
    private int currentSpeedIndex = 2; // Start at "Average"

    [Header("Audio Files")]
    public SoundEffectsManager soundEffectsManager;

    private void Awake()
    {
        StaticData.isOnHigherOrderGame = true;
        expectedSwipeSequence = StaticData.stageSwipes[0];
        StaticData.cycleInterval = speedValues[currentSpeedIndex];
        StaticData.cycleLeniency = StaticData.cycleInterval - StaticData.cycleInterval / 4;
        SetupButtons();
        InitializeUI();
        StartCoroutine(StartNewSequence());
        isCycling = true; 
    }

    private void InitializeUI()
    {
        feedbackText.text = "This game will be a rhythm game, please tap the screen to adjust to your preferred speed.";
        UpdateSpeedText();

        // Speed adjustment buttons
        speedUpButton.onClick.AddListener(() =>
        {
            if (currentSpeedIndex < speedValues.Length - 1)
                currentSpeedIndex++;
            ApplySpeedSetting();
        });

        speedDownButton.onClick.AddListener(() =>
        {
            if (currentSpeedIndex > 0)
                currentSpeedIndex--;
            ApplySpeedSetting();
        });

        // Next Stage button
        nextStageButton.onClick.AddListener(() =>
        {
            feedbackText.text = "Loading next stage...";
            DataPersistenceManager.Instance.SaveGame();
            SceneManager.LoadScene("HO_BotFightScene");
        });
    }

    private void ApplySpeedSetting()
    {
        StaticData.cycleInterval = speedValues[currentSpeedIndex];
        StaticData.cycleLeniency = StaticData.cycleInterval - StaticData.cycleInterval / 4;
        UpdateSpeedText();
    }

    private void UpdateSpeedText()
    {
        speedText.text = $"{speedLabels[currentSpeedIndex]}";
    }

    private void SetupButtons()
    {
        foreach (Transform child in buttonsParent)
            Destroy(child.gameObject);
        buttons.Clear();

        for (int i = 1; i <= maxNumber; i++)
        {
            GameObject go = Instantiate(timePeriodButtonPrefab, buttonsParent);
            TimePeriodButton btn = go.GetComponent<TimePeriodButton>();
            btn.ButtonNumber = i;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            btn.SetHighlighted(false);
            buttons.Add(btn);
        }
    }

    private IEnumerator StartNewSequence()
    {
        currentSequence = new Sequence(maxNumber, 2, 0);

        pressedNumbers.Clear();
        for (int i = 0; i < Mathf.Clamp(prePressedCount, 0, currentSequence.Numbers.Count); i++)
        {
            int num = currentSequence.Numbers[i];
            pressedNumbers.Add(num);
            buttons[num - 1].SetGreen();
            buttons[num - 1].SetPreSelected(true);
        }

        feedbackText.text = "Welcome to the Speed Calibration! Please tap the arrow buttons to adjust the speed.";
        yield return new WaitForSeconds(3f);
        feedbackText.text = "You can change the speed at any time in the settings menu under Rhythm.";
        yield return new WaitForSeconds(3f);
        StartCoroutine(CycleButtons());
    }

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
                currentSwipeIndex = 0;
                statusAnimator.SetBool("MissTrigger", false);
                statusAnimator.SetBool("AnticipateTrigger", false);
                statusAnimator.SetBool("HitTrigger", false);
                statusAnimator.SetBool("WrongTrigger", false);
                statusAnimator.SetBool("IdleTrigger", true);

                foreach (var btn in buttons)
                {
                    btn.SetHighlighted(false);
                    btn.SetWasSelected(false);
                    btn.SetHeight(false);
                    
                    if(btn.GetWrong())
                        btn.toggleWrong();
                }
                
                pressedNumbers.Clear();

                for (int i = 0; i < Mathf.Clamp(prePressedCount, 0, currentSequence.Numbers.Count); i++)
                {
                    int num = currentSequence.Numbers[i];
                    pressedNumbers.Add(num);
                    buttons[num - 1].SetGreen();
                    buttons[num - 1].SetPreSelected(true);
                }
                
                feedbackText.text = "Get ready!";
                yield return new WaitForSeconds(2f);
            }


            HighlightButton(currentCycleIndex);

            Debug.Log("Cycle index: " + currentCycleIndex);

            int btnNumber = buttons[currentCycleIndex].ButtonNumber;
            bool inSequence = currentSequence.Numbers.Contains(btnNumber);

            Debug.Log($"Button Number: {btnNumber}, In Sequence: {inSequence}");

            // Show/hide swipe direction panel based on inSequence
            if (inSequence && currentSwipeIndex < expectedSwipeSequence.Count && !(buttons[currentCycleIndex].GetPreSelected() || buttons[currentCycleIndex].GetWasSelected()))
            {  
                swipeDirectionPanel.SetActive(true);
                string expectedDirection = expectedSwipeSequence[currentSwipeIndex];
                SetArrowDirection(expectedDirection);
            }
            else
            {
                swipeDirectionPanel.SetActive(false);
            }

            // Show feedback for cycle step
            feedbackText.text = inSequence ? $"Number {btnNumber} is part of the sequence." : $"Number {btnNumber} is NOT part of the sequence.";

            // Wait for cycleInterval seconds and listen for input 
            float timer = 0f;

            bool hasNotClicked = true;

            statusAnimator.SetBool("MissTrigger", false);
            statusAnimator.SetBool("AnticipateTrigger", false);
            statusAnimator.SetBool("HitTrigger", false);
            statusAnimator.SetBool("WrongTrigger", false);
            statusAnimator.SetBool("IdleTrigger", true);
            soundEffectsManager.playIdleSound();

            while (timer < StaticData.cycleInterval && currentCycleIndex >= 0)
            {
                if (hasNotClicked)
                {
                    string swipeDir = DetectSwipe();

                    if (swipeDir != null && canTap)
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
                        if (timer > StaticData.cycleLeniency)
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
                            buttons[currentCycleIndex].SetRed();
                            buttons[currentCycleIndex].toggleWrong();
                            soundEffectsManager.playMissSound();
                            currentSwipeIndex++;
                            // Hide panel on miss
                            swipeDirectionPanel.SetActive(false);
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

            if(currentCycleIndex == maxNumber - 1)
            {
                nextStageButton.gameObject.SetActive(true);
            }

            currentCycleIndex = (currentCycleIndex + 1) % maxNumber;
        }
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

        if (pressedNumbers.Contains(btnNumber))
        {
            feedbackText.text = $"You already swiped for {btnNumber}.";
            return;
        }

        //Early swipes
        else if (currentSequence.Numbers.Contains(buttons[currentCycleIndex].ButtonNumber + 1) && gotSwipeCorrect && buttons[currentCycleIndex + 1].GetPreSelected() == false)
        {
            buttons[currentCycleIndex + 1].SetGreen();
            buttons[currentCycleIndex + 1].SetWasSelected(true);
            pressedNumbers.Add(btnNumber + 1);
            buttons[currentCycleIndex + 1].SetSelected(true);
            feedbackText.text = $"Correct swipe {direction} for: {btnNumber + 1}!";
            currentSwipeIndex++;

        }

        //Swipe not in sequence
        else if (!inSequence)
        {
            buttons[currentCycleIndex].SetRed();
            buttons[currentCycleIndex].toggleWrong();
            feedbackText.text = $"Wrong swipe! {btnNumber} is not in the sequence.";
            statusAnimator.SetBool("IdleTrigger", false);
            statusAnimator.SetBool("WrongTrigger", true);
            soundEffectsManager.playMissSound();
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
                    statusAnimator.SetBool("AnticipateTrigger", false);

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
                    Debug.Log("SFX and animation should have played");
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
                    currentSwipeIndex++;
                    //Only decrease health if stage not finished
                }
            }

        }
        isCycling = true;
    }

    private void HighlightButton(int index)
    {
        foreach (var btn in buttons)
        {
            btn.SetHighlighted(false);
            btn.SetHeight(false);
            if (btn.GetPreSelected() || btn.GetWasSelected())
                btn.SetGreen();
            if (btn.GetWrong())
                btn.SetRed();
        }
        buttons[index].SetHighlighted(true);
        buttons[index].SetHeight(true);
    }
        public void LoadData(GameData data)
    {
        data.stageSpeed = StaticData.cycleInterval;
    }

    public void SaveData(ref GameData data)
    {
        data.stageSpeed = StaticData.cycleInterval;
        StaticData.cycleLeniency = StaticData.cycleInterval - StaticData.cycleInterval / 4;
    }
}

