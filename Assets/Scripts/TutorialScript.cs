using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialScript : MonoBehaviour
{
    public static TutorialScript Instance;
    [SerializeField] private GameObject tutorialObject;
    public TextMeshProUGUI tutorialText;
    public Button nextButton;
    public Button backButton;
    public Button closeButton;
    private int currentPage = 0;
    private int maxPages = 13;
    public Sprite TVSpriteNoOrder;
    public Sprite TVSpriteIP;
    public Sprite TVSpriteNO;
    public Image tutorialBot;
    public Image tutorialPointer;
    public Image tutorialSpeechBubble;
    public Image TVDemo;
    public Image toDolistDemo;
    public Image demoStation;
    public Image demoIndicator;

    private bool wasRaycastingEnabled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (tutorialObject != null)
        {
            if (StaticData.newGame)
                tutorialObject.SetActive(true);
            else
                tutorialObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Tutorial Object not assigned!");
            Destroy(gameObject);
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

        if (SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            switch (index)
            {
                case 0:
                    tutorialText.text = "You are interacting with the workshop at the moment!";
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(18, 6);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-435, 151);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-655, 271);
                    backButton.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);
                    tutorialPointer.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyImage.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
                    demoStation.gameObject.SetActive(true);
                    demoStation.rectTransform.anchoredPosition = new Vector2(104, 55);
                    break;
                case 1:
                    tutorialText.text = "The coin icon shows how much money you have earned by completing tasks!";
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(301, 261);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-134, 380);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-359, 501);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-962, 388);
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyImage.gameObject.SetActive(true);
                    GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
                    GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
                    break;

                case 2:
                    tutorialText.text = "Remaining orders show how many orders you have left before ending the level.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(301, 261);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-134, 380);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-359, 501);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-962, 304);
                    GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
                    GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);

                    break;
                case 3:
                    tutorialText.text = "Active orders show how many orders you have that are in progress.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(140, 6);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-309, 151);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-521, 271);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-962, 210);
                    GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
                    GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
                    break;
                case 4:
                    tutorialText.text = "This shows which level / day you are currently at right now.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(140, 6);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-309, 151);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-521, 271);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-274, 416);
                    RaycastInteractor.Instance.timeText.gameObject.SetActive(false);
                    GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
                    break;
                case 5:
                    tutorialText.text = "This shows how much time left to receive full payment when finishing orders. Working late will half the amount received!";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(140, -88);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-309, 55);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-521, 171);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-157, 334);
                    RaycastInteractor.Instance.timeText.gameObject.SetActive(true);
                    TVDemo.gameObject.SetActive(false);
                    break;
                case 6:
                    tutorialText.text = "This is the TV. You get orders here and check its contents.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(424, -297);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-5, -142);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-225, -27);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-186, 131);
                    TVDemo.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteNoOrder;


                    break;

                case 7:
                    tutorialText.text = "The TV screen you are seeing right now is blank. It shows that there is no order at the moment...";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(424, -297);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-5, -142);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-225, -27);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-186, 131);
                    TVDemo.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteNoOrder;
                    break;

                case 8:
                    tutorialText.text = "This TV screen shows you have a new order! A new order will be set if there are no active orders.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(424, -297);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-5, -142);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-225, -27);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-186, 131);
                    TVDemo.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteNO;
                    break;
                case 9:
                    tutorialText.text = "This TV screen shows you have an order in progress. You can check your current orders here.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(424, -297);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-5, -142);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-225, -27);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-186, 131);
                    TVDemo.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteIP;
                    break;

                case 10:
                    tutorialText.text = "When clicking the TV, you will be directed to the order sheet to check its contents.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyImage.gameObject.SetActive(true);
                    GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
                    GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
                    GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
                    GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
                    RaycastInteractor.Instance.timeText.gameObject.SetActive(true);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(424, -297);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-5, -142);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-225, -27);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-205, 222);
                    tutorialPointer.transform.rotation = Quaternion.Euler(0, 0, 180);
                    TVDemo.gameObject.SetActive(true);
                    toDolistDemo.gameObject.SetActive(false);
                    demoStation.gameObject.SetActive(true);
                    TVDemo.sprite = TVSpriteIP;
                    break;
                case 11:
                    tutorialText.text = "The To-Do List shows what tasks you will be doing to complete the order. As the day progresses, you will encounter new tasks too!";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyImage.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
                    GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
                    GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);
                    GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
                    RaycastInteractor.Instance.timeText.gameObject.SetActive(false);
                    TVDemo.gameObject.SetActive(false);
                    demoStation.gameObject.SetActive(false);
                    toDolistDemo.gameObject.SetActive(true);
                    demoIndicator.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(424, -134);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-5, 25);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-225, 143);
                    tutorialPointer.transform.rotation = Quaternion.Euler(0, 0, 90);
                    tutorialPointer.gameObject.SetActive(true);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(-186, -335);

                    break;
                case 12:
                    tutorialText.text = "After checking out the order sheet, a floating indicator will appear showing which task you will have to do to complete the order.";
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyImage.gameObject.SetActive(true);
                    GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
                    GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
                    GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
                    GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
                    RaycastInteractor.Instance.timeText.gameObject.SetActive(true);
                    TVDemo.gameObject.SetActive(true);
                    toDolistDemo.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(438, -423);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-5, -258);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-225, -136);
                    tutorialPointer.transform.rotation = Quaternion.Euler(0, 0, 180);
                    tutorialPointer.gameObject.SetActive(false);
                    tutorialPointer.rectTransform.anchoredPosition = new Vector2(89, -121);
                    demoStation.gameObject.SetActive(true);
                    demoIndicator.gameObject.SetActive(true);
                    demoIndicator.rectTransform.anchoredPosition = new Vector2(98, 92);

                    break;;
                case 13:
                    tutorialText.text = "This is the end of the tutorial. Feel free to click on me if you feel lost again!";
                    nextButton.gameObject.SetActive(false);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(true);
                    GameLoopManager.Instance.moneyImage.gameObject.SetActive(false);
                    GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
                    GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
                    GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);
                    GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
                    RaycastInteractor.Instance.timeText.gameObject.SetActive(false);
                    demoStation.gameObject.SetActive(true);
                    TVDemo.gameObject.SetActive(false);
                    toDolistDemo.gameObject.SetActive(false);
                    tutorialPointer.gameObject.SetActive(false);
                    demoIndicator.gameObject.SetActive(false);
                    tutorialBot.rectTransform.anchoredPosition = new Vector2(18, 6);
                    tutorialSpeechBubble.rectTransform.anchoredPosition = new Vector2(-435, 151);
                    tutorialText.rectTransform.anchoredPosition = new Vector2(-655, 271);
                    break;
            }
        }

    }

    private void DisableRaycasting()
    {
        if (RaycastInteractor.Instance != null)
        {
            // Store current state before disabling
            wasRaycastingEnabled = RaycastInteractor.Instance.enabled;
            RaycastInteractor.Instance.enabled = false;
            Debug.Log("Raycasting disabled for tutorial");
        }
    }

    private void EnableRaycasting()
    {
        if (RaycastInteractor.Instance != null)
        {
            RaycastInteractor.Instance.enabled = true;
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

        GameLoopManager.Instance.ShowTV(false);

        GameLoopManager.Instance.moneyImage.gameObject.SetActive(false);
        GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
        GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
        GameLoopManager.Instance.tutorialButton.gameObject.SetActive(false);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);

        if (RaycastInteractor.Instance.ToolIndicator != null) RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(false);
        if (RaycastInteractor.Instance.WireIndicator != null) RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(false);
        if (RaycastInteractor.Instance.PaintIndicator != null) RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(false);

        RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(false);
        RaycastInteractor.Instance.readyText.gameObject.SetActive(false);

        RaycastInteractor.Instance.tutorialIndicator.gameObject.SetActive(false);
        RaycastInteractor.Instance.pointTutorial.gameObject.SetActive(false);

        if (RaycastInteractor.Instance.timeText != null)
        {
            RaycastInteractor.Instance.timeText.gameObject.SetActive(false); // Hide the time text
        }

        TimerScript.instance.StopTimer();

        tutorialObject.SetActive(true);

        Debug.Log("Tutorial restarted from the beginning.");
    }

    public void HideTutorial()
    {
        tutorialObject.SetActive(false);
        Debug.Log("Tutorial has been finished, hiding: " + tutorialObject.name);

        EnableRaycasting();

        if (StaticData.newGame)
        {
            StaticData.newGame = false;
        }

        //completeText.gameObject.SetActive(false);
        GameLoopManager.Instance.moneyImage.gameObject.SetActive(true);
        GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
        GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
        GameLoopManager.Instance.tutorialButton.gameObject.SetActive(true);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
        GameLoopManager.Instance.ShowTV(true);
        RaycastInteractor.Instance.TVSprite.gameObject.SetActive(true);

        if (StaticData.TVScreen == 0)
        {
            RaycastInteractor.Instance.TVSprite.sprite = TVSpriteNoOrder;
        }
        else if(StaticData.TVScreen == 1)
        {
            RaycastInteractor.Instance.TVSprite.sprite = TVSpriteNO;
        }
        else if (StaticData.TVScreen == 2)
        {
            RaycastInteractor.Instance.TVSprite.sprite = TVSpriteIP;
        }

        Order savedOrder = OrderManager.Instance.GetActiveOrder();

        if (StaticData.startOfDay == true)
        {
            Debug.Log("Aiya, debugging is sad!");
            RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(true);
            RaycastInteractor.Instance.readyText.gameObject.SetActive(true);
        }
        else if (StaticData.startOfDay == false)
        {
            Debug.Log("Ayo, will this work?");
            RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(false);
            RaycastInteractor.Instance.readyText.gameObject.SetActive(false);
        }


        if (RaycastInteractor.Instance.timeText != null)
        {
            RaycastInteractor.Instance.timeText.gameObject.SetActive(true); // Hide the time text
        }

        TimerScript.instance.StartTimer();

        RaycastInteractor.Instance.isOrderChecked = StaticData.isOrderChecked;
        Debug.Log("isOrderChecked status: " + RaycastInteractor.Instance.isOrderChecked);

        if (!RaycastInteractor.Instance.isOrderChecked && savedOrder != null)
        {

            Debug.Log("Setting indicators active...");

            if (savedOrder.needsTool)
            {
                RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(true);
                Debug.Log("ToolIndicator enabled");
            }
            if (savedOrder.needsWire)
            {
                RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(true);
                Debug.Log("WireIndicator enabled");
            }
            if (savedOrder.needsPaint)
            {
                RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(true);
                Debug.Log("PaintIndicator enabled");
            }

            RaycastInteractor.Instance.isOrderChecked = true;
            StaticData.isOrderChecked = true;
        }
        else if (RaycastInteractor.Instance.isOrderChecked && savedOrder != null)
        {
            Debug.Log("Order is checked, returning old indicators...");

            if (savedOrder.needsTool && !StaticData.isToolDone)
            {
                RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(true);
            }
            if (savedOrder.needsPaint && !StaticData.isPaintDone)
            {
                RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(true);
            }
            if (savedOrder.needsWire && !StaticData.isWireDone)
            {
                RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"Indicators not set. isOrderChecked={RaycastInteractor.Instance.isOrderChecked}, currentOrder={savedOrder}");
        }

    }


}
