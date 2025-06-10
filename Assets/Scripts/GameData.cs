using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public int level;

    public List<Order> savedOrders;

    public int currentOrderIndex;

    public GameData()
    {
        this.level = 1;
        this.savedOrders = new List<Order>();
        this.currentOrderIndex = -1;
}


}