using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private String fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager Instance { get; private set; }
    private bool hasLoadedFromFile = false;

    public void Awake()
    {
        
        /*if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }*/

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        dataPersistenceObjects = new List<IDataPersistence>();
        Debug.Log("Awake has been called, instance set");
    }
    private void OnEnable()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("DataPersistenceManager subscribed to sceneLoaded event");
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"===== SCENE LOADED: {scene.name} =====");

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        Debug.Log($"Found {dataPersistenceObjects.Count} IDataPersistence objects");

        LoadGame();
    }

    public void Start()
    {
        //this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        //this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (!hasLoadedFromFile)
        {
            LoadGame();
            hasLoadedFromFile = true;
        }
        
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>
            (FindObjectsSortMode.None).OfType<IDataPersistence>();

        Debug.Log($"FindAllDataPersistenceObjects found: {dataPersistenceObjects.Count()} objects");

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void RegisterDataPersistence(IDataPersistence dataPersistence)
    {
        if (!dataPersistenceObjects.Contains(dataPersistence))
        {
            dataPersistenceObjects.Add(dataPersistence);
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        Debug.Log("===== DPM LoadGame Called! =====");

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No save file has been recorded! Creating new save file.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            Debug.Log($"Calling LoadData on: {dataPersistenceObj.GetType().Name}");
            dataPersistenceObj.LoadData(gameData);
        }

        hasLoadedFromFile = true;
        Debug.Log("Loaded day number: " + gameData.level);

    }

    public int GetLevel()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No save file has been recorded! Creating new save file.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        return gameData.level;
    }

    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.Log("No new data found! A new game has to be started to save data");
            return;
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved day number: " + gameData.level + "at: " + Application.persistentDataPath);

        dataHandler.Save(gameData);
    }


    public float GetTimeLft()
    {
        return dataHandler.Load().time;
    }
    public void OnApplicationQuit()
    {
        SaveGame();
    }
}
