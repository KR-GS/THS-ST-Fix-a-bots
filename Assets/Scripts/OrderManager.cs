using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour, IDataPersistence
{
    public static OrderManager Instance;
    [SerializeField] private GameObject orderCompletePanel;
    [SerializeField] private RaycastInteractor raycastInteractor;
    private Button button;

    public List<Order> orderList = new List<Order>();
    public int currentOrderIndex = -1;


    private void Awake()
    {
        Instance = this;
        HideOrderCompletePanel();
    }

    public void CreateNewOrder()
    {
        Order newOrder = new Order
        {
            needsTool = Random.value > 0.5f,
            needsPaint = Random.value > 0.5f,
            needsWire = Random.value > 0.5f
        };

        // Ensure at least one requirement
        if (!newOrder.needsTool && !newOrder.needsPaint && !newOrder.needsWire)
            newOrder.needsTool = true;

        orderList.Add(newOrder);
        currentOrderIndex = orderList.Count - 1;

        Debug.Log("New Order Created!");
    }

    public void TryCompleteOrder()
    {
        if (GetCurrentOrder()?.IsComplete() ?? false)
        {
            Debug.Log("Order Complete!");
            ShowOrderCompletePanel();
            raycastInteractor.enabled = false;
            orderList.RemoveAt(currentOrderIndex);
            currentOrderIndex = Mathf.Clamp(currentOrderIndex - 1, 0, orderList.Count - 1);
        }
    }

    public Order GetCurrentOrder()
    {
        if (currentOrderIndex >= 0 && currentOrderIndex < orderList.Count)
            return orderList[currentOrderIndex];

        return null;
    }

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
        raycastInteractor.enabled = true;
    }

    public void LoadData(GameData data)
    {
        this.orderList = data.savedOrders ?? new List<Order>();
        this.currentOrderIndex = data.currentOrderIndex;
    }

    public void SaveData(ref GameData data)
    {
        data.savedOrders = this.orderList;
        data.currentOrderIndex = this.currentOrderIndex;
    }


}