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

    public bool orderReceived;

    public int currentOrderIndex;

    public List<int> lives;

    public List<int> restarts;

    public List<float> stageTimes;

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
        this.lives = new List<int>();
        this.restarts = new List<int>();
        this.stageDone = 0;
        this.stageTimes = new List<float>();
    }


}