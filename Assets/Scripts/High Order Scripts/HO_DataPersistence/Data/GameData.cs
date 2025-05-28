using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//Put here everyhing that needs to be saved 
public class GameData
{
    public int winCount;

    //when a new game is started, this will be the initial values
    public GameData()
    {
        this.winCount = 0;
    }
}
