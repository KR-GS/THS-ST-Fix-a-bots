using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager
{
    private GameData gameData;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There already exists a data persistence manager.");
        }
        instance = this;
    }

    private void start()
    {
        loadGame();
    }

    private void OnApplicationQuit()
    {
        saveGame();
    }

    public void newGame()
    {
        //if new game, just create new game data object
        this.gameData = new GameData();
    }

    public void loadGame()
    {
        // load any saved data from a file using data handler

        // if no previous save data found, create new game
        if (this.gameData == null)
        {
            Debug.Log("No previous data found, starting new game.");
            newGame();
        }
        // push loaded data to all other scripts that need it
    }

    public void saveGame()
    {
        // pass the data to other scripts to update

        // save the data to a file using data handler
    }
}
