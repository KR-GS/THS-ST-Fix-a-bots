using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


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
        HideOrderSheetPanel();

        if (OrderManager.Instance != null && OrderManager.Instance.GetCurrentOrder() != null)
        {
            currentOrder = OrderManager.Instance.GetCurrentOrder();
            orderReceived = true;
            ShowOrderUI(currentOrder);
            Debug.Log("[RaycastInteractor] Showing existing order on Awake.");
        }
    }

    public void HideOrderSheetPanel()
    {
        orderSheetPanel.SetActive(false);
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
  
        toolStatus.text = order.needsTool ? "Tool - Not Done" : "Tool - Not Needed";
        wireStatus.text = order.needsWire ? "Wire - Not Done" : "Wire - Not Needed";
        paintStatus.text = order.needsPaint ? "Paint - Not Done" : "Paint - Not Needed";

  
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