using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SequenceGameManager : MonoBehaviour
{
    [Header("Raycasts")]
    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;
    [Header("UI & Prefabs")]
    public GameObject timePeriodButtonPrefab;
    public Transform buttonsParent;   
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI formulaText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI restartText;
    public Button nextStageButton;
    public Button restartStageButton;
    public FormulaInputPanel formulaPanel;
    public GameTimer gameTimer;

    [Header("Central Animator")]
    public Animator statusAnimator;

    [Header("Settings")]
    private int maxNumber = 25;
    private float cycleInterval = 1;
    private float cycleLeniency = 0.4f;
    private int prePressedCount = 0;
    private bool isFormulaSeen = true;
    private bool isRandomSequence = true;

    [Header("Audio Files")]
    public SoundEffectsManager soundEffectsManager;

    private Sequence currentSequence;

    public FormulaInputPanel formulaInputPanel;

    private List<TimePeriodButton> buttons = new List<TimePeriodButton>();
    private int currentCycleIndex = 0;
    private List<int> pressedNumbers = new List<int>();

    private bool isCycling = false;

    private bool isCorrect = true;

    private bool canTap = true;

    private bool isStart = true;

    private bool gotRight = false;

    private float timer;

    private int numLives = 1;

    private int numRestarts = 0;

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
            if (result.gameObject.GetComponent<Button>() != null || result.gameObject.GetComponent<Toggle>() != null)
            {
                return true;
            }
        }

        return false;
    }

    public void SetIsRandomSequence(bool rand)
    {
        isRandomSequence = rand;
    }

    public void GetData()
    {
        maxNumber = StaticData.maxNumber;
        cycleInterval = StaticData.cycleInterval;
        cycleLeniency = StaticData.cycleLeniency;
        prePressedCount = StaticData.prePressedCount;
        isFormulaSeen = StaticData.isFormulaSeen;
        isRandomSequence = StaticData.isRandomSequence;
        formulaInputPanel.SetLockCoefficient(StaticData.lockCoefficient);
        formulaInputPanel.SetLockConstant(StaticData.lockConstant);
    }
  
    void Start()
    {
        GetData();
        formulaText.gameObject.SetActive(isFormulaSeen);
        formulaPanel.gameObject.SetActive(false);
        nextStageButton.gameObject.SetActive(false);
        //restartStageButton.gameObject.SetActive(false);
        restartStageButton.enabled = false;
        restartStageButton.onClick.AddListener(() => { isCycling = false; ResetSequence(); Debug.Log("NumRestarts" + numRestarts); });
        feedbackText.text = "Please tap screen to start game";
        SetupButtons();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && !IsPointerOverInteractableUi() && isStart)
        {
            isStart = false;
            StartNewSequence();
            StartCoroutine(DelayedStartCycle());
        }
        livesText.text = $"{numLives}";
        restartText.text = $"{numRestarts}";
        /*if (isCycling)
        {
            restartStageButton.onClick.AddListener(() => { isCycling = false; ResetSequence();});
        }*/
        if (numLives == 0)
        {
            isCycling = false;
            feedbackText.text = "You lost all your lives!";
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
        isCycling = true;

        feedbackText.text = "Watch the sequence! Tap the screen when the highlighted number is in the sequence.";
        //nextStageButton.gameObject.SetActive(false);
    }

    // Clears all things the selections in the time periods and reselects those that need to be prepressed
    void RestartSequence()
    {
        canTap = true;
        pressedNumbers.Clear();

        // Clear all buttons
        for (int i = 0; i < maxNumber; i++)
        {
            buttons[i].SetSelected(false);
            buttons[i].SetHighlighted(false);
            buttons[i].SetWasSelected(false);
        }

        // Pre-press first n numbers in the sequence, mark them as selected
        for (int i = 0; i < Mathf.Clamp(prePressedCount, 0, currentSequence.Numbers.Count); i++)
        {
            int num = currentSequence.Numbers[i];
            pressedNumbers.Add(num);
            buttons[num - 1].SetGreen();
            buttons[num - 1].SetSelected(true);
        }

        currentCycleIndex = -1;

        isCycling = true;

        feedbackText.text = "Restarting Sequence";
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
        gameTimer.StartTimer();
        restartStageButton.enabled = true;
    }

    // Main loop for the cycling
    IEnumerator CycleButtons()
    {
        while (true)
        {
            if (!isCycling)
            {
                yield return null;
                continue;
            }


            if (currentCycleIndex == 0)
            {
                yield return new WaitForSeconds(1f);
            }

            HighlightButton(currentCycleIndex);

            int btnNumber = currentCycleIndex + 1;
            bool inSequence = currentSequence.Numbers.Contains(btnNumber);

            // Show feedback for cycle step
            feedbackText.text = inSequence ? $"Number {btnNumber} is part of the sequence." : $"Number {btnNumber} is NOT part of the sequence.";

            // Wait for cycleInterval seconds and listen for input 
            timer = 0f;

            bool fuckingStop = true;

            gotRight = false;

            soundEffectsManager.playIdleSound();

            while (timer < cycleInterval)
            {
                if (fuckingStop)
                {
                    if (!inSequence)
                    {
                        statusAnimator.SetBool("IdleTrigger", true);
                    }
                    else if (inSequence)
                    {
                        statusAnimator.SetBool("IdleTrigger", false);
                        statusAnimator.SetBool("AnticipateTrigger", true);
                        if ((buttons[currentCycleIndex].GetPreSelected() || buttons[currentCycleIndex].GetWasSelected()) && timer > 0.10f)
                        {
                            statusAnimator.SetBool("AnticipateTrigger", false);
                            statusAnimator.SetBool("HitTrigger", true);
                            soundEffectsManager.playHitSound();
                            fuckingStop = false;
                        }
                        if (timer > cycleLeniency && !gotRight)
                        {
                            feedbackText.text = "You missed!";
                            statusAnimator.SetBool("AnticipateTrigger", false);
                            statusAnimator.SetBool("MissTrigger", true);
                            numLives -= 1;
                            soundEffectsManager.playMissSound();
                            fuckingStop = false;
                        }
                    }
                    if (Input.GetMouseButtonDown(0) && canTap && !IsPointerOverInteractableUi())
                    {
                        Debug.Log("Clicked: " + gameObject.name);
                        HandleUserTap(btnNumber, inSequence);
                        fuckingStop = false;
                    }
                }
                timer += Time.deltaTime;
                yield return null;
            }

            statusAnimator.SetBool("MissTrigger", false);
            statusAnimator.SetBool("AnticipateTrigger", false);
            statusAnimator.SetBool("HitTrigger", false);
            statusAnimator.SetBool("WrongTrigger", false);
            statusAnimator.SetBool("IdleTrigger", true);



            // After cycle of 25 buttons, check if sequence complete
            if (currentCycleIndex == maxNumber - 1)
            {
                if (CheckSequenceComplete() && isCorrect)
                {
                    feedbackText.text = "Great job! Sequence completed!";
                    nextStageButton.gameObject.SetActive(true);
                    restartStageButton.gameObject.SetActive(true);
                    nextStageButton.onClick.AddListener(() => OnNextStageButtonClicked());
                    canTap = false;
                }
                else
                {
                    feedbackText.text = "Sequence not complete or wrong taps. Restarting...";
                    yield return new WaitForSeconds(1f);
                    ResetSequence();
                }
            }

            currentCycleIndex = (currentCycleIndex + 1) % maxNumber;
        }
    }

    public void RestartStage()
    {
        RestartSequence();
        //restartStageButton.gameObject.SetActive(true);
        isCycling = true;
        //StartCoroutine(DelayedStartCycle());
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
            numLives -= 1;
            isCorrect = false;
            soundEffectsManager.playMissSound();
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

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetHighlighted(false);
            buttons[i].SetSelected(false);
            buttons[i].SetWasSelected(false);
        }

        pressedNumbers.Clear();

        for (int i = 0; i < Mathf.Clamp(prePressedCount, 0, currentSequence.Numbers.Count); i++)
        {
            int num = currentSequence.Numbers[i];
            pressedNumbers.Add(num);
            buttons[num - 1].SetGreen();
            buttons[num - 1].SetSelected(true);
        }

        feedbackText.text = "Restarting Stage...";

        currentCycleIndex = -1;
        numRestarts += 1;
        isCorrect = true;
        isCycling = true;
    }

    public void OnNextStageButtonClicked()
    {
        nextStageButton.gameObject.SetActive(false);
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
        formulaPanel.ShowPanel(currentSequence, gameTimer);
    }
}
