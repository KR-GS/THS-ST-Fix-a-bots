using UnityEngine;

public class Station : MonoBehaviour
{
    public enum StationType { Tool, Paint, Wire }
    public StationType type;

    public void Interact()
    {
        Order currentOrder = OrderManager.Instance.GetCurrentOrder();
        if (currentOrder == null)
        {
            Debug.LogWarning("No current order found.");
            return;
        }

        switch (type)
        {
            case StationType.Tool:
                if (currentOrder.needsTool && !currentOrder.toolDone)
                {
                    currentOrder.toolDone = true;
                    Debug.Log("Tool station activated!");
                }
                break;

            case StationType.Paint:
                if (currentOrder.needsPaint && !currentOrder.paintDone)
                {
                    currentOrder.paintDone = true;
                    Debug.Log("Paint station activated!");
                }
                break;

            case StationType.Wire:
                if (currentOrder.needsWire && !currentOrder.wireDone)
                {
                    currentOrder.wireDone = true;
                    Debug.Log("Wire station activated!");
                }
                break;
        }

        OrderManager.Instance.TryCompleteOrder();
    }
}