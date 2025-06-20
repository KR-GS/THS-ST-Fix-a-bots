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

    public int currentOrderIndex;

    public GameData()
    {
        this.level = 1;
        this.savedOrders = new List<Order>();
        this.currentOrderIndex = -1;
        this.money = 0;
        this.finished = false;
        this.prize = 0;
        this.time = 0f;
    }


}