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


public class RaycastInteractor : MonoBehaviour, IDataPersistence
{
    public static RaycastInteractor Instance;
    public float rayLength = 10f;
    private bool orderReceived = false;
    [SerializeField] private GameLoopManager gameLoopManager;
    public GameObject orderSheetPanel;
    public TextMeshProUGUI toolStatus;
    public TextMeshProUGUI wireStatus;
    public TextMeshProUGUI paintStatus;
    public Button okButton;
    public Order currentOrder;

    private void Awake()
    {
        StartCoroutine(DelayedOrderUISetup());
    }

    private IEnumerator DelayedOrderUISetup()
    {
        // Wait 1 frame to ensure OrderManager is initialized and data is loaded
        yield return null;


        if (OrderManager.Instance != null)
        {
            Order savedOrder = OrderManager.Instance.GetCurrentOrder();

            // Check if it's a valid order
            if (savedOrder != null && (savedOrder.needsTool || savedOrder.needsWire || savedOrder.needsPaint))
            {
                currentOrder = savedOrder;
                orderReceived = true;
                ShowOrderUI(currentOrder);
                Debug.Log("[RaycastInteractor] Showing existing order on delayed setup.");
                HideOrderSheetPanel();
            }
            else
            {
                HideOrderSheetPanel(); // keep it hidden if invalid
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
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);


            if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                // Station interaction
                Station station = hit.collider.GetComponent<Station>();
                if (station != null)
                {
                    Debug.Log("Station found: " + station.name);
                    station.Interact();
                    return;
                }

                // TV interaction
                if (hit.collider.CompareTag("TV"))
                {
                    if (orderReceived == false)
                    {
                        orderReceived = true;
                        Debug.Log("TV clicked - generating new order.");
                        currentOrder = OrderManager.Instance.CreateNewOrder();
                        ShowOrderUI(currentOrder);
                    }
                    else
                    {
                        Debug.Log("You have unfinished orders!");
                        if (currentOrder != null)
                        {
                            ShowOrderUI(currentOrder);
                        }
                        else
                        {
                            Debug.LogWarning("No currentOrder found, but orderReceived is true.");
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }
    }

    void ShowOrderUI(Order order)
    {
        orderSheetPanel.gameObject.SetActive(true);

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

    public void LoadData(GameData data)
    {
        this.orderReceived = data.orderReceived;
        currentOrder = OrderManager.Instance.GetCurrentOrder();
    }

    public void SaveData(ref GameData data)
    {
        data.orderReceived = this.orderReceived;
    }

    public void SetFinished(bool ord)
    {
        this.orderReceived = ord;
    }
}