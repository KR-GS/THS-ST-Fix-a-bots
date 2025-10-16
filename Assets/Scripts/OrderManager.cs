using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour, IDataPersistence
{
    public static OrderManager Instance;
    [SerializeField] private GameObject orderCompletePanel;
    [SerializeField] private RaycastInteractor raycastInteractor;
    private Button button;
    public Button nextdayButton;
    public TextMeshProUGUI completeText;
    private int prize;
    private bool isFinished = false;
    private bool sendNewOrder = false;

    public List<Order> orderList = new List<Order>();
    public List<Order> activeOrders = new List<Order>();
    public Queue<Order> pendingOrders = new Queue<Order>();
    private Coroutine deliveryRoutine;
    public Order currentOrder;
    public bool orderReceived;
    public SpriteRenderer TVSprite;
    public Sprite TVSpriteNoOrder;
    public Sprite TVSpriteIP;
    public Sprite TVSpriteNO;
    private Coroutine scheduleRoutine;

    public int currentOrderIndex = -1;
    private bool wasRaycastingEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (orderCompletePanel != null)
            HideOrderCompletePanel();
        else
            Debug.LogWarning("Order Complete Panel not assigned yet!");

        //DataPersistenceManager.Instance.LoadGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LO_WS2D")
        {
            Debug.Log("Returned to WorkshopScene. Checking for completed orders...");
            StartCoroutine(HandleWorkshopSceneLoad());
        }

    }

    private System.Collections.IEnumerator HandleWorkshopSceneLoad()
    {
        yield return null;

        if (orderCompletePanel != null)
        {
            HideOrderCompletePanel();
        }
        else
        {
            Debug.LogWarning("Order Complete Panel not assigned yet!");
        }
        

        if (isFinished)
        {
            Debug.Log("isFinished was true. Showing complete panel...");
            ShowOrderCompletePanel();
        }
        else
        {
            TryCompleteOrder();
        }

    }
    

    public Order CreateNewOrder()
    {

        int level = GameLoopManager.Instance.level; // adjust if you track level differently
        Order newOrder = new Order();

        string[] customerList = new string[]
        {
            "Mr. Burr", "JMC", "Juanton", "Vens", "Bev",
            "Gabe", "KC", "Manny", "Jan", "MacDonald"
        };

        string[] robotList = new string[]
        {
            "Theo", "MerryBot", "Toyo", "EthicAI", "TutorBot",
            "M.O.N.O", "PHPper", "FightBot", "TennisPro", "The Planter"
        };

        int namePicker = Random.Range(0, customerList.Length);

        newOrder.customername = customerList[namePicker];
        newOrder.robotname = robotList[namePicker];



        if (level >= 1 && level < 6)
        {
            newOrder.needsTool = true;
            newOrder.needsPaint = false; //originally false, gonna QA
            newOrder.needsWire = false;
        }

        else if (level >= 6 && level < 11)
        {
            float rand = Random.value;
            newOrder.needsTool = Random.value < 0.5f;
            newOrder.needsPaint = Random.value < 0.5f;
            newOrder.needsWire = false;
        }

        
        else if (level >= 11)
        {
            float rand = Random.value;
            newOrder.needsTool = Random.value < 0.5f;
            newOrder.needsPaint = Random.value < 0.5f;
            newOrder.needsWire = false; //Set to 0.5 if wire is finalized
        }
        
        // Ensure at least one requirement
        


        orderList.Add(newOrder);
        currentOrderIndex = 0;

        Debug.Log("New Order Created!");

        if (TimerScript.instance != null)
        {
            TimerScript.instance.StartTimer();
        }

        return newOrder;
    }

    public void AddToActiveOrders(Order order)
    {
        activeOrders.Add(order);
    }


    public void StartOrderBatch()
    {
            if (!orderReceived)
            {
                pendingOrders.Clear();
                orderReceived = true;
                for (int i = 0; i < 5; i++)
                {
                    Order o = CreateNewOrder();
                    pendingOrders.Enqueue(o);
                }

                if (pendingOrders.Count > 0)
                {
                    var firstOrder = pendingOrders.Dequeue();
                    AddToActiveOrders(firstOrder);
                    currentOrder = firstOrder;
                    Debug.Log("First order delivered!");
                    TVSprite.sprite = TVSpriteNO;
                    StaticData.TVScreen = 1;

                // lock sending more until this order is completed
                sendNewOrder = false;
                    StaticData.sendNewOrder = false;
                }

                // start coroutine to handle the rest
                Instance.StartCoroutine(ScheduleNextOrder());
            }
            else if (orderReceived)
            {
            Debug.Log("Orders have already been received for this level.");
        }
    }

    private IEnumerator ScheduleNextOrder()
    {
        while (pendingOrders.Count > 0)
        {
            yield return new WaitUntil(() => sendNewOrder);
            yield return new WaitForSeconds(5f);
            var nextOrder = pendingOrders.Dequeue();
            AddToActiveOrders(nextOrder); 
            Debug.Log("Delivered order!");
            Debug.Log("isChecked status: " + StaticData.isOrderChecked);
            TVSprite.sprite = TVSpriteNO;
            StaticData.TVScreen = 1;

            sendNewOrder = false;
            StaticData.sendNewOrder = false;
            Debug.Log("Order will be sent after you complete this task!"); 
            Debug.Log("SendNewOrder status: " + sendNewOrder);
        }
    }
    public void TryCompleteOrder()
    {
        if (orderList == null || orderList.Count == 0)
            return;

 
        if (orderList[0].IsComplete())
        {
            Debug.Log("Order Complete!");
            RaycastInteractor.Instance.TVSprite.sprite = TVSpriteNoOrder;
            StaticData.TVScreen = 0;
            orderList.RemoveAt(0);
            activeOrders.RemoveAt(0); 
            StaticData.isToolDone = false;
            StaticData.isPaintDone = false;
            StaticData.isWireDone = false;
            StaticData.isOrderChecked = false;
            StaticData.incorrectIndices.Clear();
            StaticData.incorrectValues.Clear();
            StaticData.missingVals = 0;
            GameLoopManager.Instance.GenerateAndStorePattern();


            if (TimerScript.instance != null)
            {
                if(TimerScript.instance.timeLft > 0)
                {
                    Debug.Log("Order completed on time! You receive full amount as payment!");
                    GameLoopManager.Instance.money += 50; //Base value 50
                    GameLoopManager.Instance.UpdateMoneyText();

                }
                else
                {
                    Debug.Log("Order completed late! You receive half amount as payment!");
                    GameLoopManager.Instance.money += 25; //Base value 50
                    GameLoopManager.Instance.UpdateMoneyText();
                }
            }

            if (RaycastInteractor.Instance != null)
            {
                sendNewOrder = true;
                StaticData.sendNewOrder = true;
                Debug.Log("sendNewOrder is now set to true!:" + sendNewOrder);
                Debug.Log("StaticData.sendNewOrder is now set to true!:" + StaticData.sendNewOrder);
            }
        }

        GameLoopManager.Instance.UpdateRemainingOrders();

        if (orderList.Count == 0)
        {
            isFinished = true;
            Debug.Log("All Orders Complete!");
            TimerScript.instance.StopTimer();
            ShowOrderCompletePanel();
            raycastInteractor.enabled = false;

        }


    }

    public Order GetNextPendingOrder()
    {
        foreach (var order in pendingOrders)
        {
            if (!order.IsComplete())
            {
                return order;
            }
        }
        return null;
    }

    public Order GetCurrentOrder()
    {
        if (currentOrderIndex >= 0 && currentOrderIndex < orderList.Count)
            return orderList[currentOrderIndex];

        return null;
    }

    public Order GetActiveOrder()
    {
        if(activeOrders.Count > 0)
            return activeOrders[0];

        return null;
    }
    /*
    public void ShowOrderCompletePanel()
    {
        orderCompletePanel.SetActive(true);
    }

    public void HideOrderCompletePanel()
    {
        orderCompletePanel.SetActive(false);
    }

    public void OnButtonClick()
    {
        HideOrderCompletePanel();
        GameLoopManager.Instance.CompleteLevel();
        //raycastInteractor.enabled = true;
    }
    */

    public void ShowOrderCompletePanel()
    {
        if (orderCompletePanel == null)
        {
            Debug.LogError("[OrderManager] Order Complete Panel is not assigned!");
            return;
        }

        DisableRaycasting();

        orderCompletePanel.SetActive(true);
        Debug.Log("[OrderManager] Showing panel: " + orderCompletePanel.name);

        //completeText.gameObject.SetActive(true);
        GameLoopManager.Instance.moneyImage.gameObject.SetActive(false);
        GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
        GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
        GameLoopManager.Instance.tutorialButton.gameObject.SetActive(false);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);
        GameLoopManager.Instance.shopButton.gameObject.SetActive(false);

        if (nextdayButton != null)
        {
            nextdayButton.gameObject.SetActive(true);
            nextdayButton.onClick.RemoveAllListeners();
            nextdayButton.onClick.AddListener(() =>
            {
                HideOrderCompletePanel();
                GameLoopManager.Instance.CompleteLevel();
            });
        }
    }

    public void HideOrderCompletePanel()
    {
        if (orderCompletePanel == null)
        {
            Debug.LogWarning("[OrderManager] Order Complete Panel not assigned — nothing to hide.");
            return;
        }

        EnableRaycasting();

        orderCompletePanel.SetActive(false);
        Debug.Log("[OrderManager] Hiding panel: " + orderCompletePanel.name);
        Debug.Log("[LOOK HERE] Pending orders count: " + pendingOrders.Count);

        //completeText.gameObject.SetActive(false);
        GameLoopManager.Instance.moneyImage.gameObject.SetActive(true);
        GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
        GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
        GameLoopManager.Instance.tutorialButton.gameObject.SetActive(true);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
        GameLoopManager.Instance.shopButton.gameObject.SetActive(true);


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
            RaycastInteractor.Instance.enabled = wasRaycastingEnabled;
            Debug.Log("Raycasting re-enabled after tutorial");
        }
    }


    public void LoadData(GameData data)
    {
        this.orderReceived = data.orderReceived;
        this.orderList = data.savedOrders ?? new List<Order>();
        this.activeOrders = data.savedActiveOrders ?? new List<Order>();
        this.currentOrderIndex = data.currentOrderIndex;
        this.isFinished = data.finished;
        this.prize = data.prize;
        StaticData.isPaintDone = data.isPaintDone;
        StaticData.isToolDone = data.isToolDone;
        StaticData.isWireDone = data.isWireDone;
        StaticData.isOrderChecked = data.isOrderChecked;
        StaticData.sendNewOrder = data.sendNewOrder;

     
        if (TimerScript.instance != null && GetCurrentOrder() != null)
        {
            TimerScript.instance.StartTimer();
            Debug.Log("Timer started from OrderManager.LoadData");
        }

        if (data.pendingOrdersList != null)
        {
            this.pendingOrders = new Queue<Order>(data.pendingOrdersList);
        }
        else
        {
            Debug.Log("There are no saved pending orders. Initializing empty queue.");
            this.pendingOrders = new Queue<Order>();
        }
     


        if (orderReceived && pendingOrders.Count > 0)
        {
            StartCoroutine(ScheduleNextOrder());
        }
        
    }

    public void SaveData(ref GameData data)
    {
        data.savedOrders = this.orderList;
        data.savedActiveOrders = this.activeOrders;
        data.pendingOrdersList = new List<Order>(this.pendingOrders);
        data.currentOrderIndex = this.currentOrderIndex;
        data.orderReceived = this.orderReceived;
        data.finished = this.isFinished;
        data.prize = this.prize;
        data.isPaintDone = StaticData.isPaintDone;
        data.isToolDone = StaticData.isToolDone;
        data.isWireDone = StaticData.isWireDone;
        data.isOrderChecked = StaticData.isOrderChecked;
        data.sendNewOrder = StaticData.sendNewOrder;
    }

    public int GetPrize()
    {
        return prize;
    }

    public bool GetStatus()
    {
        return isFinished;
    }

    public void SetStatus(bool s)
    {
        isFinished = s;
    }
}