using UnityEngine;
using UnityEngine.SceneManagement;

public class StationExit : MonoBehaviour
{

    public enum StationType { Tool, Paint, Wire }
    public StationType type;

    public OrderManager om;
    public GameLoopManager glm;
    public RaycastInteractor ri;

    public void ExitStation(){
        Order currentOrder = om?.GetCurrentOrder();
        if (currentOrder != null)
        {
            switch (type)
            {
                case StationType.Tool:
                    if (StaticData.isToolDone)
                    {
                        currentOrder.toolDone = true;
                        Debug.Log("Tool task completed, marking toolDone = true");
                        ri.ToolIndicator.gameObject.SetActive(false);
                        if (StaticData.toolWrong == 0)
                        {
                            Debug.Log("All tools used correctly! Earn 10 points!");
                            glm.toolScore += 10;
                        }
                        else if (StaticData.toolWrong > 0 && StaticData.toolWrong < 3)
                        {
                            Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                            glm.toolScore += 5;
                        }
                        else if (StaticData.toolWrong >= 3)
                        {
                            Debug.Log("You performed poorly! Earn 1 point!");
                            glm.toolScore += 1;
                        }

                        StaticData.toolWrong = 0;
                        Debug.LogWarning("Tool wrongs set to 0!");

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
                        ri.PaintIndicator.gameObject.SetActive(false);
                        if (StaticData.paintWrong == 0)
                        {
                            Debug.Log("All tools used correctly! Earn 10 points!");
                            glm.paintScore += 10;
                        }
                        else if (StaticData.paintWrong > 0 && StaticData.paintWrong < 3)
                        {
                            Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                            glm.paintScore += 5;
                        }
                        else if (StaticData.paintWrong >= 3)
                        {
                            Debug.Log("You performed poorly! Earn 1 points!");
                            glm.paintScore += 1;
                        }

                        StaticData.paintWrong = 0;
                        Debug.LogWarning("Paint wrongs set to 0!");

                    }
                    else
                    {
                        Debug.LogWarning("Paint task not completed yet!");
                       
                    }
                    break;
                case StationType.Wire:
                    if(StaticData.isWireDone)
                    {
                        currentOrder.wireDone = true;
                        Debug.Log("Wire task completed, marking wireDone = true");
                        ri.WireIndicator.gameObject.SetActive(false);
                        if (StaticData.wireWrong == 0)
                        {
                            Debug.Log("All tools used correctly! Earn 10 points!");
                            glm.wireScore += 10;
                        }
                        else if (StaticData.wireWrong > 0 && StaticData.wireWrong < 3)
                        {
                            Debug.Log("Some tools were used incorrectly! Earn 5 points!");
                            glm.wireScore += 5;
                        }
                        else if (StaticData.wireWrong >= 3)
                        {
                            Debug.Log("You performed poorly! Earn 1 points!");
                            glm.wireScore += 1;
                        }

                        StaticData.wireWrong = 0;
                        Debug.LogWarning("Wire wrongs set to 0!");

                    }
                    else
                    {
                        Debug.LogWarning("Wire task not completed yet!");

                    }
                    break;
            }

            Debug.Log($"Successfully returned from {type} station!");
            //OrderManager.Instance.TryCompleteOrder();

            if (StaticData.startOfDay == true)
            {
                Debug.Log("It is the start of day indeed!");
                ri.readyIndicator.gameObject.SetActive(true);
                ri.readyText.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("No it ain't the start of the day!");
                ri.readyIndicator.gameObject.SetActive(false);
                ri.readyText.gameObject.SetActive(false);
            }

            glm.moneyImage.gameObject.SetActive(true);
            glm.dayNumber.gameObject.SetActive(true);
            glm.moneyText.gameObject.SetActive(true);
            glm.onboardImage.gameObject.SetActive(true);
            glm.ordersOnboard.gameObject.SetActive(true);
            glm.ShowTV(true);

            if (TimerScript.instance != null && TimerScript.instance.timer != null)
            {
                TimerScript.instance.timer.gameObject.SetActive(true); // hide
            }


            if (currentOrder.needsTool && !StaticData.isToolDone)
            {
                ri.ToolIndicator.gameObject.SetActive(true);
            }
            if (currentOrder.needsPaint && !StaticData.isPaintDone)
            {
                ri.PaintIndicator.gameObject.SetActive(true);
            }
            if (currentOrder.needsWire && !StaticData.isWireDone)
            {
                ri.WireIndicator.gameObject.SetActive(true);
            }
            

        }

        SceneManager.LoadScene("LO_WS2D");
    }
}
