using UnityEngine;
using UnityEngine.SceneManagement;

public class StationExit : MonoBehaviour
{

    public enum StationType { Tool, Paint, Wire }
    public StationType type;
    public bool debugTool = false;

    public bool debugPaint = false;

    public bool debugWire = false;

    public void ExitStation(){
        Order currentOrder = StaticData.currentOrder;
        if (currentOrder != null)
        {
            Debug.Log("Yeah... you did have a current order!");
            switch (type)
            {
                case StationType.Tool:
                    if (StaticData.isToolDone)
                    {
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
                        Debug.Log("Paint task completed, marking paintDone = true");
                    }
                    else
                    {
                        Debug.LogWarning("Paint task not completed yet!");
                       
                    }
                    break;
                case StationType.Wire:
                    if(StaticData.isWireDone)
                    {  
                        Debug.Log("Wire task completed, marking wireDone = true");
                    }
                    else
                    {
                        Debug.LogWarning("Wire task not completed yet!");
                    }
                    break;
            }

            Debug.Log($"Successfully returned from {type} station!");

            if (StaticData.startOfDay == true)
            {
                Debug.Log("It is the start of day indeed!");
            }
            else
            {
                Debug.Log("You returned from work thinking it's already a new day?"); 
            }

        }
        else
        {
            Debug.Log("LOL, IT ARRIVED HERE???");
        }

        if (StaticData.isToolDone == true)
        {
            debugTool = true;
            StaticData.debugTool = true;
        }
        else
        {
            debugTool = false;
            StaticData.debugTool = false;
        }

        StaticData.isToolDone = debugTool;

        if (StaticData.isPaintDone == true)
        {
            debugPaint = true;
            StaticData.debugPaint = true;
        }
        else
        {
            debugPaint = false;
            StaticData.debugPaint = false;
        }

        StaticData.isPaintDone = debugPaint;

        if (StaticData.isWireDone == true)
        {
            debugWire = true;
            StaticData.debugWire = true;
        }
        else
        {
            debugWire = false;
            StaticData.debugWire = false;
        }

        StaticData.isWireDone = debugWire;


        DataPersistenceManager.Instance.SaveGame();
        Debug.Log("Tool static data = " + StaticData.isToolDone);
        SceneManager.LoadScene("LO_WS2D");
    }
}
