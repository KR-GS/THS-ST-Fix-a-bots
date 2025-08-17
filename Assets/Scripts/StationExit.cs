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
                        RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(false);
                        if (StaticData.toolWrong == 0)
                        {
                            Debug.Log("All tools used correctly! Earn 10 points!");
                            GameLoopManager.Instance.toolScore += 10;
                        }
                        else if (StaticData.toolWrong > 0 && StaticData.toolWrong < 3)
                        {
                            Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                            GameLoopManager.Instance.toolScore += 5;
                        }
                        else if (StaticData.toolWrong >= 3)
                        {
                            Debug.Log("You performed poorly! Earn 1 point!");
                            GameLoopManager.Instance.toolScore += 1;
                        }
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
                        if (StaticData.paintWrong == 0)
                        {
                            Debug.Log("All tools used correctly! Earn 10 points!");
                            GameLoopManager.Instance.paintScore += 10;
                        }
                        else if (StaticData.paintWrong > 0 && StaticData.paintWrong < 3)
                        {
                            Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                            GameLoopManager.Instance.paintScore += 5;
                        }
                        else if (StaticData.paintWrong >= 3)
                        {
                            Debug.Log("You performed poorly! Earn 1 points!");
                            GameLoopManager.Instance.paintScore += 1;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Paint task not completed yet!");
                       
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

            
            if(currentOrder.needsTool && !StaticData.isToolDone)
            {
                RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(true);
            }
            if (currentOrder.needsPaint && !StaticData.isPaintDone)
            {
                RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(true);
            }
            if (currentOrder.needsWire && !StaticData.isWireDone)
            {
                RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(true);
            }
            

        }

        SceneManager.LoadScene("LO_WS2D");
    }
}
