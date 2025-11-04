using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Station : MonoBehaviour
{
    public enum StationType { Tool, Paint, Wire }
    public StationType type;
    public static List<Station> AllStations = new List<Station>();
    public OrderManager om;
    public GameLoopManager glm;

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
        int currentLevel = glm.level;

        switch (type)
        {
            case StationType.Tool:
                gameObject.SetActive(true);
                break;

            case StationType.Paint:
                gameObject.SetActive(true); //originally currentLevel >= 6
                break;

            case StationType.Wire:
                gameObject.SetActive(true); //originally 11
                break;
        }
    }
    public void Interact()
    {
        Order currentOrder = om.GetActiveOrder();

        if (currentOrder == null)
        {
            Debug.LogWarning("No current order found.");
            return;
        }

        if(StaticData.isOrderChecked == true)
        {
            switch (type)
            {
                case StationType.Tool:
                    if (currentOrder.needsTool && !StaticData.isToolDone)
                    {
                        DataPersistenceManager.Instance.SaveGame();
                        glm.HideWorkshopElements();
                        LoadingScreenManager.Instance.SwitchtoSceneMath(4);
                        //SceneManager.LoadScene("LO_Tool_GUI");
                        Debug.Log("Tool station activated!");
                    }
                    else
                    {
                        Debug.Log("You do not need tool right now!");
                    }
                    break;

                case StationType.Paint:
                    if (currentOrder.needsPaint && !StaticData.isPaintDone)
                    {
                        DataPersistenceManager.Instance.SaveGame();
                        glm.HideWorkshopElements();
                        LoadingScreenManager.Instance.SwitchtoSceneMath(5);
                        //SceneManager.LoadScene("LO_Paint_GUI");
                        Debug.Log("Paint station activated!");
                    }
                    else
                    {
                        Debug.Log("You do not need paint right now!");
                    }
                    break;

                case StationType.Wire:
                    if (currentOrder.needsWire && !StaticData.isWireDone)
                    {
                        DataPersistenceManager.Instance.SaveGame();
                        glm.HideWorkshopElements();
                        LoadingScreenManager.Instance.SwitchtoSceneMath(6);
                        //SceneManager.LoadScene("LO_Wire");
                        Debug.Log("Wire station activated!");
                    }
                    else
                    {
                        Debug.Log("You do not need wire right now!");
                    }
                    break;
            }
        }
        

        om.TryCompleteOrder();
    }
}