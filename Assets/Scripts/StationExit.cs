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
                    currentOrder.toolDone = true;
                    break;
                case StationType.Paint:
                    currentOrder.paintDone = true;
                    break;
                case StationType.Wire:
                    currentOrder.wireDone = true;
                    break;
            }

            Debug.Log($"{type} station task marked complete!");
            //OrderManager.Instance.TryCompleteOrder();
        }

        SceneManager.LoadScene("LO_Workshop");
    }
}
