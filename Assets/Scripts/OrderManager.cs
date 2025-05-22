using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    public List<Order> orderList = new List<Order>();
    public int currentOrderIndex = -1;

    private void Awake()
    {
        Instance = this;
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
        if (GetCurrentOrder().IsComplete())
        {
            Debug.Log("Order Complete! Next level!");
            // Proceed to next order/level
            GameLoopManager.Instance.CompleteLevel(); // or load a new scene, etc.
        }
    }

    public Order GetCurrentOrder()
    {
        if (currentOrderIndex >= 0 && currentOrderIndex < orderList.Count)
            return orderList[currentOrderIndex];

        return null;
    }
}