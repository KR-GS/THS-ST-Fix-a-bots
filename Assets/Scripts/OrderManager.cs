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
    //public static OrderManager Instance;
    [SerializeField] public GameObject orderCompletePanel;
    [SerializeField] private RaycastInteractor ri;
    [SerializeField] private GameLoopManager glm;
    [SerializeField] private TimerScript ts;
    private Button button;
    public Button nextdayButton;
    public TextMeshProUGUI completeText;
    private int prize;
    public bool isFinished = false;
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

    private bool isScheduleRunning = false;

    public int currentOrderIndex = -1;

  
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LO_WS2D")
        {
            Debug.Log("Returned to WorkshopScene. Checking for completed orders...");
            Debug.Log("Hey, I answered the call!");
            StartCoroutine(HandleWorkshopSceneLoad());
        }

    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.SaveGame();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
   
    private System.Collections.IEnumerator HandleWorkshopSceneLoad()
    {
        Debug.Log("Now we wait!");
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
            Debug.Log("I am going to complete your order!!!");
            TryCompleteOrder();
        }

    }
   

    public Order CreateNewOrder()
    {

        int level = glm.level; // adjust if you track level differently
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


        if (level >= 1)
        {
            float rand = Random.value;
            newOrder.needsTool = Random.value < 0.5f;
            newOrder.needsPaint = Random.value < 0.5f;
            newOrder.needsWire = Random.value < 0.5f;
        }

        /*
        if (level >= 1 && level < 6)
        {
            newOrder.needsTool = true; //originally true, gonna QA
            newOrder.needsPaint = false; //originally false, gonna QA
            newOrder.needsWire = false; //originally false, gonna QA
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
            newOrder.needsWire = Random.value < 0.5f;
        }
        */

        // Ensure at least one requirement
        if (!newOrder.needsTool && !newOrder.needsPaint && !newOrder.needsWire)
        {
            Debug.Log("Are we seriously doing this???");
            newOrder.needsTool = true;
        }


        orderList.Add(newOrder);
        currentOrderIndex = 0;

        Debug.Log("New Order Created!");

        if (ts != null)
        {
            ts.StartTimer();
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
                    StaticData.currentOrder = firstOrder;
                    Debug.Log("First order delivered!");
                    TVSprite.sprite = TVSpriteNO;

                    // lock sending more until this order is completed
                    sendNewOrder = false;
                    StaticData.sendNewOrder = false;
                }

                // start coroutine to handle the rest
                StartCoroutine(ScheduleNextOrder());
            }
            else if (orderReceived)
            {
            Debug.Log("Orders have already been received for this level.");
        }
    }

    private IEnumerator ScheduleNextOrder()
    {
        if (isScheduleRunning)
        {
            Debug.LogWarning("ScheduleNextOrder already running! Skipping duplicate start.");
            yield break;
        }

        isScheduleRunning = true;
        Debug.Log("ScheduleNextOrder started.");

        while (pendingOrders.Count > 0)
        {
            yield return new WaitUntil(() => sendNewOrder);
            yield return new WaitForSeconds(5f);
            var nextOrder = pendingOrders.Dequeue();
            AddToActiveOrders(nextOrder);
            currentOrder = nextOrder;
            StaticData.currentOrder = nextOrder;
            Debug.Log("Delivered order!");
            Debug.Log("isChecked status: " + StaticData.isOrderChecked);
            TVSprite.sprite = TVSpriteNO;

            sendNewOrder = false;
            StaticData.sendNewOrder = false;
            Debug.Log("Order will be sent after you complete this task!"); 
            Debug.Log("SendNewOrder status: " + sendNewOrder);
        }

        isScheduleRunning = false;
        Debug.Log("ScheduleNextOrder finished.");
    }
    public void TryCompleteOrder()
    {

        if (orderList == null || orderList.Count == 0)
        {
            Debug.Log("How the heck did you get here???");
            return; 
        }

        if (orderList[0].IsComplete() == false)
        {
            Debug.Log("You still have not finished your task!");

            Debug.Log("StaticData tool value" + StaticData.isToolDone);
            Debug.Log("StaticData paint value" + StaticData.isPaintDone);
            Debug.Log("StaticData wire value" + StaticData.isWireDone);

            if (StaticData.isToolDone == true)
            {
                Debug.Log("Yeah... you did complete tool!");
                orderList[0].toolDone = true;
                ri.ToolIndicator.gameObject.SetActive(false);
                if (StaticData.toolWrong == 0)
                {
                    Debug.Log("All tools used correctly! Earn 10 points!");
                    glm.toolScore += 10;
                }
                else if (StaticData.toolWrong > 0 && StaticData.toolWrong < 3)
                {
                    Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                    glm.toolScore += 5;
                }
                else if (StaticData.toolWrong >= 3)
                {
                    Debug.Log("You performed poorly! Earn 1 point!");
                    glm.toolScore += 1;
                }

                StaticData.toolWrong = 0;
                Debug.LogWarning("Tool wrongs set to 0!");
            }

            if (StaticData.isPaintDone == true)
            {
                Debug.Log("Magnificent... you did complete sticker!");
                orderList[0].paintDone = true;
                ri.PaintIndicator.gameObject.SetActive(false);
                if (StaticData.paintWrong == 0)
                {
                    Debug.Log("All tools used correctly! Earn 10 points!");
                    glm.paintScore += 10;
                }
                else if (StaticData.paintWrong > 0 && StaticData.paintWrong < 3)
                {
                    Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                    glm.paintScore += 5;
                }
                else if (StaticData.paintWrong >= 3)
                {
                    Debug.Log("You performed poorly! Earn 1 points!");
                    glm.paintScore += 1;
                }

                StaticData.paintWrong = 0;
                Debug.LogWarning("Paint wrongs set to 0!");
            }

            if (StaticData.isWireDone == true)
            {
                Debug.Log("Shocking... you did complete wire!");
                orderList[0].wireDone = true;
                ri.WireIndicator.gameObject.SetActive(false);
                if (StaticData.wireWrong == 0)
                {
                    Debug.Log("All tools used correctly! Earn 10 points!");
                    glm.wireScore += 10;
                }
                else if (StaticData.wireWrong > 0 && StaticData.wireWrong < 3)
                {
                    Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                    glm.wireScore += 5;
                }
                else if (StaticData.wireWrong >= 3)
                {
                    Debug.Log("You performed poorly! Earn 1 points!");
                    glm.wireScore += 1;
                }

                StaticData.wireWrong = 0;
                Debug.LogWarning("Wire wrongs set to 0!");
            }
        }

 
        if (orderList[0].IsComplete())
        {
            Debug.Log("Order Complete!");
            ri.TVSprite.sprite = TVSpriteNoOrder;
            orderList.RemoveAt(0);
            activeOrders.RemoveAt(0);
            StaticData.orderList = orderList;
            StaticData.activeOrders = activeOrders;
            StaticData.pendingOrders = pendingOrders;
            StaticData.isToolDone = false;
            StaticData.isPaintDone = false;
            StaticData.isWireDone = false;
            StaticData.isOrderChecked = false;
            StaticData.incorrectIndices.Clear();
            StaticData.incorrectValues.Clear();
            StaticData.missingVals = 0;
            glm.GenerateAndStorePattern();

            
            if (ts != null)
            {
                if(ts.timeLft > 0)
                {
                    Debug.Log("Order completed on time! You receive full amount as payment!");
                    glm.money += 50; //Base value 50
                    glm.UpdateMoneyText();

                }
                else
                {
                    Debug.Log("Order completed late! You receive half amount as payment!");
                    glm.money += 25; //Base value 50
                    glm.UpdateMoneyText();
                }
            }

            if (ri != null && pendingOrders.Count > 0)
            {
                sendNewOrder = true;
                StaticData.sendNewOrder = true;
                Debug.Log("sendNewOrder is now set to true!:" + sendNewOrder);
                Debug.Log("StaticData.sendNewOrder is now set to true!:" + StaticData.sendNewOrder);
            }

        }

        if (orderList.Count == 0)
        {
            isFinished = true;
            Debug.Log("All Orders Complete!");
            ts.StopTimer();
            ShowOrderCompletePanel();
            ri.enabled = false;

        }

        DataPersistenceManager.Instance.SaveGame();

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

    public void ShowOrderCompletePanel()
    {
        if (orderCompletePanel == null)
        {
            Debug.LogError("[OrderManager] Order Complete Panel is not assigned!");
            return;
        }

        orderCompletePanel.SetActive(true);
        Debug.Log("[OrderManager] Showing panel: " + orderCompletePanel.name);

        //completeText.gameObject.SetActive(true);
        glm.moneyImage.gameObject.SetActive(false);
        glm.dayNumber.gameObject.SetActive(false);
        glm.moneyText.gameObject.SetActive(false);
        glm.onboardImage.gameObject.SetActive(false);
        glm.ordersOnboard.gameObject.SetActive(false);

        if (nextdayButton != null)
        {
            nextdayButton.gameObject.SetActive(true);
            nextdayButton.onClick.RemoveAllListeners();
            nextdayButton.onClick.AddListener(() =>
            {
                HideOrderCompletePanel();
                glm.CompleteLevel();
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


        orderCompletePanel.SetActive(false);
        Debug.Log("[OrderManager] Hiding panel: " + orderCompletePanel.name);
        Debug.Log("[LOOK HERE] Pending orders count: " + pendingOrders.Count);

        //completeText.gameObject.SetActive(false);
        glm.moneyImage.gameObject.SetActive(true);
        glm.dayNumber.gameObject.SetActive(true);
        glm.moneyText.gameObject.SetActive(true);
        glm.onboardImage.gameObject.SetActive(true);
        glm.ordersOnboard.gameObject.SetActive(true);

        DataPersistenceManager.Instance.SaveGame();
    }


    public void LoadData(GameData data)
    {
        this.orderReceived = data.orderReceived;
        this.orderList = data.savedOrders ?? new List<Order>();
        //StaticData.orderList = this.orderList;
        this.activeOrders = data.savedActiveOrders ?? new List<Order>();
        //StaticData.activeOrders = this.activeOrders;
        this.currentOrderIndex = data.currentOrderIndex;
        this.isFinished = data.finished;
        this.prize = data.prize;
        StaticData.isPaintDone = data.isPaintDone;
        StaticData.isToolDone = data.isToolDone;
        StaticData.isWireDone = data.isWireDone;
        StaticData.isOrderChecked = data.isOrderChecked;
        StaticData.sendNewOrder = data.sendNewOrder;

     
        if (ts != null && GetCurrentOrder() != null)
        {
            ts.StartTimer();
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

        if (SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            Debug.Log("Data loaded in workshop scene. Checking orders...");
            Debug.Log("Hey, I answered the call after loading data!");
            StartCoroutine(HandleWorkshopSceneLoadAfterData());
        }

    }

    private System.Collections.IEnumerator HandleWorkshopSceneLoadAfterData()
    {
        yield return null; // Wait one frame for everything to initialize

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
            Debug.Log("I am going to complete your order!!!");
            Debug.Log($"OrderList count: {orderList.Count}");
            TryCompleteOrder();
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