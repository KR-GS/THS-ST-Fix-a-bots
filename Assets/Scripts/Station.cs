using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Station : MonoBehaviour
{
    public enum StationType { Tool, Paint, Wire }
    public StationType type;
    public static List<Station> AllStations = new List<Station>();

    private void Awake()
    {
        if (!AllStations.Contains(this))
            AllStations.Add(this);
    }

    private void OnDestroy()
    {
        AllStations.Remove(this);
    }

    private void Start()
    {
        SetStationVisibility();
    }

    public void SetStationVisibility()
    {
        int currentLevel = GameLoopManager.Instance.level;

        switch (type)
        {
            case StationType.Tool:
                gameObject.SetActive(true);
                break;

            case StationType.Paint:
                gameObject.SetActive(currentLevel >= 1); //originally 6
                break;

            case StationType.Wire:
                gameObject.SetActive(currentLevel >= 11);
                break;
        }
    }
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
                    SceneManager.LoadScene("LO_Tool_GUI");
                    //currentOrder.toolDone = true;
                    Debug.Log("Tool station activated!");
                }
                break;

            case StationType.Paint:
                if (currentOrder.needsPaint && !currentOrder.paintDone)
                {
                    SceneManager.LoadScene("LO_Paint_GUI");
                    //currentOrder.paintDone = true;
                    Debug.Log("Paint station activated!");
                }
                break;

            case StationType.Wire:
                if (currentOrder.needsWire && !currentOrder.wireDone)
                {
                    SceneManager.LoadScene("LO_Wire");
                    //currentOrder.wireDone = true;
                    Debug.Log("Wire station activated!");
                }
                break;
        }

        OrderManager.Instance.TryCompleteOrder();
    }
}