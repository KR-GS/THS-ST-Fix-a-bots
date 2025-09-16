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
    public TextMeshProUGUI pageText;
    public Button nextButton;
    public Button backButton;
    public Button closeButton;
    private int currentPage = 0;
    private int maxPages = 12;
    public Image tutorialScene;
    public Sprite TVSpriteNoOrder;
    public Sprite TVSpriteIP;
    public Sprite TVSpriteNO;

    public Sprite WT1;
    public Sprite WT2;
    public Sprite WT3;
    public Sprite WT4;
    public Sprite WT5;
    public Sprite WT6;
    public Sprite WT7;
    public Sprite WT8;
    public Sprite WT9;
    public Sprite WT10;
    public Sprite WT11;
    public Sprite WT12;

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

        if(SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            switch (index)
            {
                case 0:
                    tutorialText.text = "You are interacting with the workshop at the moment!";
                    tutorialScene.sprite = WT1;
                    pageText.text = "Page 1/" + maxPages;
                    backButton.gameObject.SetActive(false);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 1:
                    tutorialText.text = "The coin icon shows how much money you have earned by completing tasks!";
                    tutorialScene.sprite = WT2;
                    pageText.text = "Page 2/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 2:
                    tutorialText.text = "Remaining orders show how many orders you have left before ending the level.";
                    tutorialScene.sprite = WT3;
                    pageText.text = "Page 3/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 3:
                    tutorialText.text = "Active orders show how many orders you have that are in progress.";
                    tutorialScene.sprite = WT4;
                    pageText.text = "Page 4/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 4:
                    tutorialText.text = "This shows which level / day you are currently at right now.";
                    tutorialScene.sprite = WT5;
                    pageText.text = "Page 5/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 5:
                    tutorialText.text = "This shows how many time you have left to complete orders to receive full payment. Completing orders when the timer strikes 0 means you will receive half the amount!";
                    tutorialScene.sprite = WT6;
                    pageText.text = "Page 6/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 6:
                    tutorialText.text = "This is the TV. You get orders here and check its contents.";
                    tutorialScene.sprite = WT7;
                    pageText.text = "Page 7/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 7:
                    tutorialText.text = "The TV screen can change depending on the status of your orders. The screen you see shows a detailed explanation of each TV screen.";
                    tutorialScene.sprite = WT8;
                    pageText.text = "Page 8/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 8:
                    tutorialText.text = "When clicking the TV, you will be directed to the order sheet to check its contents.";
                    tutorialScene.sprite = WT9;
                    pageText.text = "Page 9/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 9:
                    tutorialText.text = "The To-Do List shows what tasks you will be doing to complete the order. As the day progresses, you will encounter new tasks too!";
                    tutorialScene.sprite = WT10;
                    pageText.text = "Page 10/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;
                case 10:
                    tutorialText.text = "After checking out the order sheet, a floating indicator will appear showing which task you will have to do to complete the order.";
                    tutorialScene.sprite = WT11;
                    pageText.text = "Page 11/" + maxPages;
                    nextButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(false);

                    break;;
                case 11:
                    tutorialText.text = "This is the end of the tutorial. Feel free to click on me if you feel lost again!";
                    tutorialScene.sprite = WT12;
                    pageText.text = "Page 12/" + maxPages;
                    nextButton.gameObject.SetActive(false);
                    backButton.gameObject.SetActive(true);
                    closeButton.gameObject.SetActive(true);

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
        GameLoopManager.Instance.ShowTV(false);
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
