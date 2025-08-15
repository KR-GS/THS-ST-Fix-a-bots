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
    public TextMeshProUGUI toolStatus;
    public TextMeshProUGUI wireStatus;
    public TextMeshProUGUI paintStatus;
    public Button okButton;
    public Order currentOrder;
    private Queue<Order> pendingOrders = new Queue<Order>();
    public SpriteRenderer TVSprite;       
    public Sprite TVSpriteIP;             
    public Sprite TVSpriteNO;

    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(DelayedOrderUISetup());
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

            if (TVSprite != null && TVSprite.sprite == TVSpriteNO)
            {
                TVSprite.sprite = TVSpriteIP;
            }

            GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
            GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
            GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
            GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
            if (timeText != null)
            {
                timeText.gameObject.SetActive(true); // Hide the time text
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
            yield return new WaitForSeconds(5f); // Delay between orders

            Order nextOrder = pendingOrders.Dequeue();
            OrderManager.Instance.AddToActiveOrders(nextOrder); // Push to active orders

            Debug.Log("Order delivered to active queue!");
            // You can play a notification sound or blink the TV here
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

        Order order = OrderManager.Instance.activeOrders[0]; // Get the first active order

        orderSheetPanel.gameObject.SetActive(true);

        GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
        GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);
        if(timeText != null)
        {
            timeText.gameObject.SetActive(false); // Hide the time text
        }

        if (order.needsTool)
        {
            toolStatus.gameObject.SetActive(true);
            toolStatus.text = StaticData.isToolDone ? "Tool - Complete" : "Tool - Not Done";
        }
        else
        {
            toolStatus.gameObject.SetActive(false);
        }

        if (order.needsWire)
        {
            wireStatus.gameObject.SetActive(true);
            wireStatus.text = StaticData.isWireDone ? "Wire - Complete" : "Wire - Not Done";
        }
        else
        {
            wireStatus.gameObject.SetActive(false);
        }

        if (order.needsPaint)
        {
            paintStatus.gameObject.SetActive(true);
            paintStatus.text = StaticData.isPaintDone ? "Paint - Complete" : "Paint - Not Done";
        }
        else
        {
            paintStatus.gameObject.SetActive(false);
        }

        okButton.gameObject.SetActive(true);
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            HideOrderSheetPanel();
        });
    }

}
