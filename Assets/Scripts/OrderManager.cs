using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour, IDataPersistence
{
    public static OrderManager Instance;
    [SerializeField] private GameObject orderCompletePanel;
    [SerializeField] private RaycastInteractor raycastInteractor;
    private Button button;
    private int prize;
    private bool isFinished = false;

    public List<Order> orderList = new List<Order>();
    public List<Order> activeOrders = new List<Order>();
    public Queue<Order> pendingOrders = new Queue<Order>();
    private Coroutine deliveryRoutine;
    public Order currentOrder;
    public bool orderReceived;
    public SpriteRenderer TVSprite;
    public Sprite TVSpriteIP;
    public Sprite TVSpriteNO;


    public int currentOrderIndex = -1;


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

        HideOrderCompletePanel();

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

        if (isFinished)
        {
            Debug.Log("isFinished was true. Showing complete panel...");
            ShowOrderCompletePanel();
            //raycastInteractor.enabled = false;
        }
        else
        {
            TryCompleteOrder(); 
        }
    }

    public Order CreateNewOrder()
    {
        /*
        Order newOrder = new Order
        {
            //needsTool = Random.value > 0.99f

            needsTool = Random.value > 0.0f,
            needsPaint = Random.value > 1.0f,
            //needsWire = Random.value > 0.5f
        };
        */

        int level = GameLoopManager.Instance.level; // adjust if you track level differently
        Order newOrder = new Order();

      
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
            newOrder.needsWire = false;
        }
        
        // Ensure at least one requirement
        if (!newOrder.needsTool && !newOrder.needsPaint && !newOrder.needsWire)
            newOrder.needsTool = true;

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
        pendingOrders.Clear();
        if (!orderReceived)
        {
            orderReceived = true;
            for (int i = 0; i < 5; i++)
            {
                Order o = CreateNewOrder();
                pendingOrders.Enqueue(o);
            }
            Instance.StartCoroutine(ScheduleNextOrder());
        }
    }

    private IEnumerator ScheduleNextOrder()
    {
        while (pendingOrders.Count > 0)
        {
            yield return new WaitForSeconds(5f);
            var nextOrder = pendingOrders.Dequeue();
            AddToActiveOrders(nextOrder); 
            Debug.Log("Delivered order!");
            TVSprite.sprite = TVSpriteNO;
        }
    }
    public void TryCompleteOrder()
    {
        if (orderList == null || orderList.Count == 0)
            return;

 
        if (orderList[0].IsComplete())
        {
            Debug.Log("Order Complete!");
            
            orderList.RemoveAt(0);
            activeOrders.RemoveAt(0); 
            StaticData.isToolDone = false;
            StaticData.isPaintDone = false;
            StaticData.isWireDone = false;
            StaticData.isOrderChecked = false;
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
        }

        GameLoopManager.Instance.UpdateRemainingOrders();

        if (orderList.Count == 0)
        {
            isFinished = true;
            Debug.Log("All Orders Complete!");
            TimerScript.instance.StopTimer();
            ShowOrderCompletePanel();
            //raycastInteractor.enabled = false;

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

    public void ShowOrderCompletePanel()
    {
        /*
        Transform prizeTextFind = orderCompletePanel.transform.Find("PrizeText");
        if (prizeTextFind != null)
        {
            TextMeshProUGUI prizeTexts = prizeTextFind.GetComponent<TextMeshProUGUI>();
            if (prize == 1)
            {
                prizeTexts.text = "Order completed on time!";
            }
            else
            {
                prizeTexts.text = "That was disappointing... Earned nothing.";
            }
        }
        */
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