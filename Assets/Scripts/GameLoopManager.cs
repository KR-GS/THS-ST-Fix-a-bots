using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;

public class GameLoopManager : MonoBehaviour, IDataPersistence
{
    public static GameLoopManager Instance;

    public static DataPersistenceManager dpm;

    //[SerializeField] private String fileName;

    public int level = 0;

    public TextMeshPro dayNumber;

    private void Awake()
    {
        Instance = this;

    }
    


    public void LoadData(GameData data)
    {
        this.level = data.level;
        dayNumber.text = "Day: " + this.level;
        Debug.Log("Level: " + level);
    }

    public void SaveData(ref GameData data)
    {
        data.level = this.level;
    }

    public void StartNewLevel()
    {
        level++;
        dayNumber.text = "Day: " + level;
        Debug.Log("Starting Level " + level);
    }

    public void CompleteLevel()
    {
        Debug.Log("Level " + level + " complete!");
        StartNewLevel();
    }
}
