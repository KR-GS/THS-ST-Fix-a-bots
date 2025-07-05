using UnityEngine;
using UnityEngine.SceneManagement;

public class StationExit : MonoBehaviour
{

    public enum StationType { Tool, Paint, Wire }
    public StationType type;

    public void ExitStation(){

        switch (type)
        {
            case StationType.Tool:
                Debug.Log("Exiting Tool Station");
                break;
            case StationType.Paint:
                Debug.Log("Exiting Paint Station");
                break;
        }

        SceneManager.LoadScene("LO_Stage_Select");
        /*
        // Remove if experiment is finished

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
                    currentOrder.paintDone = true;
                    break;
                case StationType.Wire:
                    currentOrder.wireDone = true;
                    break;
            }

            Debug.Log($"Successfully returned from {type} station!");
            //OrderManager.Instance.TryCompleteOrder();
        }

        SceneManager.LoadScene("LO_Workshop");
        */
    }
}
