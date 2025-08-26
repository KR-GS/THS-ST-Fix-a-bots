using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;



public class RaycastInteractor : MonoBehaviour
{
    public static RaycastInteractor Instance;
    public float rayLength = 10f;
    public TextMeshProUGUI timeText;
    [SerializeField] private GameLoopManager gameLoopManager;
    public GameObject orderSheetPanel;
    public bool isOrderChecked = false;
    

    //public TextMeshProUGUI toolStatus;
    //public TextMeshProUGUI wireStatus;
    //public TextMeshProUGUI paintStatus;

    public Button okButton;
    public Order currentOrder;
    private Queue<Order> pendingOrders = new Queue<Order>();
    public SpriteRenderer TVSprite;       
    public Sprite TVSpriteIP;             
    public Sprite TVSpriteNO;
    public Sprite TVSpriteNoOrder;

    public Image ToolIndicator;
    public Image WireIndicator;
    public Image PaintIndicator;

    [Header("Station Statuses")]
    public UIStationStatus toolStatus;
    public UIStationStatus wireStatus;
    public UIStationStatus paintStatus;

    [Header("Status Sprites")]
    public Sprite completeSprite;
    public Sprite notDoneSprite;

    [System.Serializable]
    public class UIStationStatus
    {
        public GameObject root;         
        public Image iconImage;         
        public TextMeshProUGUI dash;    
        public Image statusImage;       
        public Sprite stationSprite;    
    }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (ToolIndicator != null) ToolIndicator.gameObject.SetActive(false);
        if (WireIndicator != null) WireIndicator.gameObject.SetActive(false);
        if (PaintIndicator != null) PaintIndicator.gameObject.SetActive(false);

        StartCoroutine(DelayedOrderUISetup());

    }

    private void Start()
    {
        if (OrderManager.Instance != null && OrderManager.Instance.GetActiveOrder() != null)
        {
            if (TVSprite != null)
            {
                TVSprite.sprite = TVSpriteIP;
            }
        }
        else if (OrderManager.Instance != null && OrderManager.Instance.GetActiveOrder() == null)
        {
            if (TVSprite != null)
            {
                TVSprite.sprite = TVSpriteNoOrder;
            }

            if (ToolIndicator != null) ToolIndicator.gameObject.SetActive(false);
            if (WireIndicator != null) WireIndicator.gameObject.SetActive(false);
            if (PaintIndicator != null) PaintIndicator.gameObject.SetActive(false);
        }
    }

    private void UpdateUIStation(UIStationStatus status, bool needed, bool isDone)
    {
        if (status.root == null) return;

        status.root.SetActive(needed);

        if (needed)
        {
            status.iconImage.sprite = status.stationSprite;
            status.dash.text = "-";
            status.statusImage.sprite = isDone ? completeSprite : notDoneSprite;
        }
    }

    private IEnumerator DelayedOrderUISetup()
    {
        yield return null;

        if (OrderManager.Instance != null)
        {
            Order savedOrder = OrderManager.Instance.GetCurrentOrder();

            if (savedOrder != null && (savedOrder.needsTool || savedOrder.needsWire || savedOrder.needsPaint))
            {
                currentOrder = savedOrder;
                ShowOrderUI();
                Debug.Log("[RaycastInteractor] Showing existing order on delayed setup.");
                HideOrderSheetPanel();

            }
            else
            {
                HideOrderSheetPanel(); 
                Debug.Log("[RaycastInteractor] No valid saved order to display.");
            }
        }
        else
        {
            Debug.LogWarning("[RaycastInteractor] OrderManager.Instance is null in coroutine.");
        }
    }

    public void HideOrderSheetPanel()
    {
        if (orderSheetPanel != null)
        {
            Debug.Log("Hiding order sheet panel.");
            orderSheetPanel.SetActive(false);

            Order savedOrder = OrderManager.Instance.GetActiveOrder();

            if (TVSprite != null && TVSprite.sprite == TVSpriteNO)
            {
                TVSprite.sprite = TVSpriteIP;
            }

            Debug.Log("Internal tool score: " + GameLoopManager.Instance.toolScore);
            Debug.Log("Internal paint score: " + GameLoopManager.Instance.paintScore);
            Debug.Log("Internal wire score: " + GameLoopManager.Instance.wireScore);

            GameLoopManager.Instance.moneyImage.gameObject.SetActive(true);
            GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
            GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
            GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
            GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
            if (timeText != null)
            {
                timeText.gameObject.SetActive(true); // Hide the time text
            }

            isOrderChecked = StaticData.isOrderChecked;
            Debug.Log("isOrderChecked status: " + isOrderChecked);

            if (!isOrderChecked && savedOrder != null)
            {

                Debug.Log("Setting indicators active...");

                if (savedOrder.needsTool)
                {
                    ToolIndicator.gameObject.SetActive(true);
                    Debug.Log("ToolIndicator enabled");
                }
                if (savedOrder.needsWire)
                {
                    WireIndicator.gameObject.SetActive(true);
                    Debug.Log("WireIndicator enabled");
                }
                if (savedOrder.needsPaint)
                {
                    PaintIndicator.gameObject.SetActive(true);
                    Debug.Log("PaintIndicator enabled");
                }

                isOrderChecked = true;
                StaticData.isOrderChecked = true; 
            }
            else if (isOrderChecked && savedOrder != null)
            {
                Debug.Log("Order is checked, returning old indicators...");

                if (savedOrder.needsTool && !StaticData.isToolDone)
                {
                    ToolIndicator.gameObject.SetActive(true);
                }
                if (savedOrder.needsPaint && !StaticData.isPaintDone)
                {
                    PaintIndicator.gameObject.SetActive(true);
                }
                if (savedOrder.needsWire && !StaticData.isWireDone)
                {
                    WireIndicator.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log($"Indicators not set. isOrderChecked={isOrderChecked}, currentOrder={savedOrder}");
            }


        }
        else
        {
            Debug.LogWarning("orderSheetPanel is null — can't hide it.");
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;

            // Convert screen position to world position for 2D
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
            worldPosition.z = 0f; // Ensure Z is 0 for 2D

            // For 2D, we typically use OverlapPoint instead of raycast for touch detection
            Vector2 worldPoint = new Vector2(worldPosition.x, worldPosition.y);

            // Use OverlapPoint to detect what's at the touch position
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

            if (hitCollider != null)
            {
                Debug.Log("2D Point detection hit: " + hitCollider.name);

                // Station interaction
                Station station = hitCollider.GetComponent<Station>();
                if (station != null)
                {
                    Debug.Log("Station found: " + station.name);
                    station.Interact();
                    return;
                }

                // TV interaction
                if (hitCollider.CompareTag("TV"))
                {
                    if (!OrderManager.Instance.orderReceived)
                    {
                        OrderManager.Instance.StartOrderBatch();
                        TimerScript.instance.timer.gameObject.SetActive(true);
                        GameLoopManager.Instance.UpdateRemainingOrders();
                    }
                    else
                    {
                        Debug.Log("You have unfinished orders!");
                        if (OrderManager.Instance.currentOrder != null)
                        {
                            ShowOrderUI();
                            //ShowOrderUI(OrderManager.Instance.currentOrder);
                        }
                    }

                }
            }
            else
            {
                Debug.Log("2D Point detection did not hit anything.");
            }
        }
    }

    private IEnumerator ScheduleNextOrder()
    {
        while (pendingOrders.Count > 0)
        {

            yield return new WaitForSeconds(1f); // small delay before showing next order

            Order nextOrder = pendingOrders.Dequeue();
            OrderManager.Instance.AddToActiveOrders(nextOrder);
            currentOrder = nextOrder;

            Debug.Log("Order delivered to active queue!");
            
        }
    }

    void ShowOrderUI()
    {
        //Get all the data from StaticData
        

        if (OrderManager.Instance.activeOrders.Count == 0)
        {
            Debug.LogWarning("No active orders to show!");
            return;
        }

        Order order = OrderManager.Instance.GetActiveOrder(); // Get the first active order (activeOrders[0])

        orderSheetPanel.gameObject.SetActive(true);
        GameLoopManager.Instance.moneyImage.gameObject.SetActive(false);
        GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
        GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);

        if (ToolIndicator != null) ToolIndicator.gameObject.SetActive(false);
        if (WireIndicator != null) WireIndicator.gameObject.SetActive(false);
        if (PaintIndicator != null) PaintIndicator.gameObject.SetActive(false);

        if (timeText != null)
        {
            timeText.gameObject.SetActive(false); // Hide the time text
        }

        UpdateUIStation(toolStatus, order.needsTool, StaticData.isToolDone);
        UpdateUIStation(wireStatus, order.needsWire, StaticData.isWireDone);
        UpdateUIStation(paintStatus, order.needsPaint, StaticData.isPaintDone);

        okButton.gameObject.SetActive(true);
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            HideOrderSheetPanel();
        });

    }

}
