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
    [SerializeField] private TutorialManager tm;
    [SerializeField] private float notificationDuration = 2f;
    private Button button;
    public Button nextdayButton;
    public TextMeshProUGUI completeText;
    public TextMeshProUGUI totalaccumulatedText;
    public TextMeshProUGUI initialBalanceText;
    public TextMeshProUGUI finalBalanceText;
    public int prize;
    public bool isFinished = false;
    private bool sendNewOrder = false;
    private bool wasRaycastingEnabled = true;
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



    private bool wireHover = false;
    private bool toolHover = false;
    private bool paintHover = false;

    private bool isScheduleRunning = false;

    public bool didExit = false;

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
            DataPersistenceManager.Instance.SaveGame();
            Debug.Log("Game saved after scene load in OrderManager with StaticData Tool being = ." + StaticData.isToolDone);
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

        if (orderReceived && didExit == true)
        {
            Debug.Log("Oh, you must've exited the app after completing a task... wait hold on for a bit");
            Debug.Log("You have " + activeOrders.Count + " active orders and " + pendingOrders.Count + " pending orders.");
            Debug.Log("Is the scheduler running? Here's the answer!" + isScheduleRunning);
            if (activeOrders.Count == 0 && pendingOrders.Count > 0 && isScheduleRunning == true)
            {
                sendNewOrder = true;
                isScheduleRunning = false;
                didExit = false;
                StartCoroutine(ScheduleNextOrder());
                Debug.Log("Fixed. Now we wait!");
            }
        }
        else if (orderReceived && didExit == false)
        {
            Debug.Log("You did return from the station right?");
            Debug.Log("You have " + activeOrders.Count + " active orders and " + pendingOrders.Count + " pending orders.");
            Debug.Log("Is the scheduler running? Here's the answer!" + isScheduleRunning);
            if (activeOrders.Count == 0 && pendingOrders.Count > 0 && isScheduleRunning == true)
            {
                sendNewOrder = true;
                isScheduleRunning = false;
                didExit = false;
                StartCoroutine(ScheduleNextOrder());
                Debug.Log("Sending your next order!");
            }
        }
    }
    public void Start()
    {
        if (StaticData.isFirstWS == true)
        {
            StartCoroutine(WaitForLoadingThenShowTutorial());
        }
    }

    private IEnumerator WaitForLoadingThenShowTutorial()
    {
        GameObject loadingScreen = null;

        // Find the loading screen object (adjust the name to match your actual GameObject name)
        loadingScreen = GameObject.Find("LoadingScreenMath"); // or whatever it's named

        // If not found by name, you can search through all DontDestroyOnLoad objects
        if (loadingScreen == null)
        {
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

            foreach (GameObject obj in allObjects)
            {
                if (obj.scene.name == null || obj.scene.name == "DontDestroyOnLoad")
                {
                    if (obj.name.Contains("Math") || obj.name.Contains("Loading")) // adjust to your naming
                    {
                        loadingScreen = obj;
                        break;
                    }
                }
            }
        }

        if (loadingScreen != null)
        {
            while (loadingScreen.activeSelf)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.3f);

        OpenTutorial();
    }

    public void OpenTutorial()
    {
        glm.HideWorkshopElements();
        tm.OpenTutorial();
        //ri.enabled = false;
        Debug.Log("Raycasting disabled for tutorial");
        ts.StopTimer();
    }

    public void CloseTutorial()
    {
        glm.ShowWorkshopElements();
        //wasRaycastingEnabled = ri.enabled;
        //ri.enabled = true;
        StaticData.isFirstWS = false;

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


        if (StaticData.lookAtOrder == true)
        {
            glm.HideWorkshopElements();
        }

        if (orderReceived)
        {
            Debug.Log("Welcome back after the tutorial! Let me reset things for you!");
            Debug.Log("You have " + activeOrders.Count + " active orders and " + pendingOrders.Count + " pending orders.");
            Debug.Log("Is the scheduler running? Here's the answer!" + isScheduleRunning);
            if (activeOrders.Count == 0 && pendingOrders.Count > 0 && isScheduleRunning == true)
            {
                sendNewOrder = true;
                isScheduleRunning = false;
                didExit = false;
                StartCoroutine(ScheduleNextOrder());
                Debug.Log("Fixed. Now we wait!");
            }
        }

        if (ts != null && ts.timer != null)
        {
            ts.timer.gameObject.SetActive(true); // hide
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

        /*
        //QA Data

        if (level >= 1)
        {
            float rand = Random.value;
            newOrder.needsTool = Random.value < 0.5f;
            newOrder.needsPaint = Random.value < 0.5f;
            newOrder.needsWire = Random.value < 0.5f;
        }
        */

        
        if (level >= 1 && level < 3) // && level < 3
        {
            newOrder.needsTool = true; //originally true, gonna QA
            newOrder.needsPaint = false; //originally false, gonna QA
            newOrder.needsWire = false; //originally false, gonna QA
        }

        
        else if (level >= 3 && level < 6)
        {
            float rand = Random.value;
            newOrder.needsTool = Random.value < 0.5f;
            newOrder.needsPaint = Random.value < 0.5f;
            newOrder.needsWire = false;
        }

        
        else if (level >= 6)
        {
            float rand = Random.value;
            newOrder.needsTool = Random.value < 0.5f;
            newOrder.needsPaint = Random.value < 0.5f;
            newOrder.needsWire = Random.value < 0.5f;
        }
        

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
        Debug.Log("Pending orders count: " + pendingOrders.Count);
        while (pendingOrders.Count > 0)
        {
            Debug.Log("I do have lots right?");
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

    public void SendNewOrderDebug()
    {
        Debug.Log("Sending new order because the other one is bugged lol.");
        Debug.Log("Pending orders count: " + pendingOrders.Count);
        while (pendingOrders.Count > 0)
        {
            Debug.Log("I do have lots right?");
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
                if(toolHover == false)
                {
                    toolHover = true;

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
                }

                StaticData.toolWrong = 0;
                Debug.LogWarning("Tool wrongs set to 0!");
            }

            if (StaticData.isPaintDone == true)
            {
                Debug.Log("Magnificent... you did complete sticker!");
                orderList[0].paintDone = true;
                ri.PaintIndicator.gameObject.SetActive(false);
                if (paintHover == false)
                {
                    paintHover = true;
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
                }

                StaticData.paintWrong = 0;
                Debug.LogWarning("Paint wrongs set to 0!");
            }

            if (StaticData.isWireDone == true)
            {
                Debug.Log("Shocking... you did complete wire!");
                orderList[0].wireDone = true;
                ri.WireIndicator.gameObject.SetActive(false);
                if(wireHover == false)
                {
                    wireHover = true;
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
                }

                StaticData.wireWrong = 0;
                Debug.LogWarning("Wire wrongs set to 0!");
            }
        }

 
        if (orderList[0].IsComplete())
        {
            Debug.Log("Order Complete!");
            ri.TVSprite.sprite = TVSpriteNoOrder;
            StaticData.orderNumber += 1;
            orderList.RemoveAt(0);
            activeOrders.RemoveAt(0);
            StaticData.orderList = orderList;
            StaticData.activeOrders = activeOrders;
            StaticData.pendingOrders = pendingOrders;
            StaticData.isToolDone = false;
            StaticData.isPaintDone = false;
            StaticData.isWireDone = false;
            wireHover = false;
            toolHover = false;
            paintHover = false;
            StaticData.debugPaint = false;
            StaticData.debugTool = false;
            StaticData.debugWire = false;
            StaticData.isOrderChecked = false;
            StaticData.incorrectIndices.Clear();
            StaticData.incorrectValues.Clear();
            StaticData.missingVals = 0;
            glm.GenerateAndStorePattern();

            
            if (ts != null)
            {
                glm.prizeText.gameObject.SetActive(true);
                glm.prizeImej.gameObject.SetActive(true);
                if (ts.timeLft < 900)
                {
                    if(pendingOrders.Count > 0)
                    {
                        Debug.Log("Order completed on time! You receive full amount as payment!");
                        glm.prizeText.text = "+50";
                        prize += 50;
                        glm.money += 50;
                        StartCoroutine(HideNotificationAfterDelay());
                    }
                    else
                    {
                        prize += 50;
                        glm.money += 50;
                    }

                }
                else
                {
                    if(pendingOrders.Count > 0)
                    {
                        Debug.Log("Order completed late! You receive half amount as payment!");
                        glm.prizeText.text = "+25";
                        prize += 25;
                        glm.money += 25;
                        StartCoroutine(HideNotificationAfterDelay());
                    }
                    else
                    {
                        prize += 25;
                        glm.money += 25;
                    }
                    
                }
            }


        }

        if (orderList.Count == 0)
        {
            isFinished = true;
            Debug.Log("All Orders Complete!");
            ts.StopTimer();
            ShowOrderCompletePanel();
            

        }

        DataPersistenceManager.Instance.SaveGame();

    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);
        glm.prizeImej.gameObject.SetActive(false);
        glm.prizeText.gameObject.SetActive(false);
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
        glm.prizeText.gameObject.SetActive(false);
        Debug.Log("[OrderManager] Showing panel: " + orderCompletePanel.name);

        totalaccumulatedText.text = "+" + prize.ToString();
        initialBalanceText.text = (glm.money - prize).ToString();
        finalBalanceText.text = glm.money.ToString();
        glm.tutorialButton.gameObject.SetActive(false);
        glm.dayNumber.gameObject.SetActive(false);
        glm.calendar.gameObject.SetActive(false);
        glm.shopButton.gameObject.SetActive(true);

        if (nextdayButton != null)
        {
            nextdayButton.gameObject.SetActive(true);
            nextdayButton.onClick.RemoveAllListeners();
            nextdayButton.onClick.AddListener(() =>
            {
                orderCompletePanel.SetActive(false);
                ri.timeText.gameObject.SetActive(false);
                LoadingScreenManager.Instance.SwitchtoSceneMath(7, () =>
                {
                    HideOrderCompletePanel();
                    glm.CompleteLevel();
                });
            });
        }
    }

    public void HideOrderCompletePanel()
    {
        if (orderCompletePanel == null)
        {
            Debug.LogWarning("[OrderManager] Order Complete Panel not assigned � nothing to hide.");
            return;
        }

        //LoadingScreenManager.Instance.SwitchtoSceneMath(7);


        //orderCompletePanel.SetActive(false);
        Debug.Log("[OrderManager] Hiding panel: " + orderCompletePanel.name);
        Debug.Log("[LOOK HERE] Pending orders count: " + pendingOrders.Count);

        //completeText.gameObject.SetActive(false);
        glm.dayNumber.gameObject.SetActive(true);
        glm.calendar.gameObject.SetActive(true);
        glm.tutorialButton.gameObject.SetActive(true);
        glm.shopButton.gameObject.SetActive(false);

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
        this.toolHover = data.toolHover;
        this.paintHover = data.paintHover;
        this.wireHover = data.wireHover;
        this.isScheduleRunning = data.isSchedulerRunning;
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
        data.toolHover = this.toolHover;
        data.paintHover = this.paintHover;
        data.wireHover = this.wireHover;
        data.isSchedulerRunning = this.isScheduleRunning;
        didExit = true;
        data.didExit = this.didExit;
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