using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]

public class GameData
{
    [Header("Save Settings")]
    public string language = "English";

    public float stageSpeed = 1f;


    [Header("Lower Order Data")]
    public int level;

    public int money;

    public int prize;

    public float time;

    public List<Order> savedOrders;

    public bool finished;

    public bool isPatternStarted;

    public bool orderReceived;

    public int currentOrderIndex;

    public List<Order> savedActiveOrders;

    public List<Order> pendingOrdersList;

    public List<int> correctPattern;

    public List<int> incorrectPattern;

    public List<int> paintPattern;

    public List<int> incorrectPaintPattern;

    public List<int> wirePattern;

    public List<int> incorrectIndices;

    public List<int> incorrectValues;

    public int selectedFastenerIndex;

    public int selectedStickerIndex;

    public int selectedStickerIndexTwo;

    public int toolpatternLength;

    public int paintpatternLength;

    public int wirepatternLength;

    public bool isToolDone;

    public bool isPaintDone;

    public bool isWireDone;

    public int valuestoChange;

    public bool isOrderChecked;

    public bool sendNewOrder;

    public int toolScore;

    public int paintScore;

    public int wireScore;

    public bool paintHover;

    public bool wireHover;

    public bool toolHover;

    public bool didExit;

    public bool isSchedulerRunning;

    public int medValue;

    public int toolWrong;

    public int paintWrong;

    public int wireWrong;

    public bool startOfDay;

    [Header("High Order Data")]
    public List<int> lives;

    public List<int> restarts;

    public List<float> stageTimes;

    public List<string> formulaAttempts;

    public List<int> stageStars;

    public int stageDone;

    public GameData()
    {
        this.level = 1;
        this.savedOrders = new List<Order>();
        this.currentOrderIndex = -1;
        this.money = 0;
        this.finished = false;
        this.orderReceived = false;
        this.prize = 0;
        this.time = 0f;

        //Higher Order
        int stageCount = 45;
        this.lives = new List<int>(new int[stageCount]);
        this.restarts = new List<int>(new int[stageCount]);
        this.stageTimes = new List<float>(new float[stageCount]);
        this.formulaAttempts = new List<string>(new string[stageCount]);
        this.stageStars = new List<int>(new int[stageCount]);
        this.stageDone = 0;
        this.stageSpeed = 1f;

        //Lower Order
        this.isPatternStarted = false;
        this.correctPattern = new List<int>();
        this.incorrectPattern = new List<int>();
        this.selectedFastenerIndex = 0;
        this.selectedStickerIndex = 0;
        this.selectedStickerIndexTwo = 0;
        this.incorrectIndices = new List<int>();
        this.incorrectValues = new List<int>();
        this.toolpatternLength = 5;
        this.paintpatternLength = 5;
        this.isPaintDone = false;
        this.isWireDone = false;
        this.isToolDone = false;
        this.isOrderChecked = false;
        this.sendNewOrder = false;
        this.toolScore = 0;
        this.paintScore = 0;
        this.wireScore = 0;
        this.medValue = 0;
        this.startOfDay = true;
        this.toolWrong = 0;
        this.paintWrong = 0;
        this.wireWrong = 0;
        this.isSchedulerRunning = false;
        this.valuestoChange = 0;
        this.didExit = true;
    }


}