using UnityEngine;
using UnityEngine.SceneManagement;

public class StationExit : MonoBehaviour
{

    public enum StationType { Tool, Paint, Wire }
    public StationType type;

    public void ExitStation(){
        Order currentOrder = OrderManager.Instance?.GetCurrentOrder();
        if (currentOrder != null)
        {
            switch (type)
            {
                case StationType.Tool:
                    if (StaticData.isToolDone)
                    {
                        currentOrder.toolDone = true;
                        Debug.Log("Tool task completed, marking toolDone = true");
                    }
                    else
                    {
                        Debug.LogWarning("Tool task not completed yet!");
                    }
                    break;
                case StationType.Paint:
                    if (StaticData.isPaintDone)
                    {
                        currentOrder.paintDone = true;
                        Debug.Log("Paint task completed, marking paintDone = true");
                    }
                    else
                    {
                        Debug.LogWarning("Tool task not completed yet!");
                    }
                    break;
                case StationType.Wire:
                    currentOrder.wireDone = true;
                    break;
            }

            Debug.Log($"Successfully returned from {type} station!");
            //OrderManager.Instance.TryCompleteOrder();
            GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
            GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
            GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
            GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
            GameLoopManager.Instance.ShowTV(true);
        }

        SceneManager.LoadScene("LO_WS2D");
    }
}
