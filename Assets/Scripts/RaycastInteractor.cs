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
using UnityEngine.EventSystems;



public class RaycastInteractor : MonoBehaviour
{
    public static RaycastInteractor Instance;
    public float rayLength = 10f;
    public TextMeshProUGUI timeText;
    [SerializeField] private GameLoopManager gameLoopManager;
    [SerializeField] private TimerScript ts;
    public GameObject orderSheetPanel;
    public bool isOrderChecked = false;

    public OrderManager om;
    public GameLoopManager glm;

    public TextMeshProUGUI botName;
    public TextMeshProUGUI customerName;

    public Image readyIndicator;
    public TextMeshProUGUI readyText;

    public Button okButton;
    public Order currentOrder;
    private Queue<Order> pendingOrders = new Queue<Order>();
    public SpriteRenderer TVSprite;       
    public Sprite TVSpriteIP;             
    public Sprite TVSpriteNO;
    public Sprite TVSpriteNoOrder;

    public Button pauseButton;

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

        if (ToolIndicator != null) ToolIndicator.gameObject.SetActive(false);
        if (WireIndicator != null) WireIndicator.gameObject.SetActive(false);
        if (PaintIndicator != null) PaintIndicator.gameObject.SetActive(false);

        StartCoroutine(DelayedOrderUISetup());

    }

    private void Start()
    {
        if (om.GetActiveOrder() != null) //om != null && om.GetActiveOrder() != null
        {
            Debug.Log("Is this for real???");
            if (TVSprite != null)
            {
                TVSprite.sprite = TVSpriteIP;
                readyIndicator.gameObject.SetActive(false);
                readyText.gameObject.SetActive(false);
            }

            glm.moneyImage.gameObject.SetActive(true);
            glm.dayNumber.gameObject.SetActive(true);
            glm.moneyText.gameObject.SetActive(true);
            glm.onboardImage.gameObject.SetActive(true);
            glm.ordersOnboard.gameObject.SetActive(true);
            glm.ShowTV(true);

            if (ts != null && ts.timer != null)
            {
                ts.timer.gameObject.SetActive(true); // hide
            }

            if (currentOrder.needsTool && !StaticData.isToolDone)
            {
                ToolIndicator.gameObject.SetActive(true);
            }
            if (currentOrder.needsPaint && !StaticData.isPaintDone)
            {
                PaintIndicator.gameObject.SetActive(true);
            }
            if (currentOrder.needsWire && !StaticData.isWireDone)
            {
                WireIndicator.gameObject.SetActive(true);
            }
        }
        else if (om.GetActiveOrder() == null) // != null && om.GetActiveOrder() == null
        {
            Debug.Log("No shot sherlock!");
            if (TVSprite != null)
            {
                TVSprite.sprite = TVSpriteNoOrder;
            }

            if (ToolIndicator != null) ToolIndicator.gameObject.SetActive(false);
            if (WireIndicator != null) WireIndicator.gameObject.SetActive(false);
            if (PaintIndicator != null) PaintIndicator.gameObject.SetActive(false);


            if (StaticData.startOfDay == true)
            {
                Debug.Log("It is the start of day indeed!");
                readyIndicator.gameObject.SetActive(true);
                readyText.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("No it ain't the start of the day!");
                readyIndicator.gameObject.SetActive(false);
                readyText.gameObject.SetActive(false);
            }

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

        if (om != null)
        {
            Order savedOrder = om.GetCurrentOrder();

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

            Order savedOrder = om.GetActiveOrder();

            if (TVSprite != null && TVSprite.sprite == TVSpriteNO)
            {
                TVSprite.sprite = TVSpriteIP;
            }

            Debug.Log("Internal tool score: " + glm.toolScore);
            Debug.Log("Internal paint score: " + glm.paintScore);
            Debug.Log("Internal wire score: " + glm.wireScore);

            glm.moneyImage.gameObject.SetActive(true);
            glm.dayNumber.gameObject.SetActive(true);
            glm.moneyText.gameObject.SetActive(true);
            glm.onboardImage.gameObject.SetActive(true);
            glm.ordersOnboard.gameObject.SetActive(true);
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

                //om.TryCompleteOrder()

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

            if (StaticData.startOfDay == true)
            {
                Debug.Log("It is the start of day indeed!");
            }
            else
            {
                Debug.Log("No it ain't the start of the day!");
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
            if (IsPointerOverUI(Input.GetTouch(0).position))
            {
                Debug.Log("Pressing the button!");
                return; 
            }

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
                    if (!StaticData.orderReceived)
                    {
                        om.StartOrderBatch();
                        ts.timer.gameObject.SetActive(true);

                        if (readyIndicator != null && readyText != null)
                        {
                            if (StaticData.startOfDay == true)
                            {
                                Debug.Log("Ready indicator status before TV: " + StaticData.startOfDay);
                                readyIndicator.gameObject.SetActive(false);
                                readyText.gameObject.SetActive(false);
                                StaticData.startOfDay = false;
                                Debug.Log("Ready indicator status after TV: " + StaticData.startOfDay);
                                StaticData.orderReceived = true;

                            }
                        }
                    }
                    else
                    {
                        Debug.Log("You have unfinished orders!");
                        if (om.currentOrder != null)
                        {
                            ShowOrderUI();
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

    private bool IsPointerOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            Debug.Log("UI Raycast hit: " + result.gameObject.name);
        }

        return results.Count > 0;
    }

    private IEnumerator ScheduleNextOrder()
    {
        while (pendingOrders.Count > 0)
        {

            yield return new WaitForSeconds(1f); // small delay before showing next order

            Order nextOrder = pendingOrders.Dequeue();
            om.AddToActiveOrders(nextOrder);
            currentOrder = nextOrder;

            Debug.Log("Order delivered to active queue!");
            
        }
    }

    void ShowOrderUI()
    {
        //Get all the data from StaticData

        if (om.activeOrders.Count == 0)
        {
            Debug.LogWarning("No active orders to show!");
            return;
        }

        Order order = om.GetActiveOrder(); // Get the first active order (activeOrders[0])

        customerName.text = order.customername;
        botName.text = order.robotname;

        orderSheetPanel.gameObject.SetActive(true);
        glm.moneyImage.gameObject.SetActive(false);
        glm.dayNumber.gameObject.SetActive(false);
        glm.moneyText.gameObject.SetActive(false);
        glm.onboardImage.gameObject.SetActive(false);
        glm.ordersOnboard.gameObject.SetActive(false);

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
