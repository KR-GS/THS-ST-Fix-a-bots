using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Station : MonoBehaviour
{
    public enum StationType { Tool, Paint, Wire }
    public StationType type;
    public static List<Station> AllStations = new List<Station>();
    public int teleport = 0;

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
                gameObject.SetActive(currentLevel >= 6); //originally 6
                break;

            case StationType.Wire:
                gameObject.SetActive(currentLevel >= 11);
                break;
        }
    }
    public void Interact()
    {
        Order currentOrder = OrderManager.Instance.GetActiveOrder();
        

        if (currentOrder == null)
        {
            Debug.LogWarning("No current order found.");
            return;
        }

        if(RaycastInteractor.Instance.isOrderChecked == true)
        {
            
            if (GameLoopManager.Instance.dayNumber != null) GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
            if (GameLoopManager.Instance.moneyText != null) GameLoopManager.Instance.moneyText.gameObject.SetActive(false);
            if (GameLoopManager.Instance.tutorialButton != null) GameLoopManager.Instance.tutorialButton.gameObject.SetActive(false);
            if (GameLoopManager.Instance.remainingOrders != null) GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
            if (GameLoopManager.Instance.ordersOnboard != null) GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);
            if (GameLoopManager.Instance.moneyImage != null) GameLoopManager.Instance.moneyImage.gameObject.SetActive(false);
            GameLoopManager.Instance.shopButton.gameObject.SetActive(false);
            RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(false);
            RaycastInteractor.Instance.readyText.gameObject.SetActive(false);
            GameLoopManager.Instance.ShowTV(false);
            if (TimerScript.instance != null && TimerScript.instance.timer != null)
            {
                TimerScript.instance.timer.gameObject.SetActive(false); // hide
            }
            if (RaycastInteractor.Instance.ToolIndicator != null) RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(false);
            if (RaycastInteractor.Instance.WireIndicator != null) RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(false);
            if (RaycastInteractor.Instance.PaintIndicator != null) RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(false);
            


            switch (type)
            {
                case StationType.Tool:
                    if (currentOrder.needsTool && !currentOrder.toolDone)
                    {
                        LoadingScreenManager.Instance.SwitchtoScene(3);
                        //SceneManager.LoadScene("LO_Tool_GUI");
                        //currentOrder.toolDone = true;
                        Debug.Log("Tool station activated!");
                    }
                    break;

                case StationType.Paint:
                    if (currentOrder.needsPaint && !currentOrder.paintDone)
                    {
                        LoadingScreenManager.Instance.SwitchtoScene(2);
                        //SceneManager.LoadScene("LO_Paint_GUI");
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


        }
        

        OrderManager.Instance.TryCompleteOrder();
    }
}