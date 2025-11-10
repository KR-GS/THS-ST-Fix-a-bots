using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StationExit : MonoBehaviour
{

    public enum StationType { Tool, Paint, Wire }
    public StationType type;
    public bool debugTool = false;

    public bool debugPaint = false;

    public bool debugWire = false;

    public int howManyWrongs = 0;

    public List<int> correctPattern = new List<int>();
    public List<int> playerAnswer = new List<int>();
    public List<int> paint2Patterning = new List<int>();
    public List<int> answer2Painting = new List<int>();
    public int level = 0;

    [SerializeField] StationTimeManager getTime;

    public void ExitStation(){
        Order currentOrder = StaticData.currentOrder;
        if (currentOrder != null)
        {
            Debug.Log("Yeah... you did have a current order!");
            StaticData.cutscenePlay = false;

            switch (type)
            {
                case StationType.Tool:
                    if (StaticData.isToolDone)
                    {
                        Debug.Log("Tool task completed, marking toolDone = true");
                        StaticData.timeSpent = getTime.GetStationTime();
                        Debug.Log("You spent this much time here: " + StaticData.timeSpent);
                        StaticData.pendingToolWrongs = StaticData.toolWrong;
                    }
                    else
                    {
                        Debug.LogWarning("Tool task not completed yet!");
                        StaticData.timeSpent = getTime.GetStationTime();
                        Debug.Log("You spent this much time here: " + StaticData.timeSpent);
                        StaticData.pendingToolWrongs = StaticData.toolWrong;

                    }
                    StaticData.enteredStation = 0;
                    break;
                case StationType.Paint:
                    if (StaticData.isPaintDone)
                    {
                        Debug.Log("Paint task completed, marking paintDone = true");
                        StaticData.timeSpent = getTime.GetStationTime();
                        Debug.Log("You spent this much time here: " + StaticData.timeSpent);
                        StaticData.pendingPaintWrongs = StaticData.paintWrong;
                    }
                    else
                    {
                        Debug.LogWarning("Paint task not completed yet!");
                        StaticData.timeSpent = getTime.GetStationTime();
                        Debug.Log("You spent this much time here: " + StaticData.timeSpent);
                        StaticData.pendingPaintWrongs = StaticData.paintWrong;

                    }
                    StaticData.enteredStation = 1;
                    break;
                case StationType.Wire:
                    if(StaticData.isWireDone)
                    {  
                        Debug.Log("Wire task completed, marking wireDone = true");
                        StaticData.timeSpent = getTime.GetStationTime();
                        Debug.Log("You spent this much time here: " + StaticData.timeSpent);
                        StaticData.pendingWireWrongs = StaticData.wireWrong;
                    }
                    else
                    {
                        Debug.LogWarning("Wire task not completed yet!");
                        StaticData.timeSpent = getTime.GetStationTime();
                        Debug.Log("You spent this much time here: " + StaticData.timeSpent);
                        StaticData.pendingWireWrongs = StaticData.wireWrong;
                    }
                    StaticData.enteredStation = 2;
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

        switch (StaticData.enteredStation)
        {
            case 0:
                correctPattern = new List<int>(StaticData.toolPattern ?? new List<int>());
                playerAnswer = new List<int>(StaticData.playerToolPattern ?? new List<int>());
                howManyWrongs = StaticData.pendingToolWrongs;
                break;

            case 1:
                correctPattern = new List<int>(StaticData.paintPattern ?? new List<int>());
                playerAnswer = new List<int>(StaticData.playerPaintPattern ?? new List<int>());
                paint2Patterning = new List<int>(StaticData.paint2Pattern ?? new List<int>());
                answer2Painting = new List<int>(StaticData.playerPaint2Pattern ?? new List<int>());
                howManyWrongs = StaticData.pendingPaintWrongs;
                break;

            case 2:
                correctPattern = new List<int>(StaticData.wirePattern ?? new List<int>());
                playerAnswer = new List<int>(StaticData.playerWirePattern ?? new List<int>());
                howManyWrongs = StaticData.pendingWireWrongs;
                break;

            default:
                Debug.LogWarning("Where have you been? There's no soldering station!");
                return;
        }

        StaticData.pendingGameRecord = new GameData.GameRecord(
            correctPattern,
            playerAnswer,
            paint2Patterning,
            answer2Painting,
            StaticData.timeSpent,
            StaticData.dayNo,
            howManyWrongs, // Capture NOW
            StaticData.enteredStation,
            StaticData.orderNumber
        );

        Debug.Log("Saving the time you spent with this amount: " + StaticData.timeSpent.ToString() + " seconds.");
        DataPersistenceManager.Instance.SaveGame();
        Debug.Log("What about after: " + StaticData.timeSpent.ToString() + " seconds.");
        Invoke(nameof(DelayedSceneSwitch), 0.3f); 
    }

    private void DelayedSceneSwitch()
    {
        LoadingScreenManager.Instance.SwitchtoSceneGear(7);
    }
}
