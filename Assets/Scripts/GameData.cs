using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public int level;

    public int money;

    public int prize;

    public float time;

    public List<Order> savedOrders;

    public bool finished;

    public bool isPatternStarted;

    public bool orderReceived;

    public int currentOrderIndex;

    public List<int> lives;

    public List<int> restarts;

    public List<float> stageTimes;

    public List<Order> savedActiveOrders;

    public List<Order> pendingOrdersList;

    public List<int> correctPattern;

    public List<int> incorrectPattern;

    public List<int> paintPattern;

    public List<int> incorrectPaintPattern;

    public List<int> incorrectIndices;

    public List<int> incorrectValues;

    public int selectedFastenerIndex;

    public int selectedStickerIndex;

    public int selectedStickerIndexTwo;

    public int stageDone;

    public int patternLength;

    public bool isToolDone;

    public bool isPaintDone;

    public bool isWireDone;

    public bool isOrderChecked;

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
        this.lives = new List<int>();
        this.restarts = new List<int>();
        this.stageDone = 0;
        this.stageTimes = new List<float>();
        this.isPatternStarted = false;
        this.correctPattern = new List<int>();
        this.incorrectPattern = new List<int>();
        this.selectedFastenerIndex = 0;
        this.selectedStickerIndex = 0;
        this.selectedStickerIndexTwo = 0;
        this.incorrectIndices = new List<int>();
        this.incorrectValues = new List<int>();
        this.patternLength = 5;
        this.isPaintDone = false;
        this.isWireDone = false;
        this.isToolDone = false;
        this.isOrderChecked = false;
    }


}