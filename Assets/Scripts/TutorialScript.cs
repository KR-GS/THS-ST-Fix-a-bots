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
    private int maxPages = 3;
    private Image tutorialScene;
    public Sprite TVSpriteNoOrder;
    public Sprite TVSpriteIP;
    public Sprite TVSpriteNO;

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

        switch (index)
        {
            case 0:
                tutorialText.text = "Hello";
                pageText.text = "Page 1/" + maxPages;
                backButton.gameObject.SetActive(false);
                closeButton.gameObject.SetActive(false);

                break;
            case 1:
                tutorialText.text = "This is the next page. Feel free to look at me!";
                pageText.text = "Page 2/" + maxPages;
                nextButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                closeButton.gameObject.SetActive(false);
         
                break;
            case 2:
                tutorialText.text = "This is the end of the tutorial. Press the question mark button if you feel lost again!";
                pageText.text = "Page 3/" + maxPages;
                nextButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);
                closeButton.gameObject.SetActive(true);
       
                break;
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
