using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorkshopTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private GameLoopManager glm;
    [SerializeField] private OrderManager om;
    [SerializeField] private RaycastInteractor ri;
    [SerializeField] private TimerScript ts;
    public TextMeshProUGUI tutorialText;
    public Button nextButton;
    public Button backButton;
    public Button closeButton;
    private int currentPage = 0;
    private int maxPages = 8;
    public Sprite TVSpriteNoOrder;
    public Sprite TVSpriteIP;
    public Sprite TVSpriteNO;
    public Image tutorialPointer;
    public Image TVDemo;
    public Image PATTSprite;
    public Image SpeechBubble;
    public Image toDolistDemo;
    public Image demoIndicator;
    public Image demoStation;
    public Image tutorialLights;

    private bool wasRaycastingEnabled = true;


    private void Awake()
    {
        if (tutorialObject != null)
        {

            tutorialObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Tutorial Object not assigned!");
        }
    }

    private void updateContents(int index)
    {
        DisableRaycasting();

        nextButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        nextButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        tutorialPointer.gameObject.SetActive(false);
        TVDemo.gameObject.SetActive(false);
        toDolistDemo.gameObject.SetActive(false);
        demoIndicator.gameObject.SetActive(false);
        glm.calendar.gameObject.SetActive(false);
        glm.dayNumber.gameObject.SetActive(false);


        if (SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            switch (index)
            {
                case 0:
                    tutorialText.text = "Welcome to your workshop! Let me tour you around!";
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-258, 42);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-586, -16);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(-18, -5);
                    backButton.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);
                    tutorialPointer.gameObject.SetActive(false);
                    demoStation.gameObject.SetActive(true);
                    glm.calendar.gameObject.SetActive(false);
                    glm.dayNumber.gameObject.SetActive(false);
                    break;
                case 1:
                    tutorialText.text = "The calendar here shows how many days since you've first opened right here!";
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-358, 265);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-726, 207);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(-158, 218);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-707, 475);
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    glm.calendar.gameObject.SetActive(true);
                    glm.dayNumber.gameObject.SetActive(true);
                    ri.timeText.gameObject.SetActive(true);
                    break;

                case 2:
                    tutorialText.text = "Now this shows how much time you have left on the day.";
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-358, 265);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-726, 207);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(-158, 218);
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(41, 437);
                    glm.calendar.gameObject.SetActive(true);
                    glm.dayNumber.gameObject.SetActive(true);
                    ri.timeText.gameObject.SetActive(true);
                    TVDemo.gameObject.SetActive(false);

                    break;
                case 3:
                    tutorialText.text = "Tap on the T.V. over here to check for any orders you may have.";
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-209, 8);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-537, -50);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(31, -39);
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    tutorialPointer.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);
                    glm.calendar.gameObject.SetActive(true);
                    glm.dayNumber.gameObject.SetActive(true);
                    ri.timeText.gameObject.SetActive(true);
                    TVDemo.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteNoOrder;
                    break;
                case 4:
                    tutorialText.text = "Check it when it lights up for any new orders from customers!";
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-209, 8);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-537, -50);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(31, -39);
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    glm.calendar.gameObject.SetActive(true);
                    glm.dayNumber.gameObject.SetActive(true);
                    ri.timeText.gameObject.SetActive(true);
                    toDolistDemo.gameObject.SetActive(false);
                    TVDemo.gameObject.SetActive(true);
                    demoStation.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteNO;
                    break;
                case 5:
                    tutorialText.text = "The order form here shows what jobs need to be done. Take note of the pictures on the right.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-317, -55);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-645, -113);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(-77, -102);
                    toDolistDemo.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(607, 116);
                    glm.calendar.gameObject.SetActive(false);
                    glm.dayNumber.gameObject.SetActive(false);
                    ri.timeText.gameObject.SetActive(false);
                    TVDemo.gameObject.SetActive(false);
                    demoStation.gameObject.SetActive(false);
                    demoIndicator.gameObject.SetActive(false);
                    break;
                case 6:
                    tutorialText.text = "They appear on the stations the bots need fixing at!";
                    toDolistDemo.gameObject.SetActive(false);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-317, -266);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-645, -324);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(-77, -313);
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialPointer.gameObject.SetActive(false);
                    TVDemo.gameObject.SetActive(true);
                    demoStation.gameObject.SetActive(true);
                    demoIndicator.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteIP;
                    glm.calendar.gameObject.SetActive(true);
                    glm.dayNumber.gameObject.SetActive(true);
                    ri.timeText.gameObject.SetActive(true);
                    break;

                case 7:
                    tutorialText.text = "Look down! Once all jobs are done, you will get paid based on your speed.";
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-258, 42);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-586, -16);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(-18, -5);
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    glm.calendar.gameObject.SetActive(true);
                    glm.dayNumber.gameObject.SetActive(true);
                    ri.timeText.gameObject.SetActive(true);
                    glm.prizeText.gameObject.SetActive(true);
                    demoIndicator.gameObject.SetActive(false);
                    glm.prizeText.text = "You earned +50 for finishing on time!";
                    tutorialPointer.gameObject.SetActive(false);
                    TVDemo.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteNoOrder;
                    break;

                case 8:
                    tutorialText.text = "That’s the tour! If you feel lost, don’t hesitate to ask me!";
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-258, 42);
                    PATTSprite.rectTransform.anchoredPosition = new Vector2(-586, -16);
                    SpeechBubble.rectTransform.anchoredPosition = new Vector2(-18, -5);
                    nextButton.gameObject.SetActive(false);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(true);
                    tutorialPointer.gameObject.SetActive(false);
                    glm.calendar.gameObject.SetActive(false);
                    glm.dayNumber.gameObject.SetActive(false);
                    glm.prizeText.gameObject.SetActive(false);
                    ri.timeText.gameObject.SetActive(false);
                    //tutorialPointer.rectTransform.anchoredPosition = new Vector2(-186, 131);
                    TVDemo.gameObject.SetActive(true);
                    break;
            }
        }

    }

    private void DisableRaycasting()
    {
        if (RaycastInteractor.Instance != null)
        {
            // Store current state before disabling
            wasRaycastingEnabled = ri.enabled;
            ri.enabled = false;
            Debug.Log("Raycasting disabled for tutorial");
        }
    }

    private void EnableRaycasting()
    {
        if (RaycastInteractor.Instance != null)
        {
            ri.enabled = true;
            Debug.Log("Raycasting re-enabled after tutorial");
        }
    }

    public void NextPage()
    {
        ++currentPage;
        updateContents(currentPage);
    }

    public void PreviousPage()
    {
        --currentPage;
        updateContents(currentPage);
    }

    public void ResetTutorial()
    {
        currentPage = 0;
        updateContents(currentPage);

        tutorialLights.gameObject.SetActive(true);

        glm.ShowTV(false);
        
        glm.pauseButton.gameObject.SetActive(false);

        glm.dayNumber.gameObject.SetActive(false);

        if (ri.ToolIndicator != null) ri.ToolIndicator.gameObject.SetActive(false);
        if (ri.WireIndicator != null) ri.WireIndicator.gameObject.SetActive(false);
        if (ri.PaintIndicator != null) ri.PaintIndicator.gameObject.SetActive(false);

        ri.readyIndicator.gameObject.SetActive(false);
        ri.readyText.gameObject.SetActive(false);

        //ri.tutorialIndicator.gameObject.SetActive(false);
        //ri.pointTutorial.gameObject.SetActive(false);

        if (ri.timeText != null)
        {
            ri.timeText.gameObject.SetActive(false); // Hide the time text
        }

        ts.StopTimer();
        //TimerScript.instance.StopTimer();

        tutorialObject.SetActive(true);

        Debug.Log("Tutorial restarted from the beginning.");
    }

    public void HideTutorial()
    {
        tutorialObject.SetActive(false);
        Debug.Log("Tutorial has been finished, hiding: " + tutorialObject.name);

        tutorialLights.gameObject.SetActive(false);

        EnableRaycasting();

        glm.pauseButton.gameObject.SetActive(true);

        if (StaticData.isFirstWS == true)
        {
            StaticData.isFirstWS = false;
        }

        if (SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            glm.ShowWorkshopElements();

            Order savedOrder = om.GetActiveOrder();

            if (ri.timeText != null)
            {
                ri.timeText.gameObject.SetActive(true); // Hide the time text
            }

            if (StaticData.startOfDay == true)
            {
                Debug.Log("Aiya, debugging is sad!");
                ri.readyIndicator.gameObject.SetActive(true);
                ri.readyText.gameObject.SetActive(true);
            }
            else if (StaticData.startOfDay == false)
            {
                Debug.Log("Ayo, will this work?");
                ri.readyIndicator.gameObject.SetActive(false);
                ri.readyText.gameObject.SetActive(false);
                ts.StartTimer();
            }

            ri.isOrderChecked = StaticData.isOrderChecked;
            Debug.Log("isOrderChecked status: " + ri.isOrderChecked);

            if (!ri.isOrderChecked && savedOrder != null)
            {
                Debug.Log("Setting indicators active...");

                if (savedOrder.needsTool)
                {
                    ri.ToolIndicator.gameObject.SetActive(true);
                    Debug.Log("ToolIndicator enabled");
                }
                if (savedOrder.needsWire)
                {
                    ri.WireIndicator.gameObject.SetActive(true);
                    Debug.Log("WireIndicator enabled");
                }
                if (savedOrder.needsPaint)
                {
                    ri.PaintIndicator.gameObject.SetActive(true);
                    Debug.Log("PaintIndicator enabled");
                }

                ri.isOrderChecked = true;
                StaticData.isOrderChecked = true;
            }
            else if (ri.isOrderChecked && savedOrder != null)
            {
                Debug.Log("Order is checked, returning old indicators...");

                if (savedOrder.needsTool && !StaticData.isToolDone)
                {
                    ri.ToolIndicator.gameObject.SetActive(true);
                }
                if (savedOrder.needsPaint && !StaticData.isPaintDone)
                {
                    ri.PaintIndicator.gameObject.SetActive(true);
                }
                if (savedOrder.needsWire && !StaticData.isWireDone)
                {
                    ri.WireIndicator.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log($"Indicators not set. isOrderChecked={ri.isOrderChecked}, currentOrder={savedOrder}");
            }
        }


    }
}
