using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

//using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Camera cam;

    [SerializeField] private int base_Lowest = 1;
    [SerializeField] private int base_Highest = 10;

    [SerializeField] private int diff_Lowest = 1;
    [SerializeField] private int diff_Highest = 5;

    private int generatedDifference;

    private List<int> currentPattern;

    public static GameLoopManager Instance;

    public static DataPersistenceManager dpm;

    //[SerializeField] private String fileName;

    public int level = 0;

    public int money = 0;

    public bool isPatternStarted;

    public TextMeshPro dayNumber;

    public TextMeshPro moneyText;

    public TextMeshPro remainingOrders;

    private TimerScript timer;

    private void Awake()
    {
        //Instance = this;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        OrderManager.Instance.TryCompleteOrder();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (cam != null)
        {
            cam.gameObject.SetActive(scene.name == "LO_WS2D"); //LO_Workshop
        }

        if (scene.name == "LO_WS2D")
        {
            // Re-find and assign the new instance of the text
            GameObject dayTextObject = GameObject.Find("Day_Number");
            if (dayTextObject != null)
            {
                dayNumber = dayTextObject.GetComponent<TextMeshPro>();
                if (dayNumber != null)
                {
                    dayNumber.text = "Day: " + level;
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on DayText object.");
                }
            }
            else
            {
                Debug.LogWarning("DayText object not found in LO_Workshop.");
            }

            GameObject moneyTextObject = GameObject.Find("MoneyText");
            if (moneyTextObject != null)
            {
                moneyText = moneyTextObject.GetComponent<TextMeshPro>();
                if (moneyText != null)
                {
                    moneyText.text = "Money: " + money;
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on MoneyText object.");
                }
            }
            else
            {
                Debug.LogWarning("MoneyText object not found in LO_Workshop.");
            }

            GameObject remainingOrderObj = GameObject.Find("RemainingOrders");
            if (remainingOrderObj != null)
            {
                remainingOrders = remainingOrderObj.GetComponent<TextMeshPro>();
                if (remainingOrders != null)
                {
                    UpdateRemainingOrders(); 
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on RemainingOrders object.");
                }
            }
            else
            {
                Debug.LogWarning("RemainingOrders object not found in LO_Workshop.");
            }

            if (TimerScript.instance != null)
            {
                if (OrderManager.Instance != null && OrderManager.Instance.GetCurrentOrder() != null)
                {
                    TimerScript.instance.StartTimer();
                }
                else
                {
                    Debug.Log("No active order. Timer not restarted.");
                }
            }

            if (level >= 1 && level < 5)
            {
                Debug.Log("Static Data for difficulty is easy!");
                StaticData.diffInt = 0; // Easy
            }
            else if (level >= 5 && level < 10)
            {
                Debug.Log("Static Data for difficulty is medium!");
                StaticData.diffInt = 1; // Medium
            }
            else if (level >= 10)
            {
                Debug.Log("Static Data for difficulty is hard!");
                StaticData.diffInt = 2; // Hard
            }

            if (isPatternStarted == false)
            {
                GenerateAndStorePattern();
                isPatternStarted = true;
            }
        }
    }
    public enum DifficultyLevel
    {
        tutorial,
        easy,
        medium,
        hard
    }

    public enum Minigame
    {
        tool,
        paint,
        wire
    }

    public void LoadData(GameData data)
    {
        this.level = data.level;
        this.money = data.money;
        this.isPatternStarted = data.isPatternStarted;

        StaticData.dayNo = this.level;

        if (dayNumber != null)
        {
            dayNumber.text = "Day: " + this.level;
        }

        if (moneyText != null)
        {
            moneyText.text = "Money: " + this.money;
        }
        Debug.Log("Level: " + level);
        Debug.Log("Money: " + money);
    }

    public void SaveData(ref GameData data)
    {
        data.level = this.level;
        data.money = this.money;
        data.isPatternStarted = this.isPatternStarted;
    }

    public void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money;
        }
    }

    public void UpdateRemainingOrders()
    {
        if (remainingOrders != null && OrderManager.Instance != null)
        {
            remainingOrders.text = "Remaining Orders: " + OrderManager.Instance.orderList.Count;
        }
    }

    private List<int> GeneratePatternArray(int patternLen)
    {
        generatedDifference = Random.Range(diff_Lowest, diff_Highest);
        StaticData.sequenceDiff = generatedDifference; // Store in StaticData for sequence difference
        int baseHolder = Random.Range(base_Lowest, base_Highest);

        List<int> numberPatternList = new List<int>();

        DifficultyLevel difficulty = DifficultyLevel.easy;

        switch (StaticData.diffInt)
        {
            case 0: difficulty = DifficultyLevel.easy; break;
            case 1: difficulty = DifficultyLevel.medium; break;
            case 2: difficulty = DifficultyLevel.hard; break;
        }

        

        if (difficulty == DifficultyLevel.easy || difficulty == DifficultyLevel.medium)
        {
            for (int i = 1; i <= patternLen; i++)
            {
                numberPatternList.Add(baseHolder + (generatedDifference * i));
            }
        }
        else if (difficulty == DifficultyLevel.hard)
        {
            for (int i = 1; i <= patternLen; i++)
            {
                baseHolder += (generatedDifference + i);
                numberPatternList.Add(baseHolder);
            }
        }

        return numberPatternList;
    }

    public List<int> GetPattern(int length)
    {
        return GeneratePatternArray(length);
    }

    private void ConfigureDifficulty(out int patternLength, out int incorrectVals, out int missingVals, out int noOfTypes, Minigame gameType)
    {
        DifficultyLevel level = DifficultyLevel.easy;

        switch (StaticData.diffInt)
        {
            case 0: level = DifficultyLevel.easy; break;
            case 1: level = DifficultyLevel.medium; break;
            case 2: level = DifficultyLevel.hard; break;
        }

        patternLength = 5;
        incorrectVals = 1;
        missingVals = 0;
        noOfTypes = 1;

        if (level == DifficultyLevel.easy)
        {
            missingVals = 0;
            noOfTypes = 1;

            if (gameType == Minigame.tool)
            {
                incorrectVals = Random.Range(1, 2);
                patternLength = Random.Range(5, 6);
            }
            else
            {
                incorrectVals = 1;
            }
        }
        else if (level == DifficultyLevel.medium)
        {
            noOfTypes = 2;

            if (gameType == Minigame.tool)
            {
                patternLength = Random.Range(5, 10);
                incorrectVals = Random.Range(2, 3);
                missingVals = Random.Range(1, 3);
            }
            else
            {
                incorrectVals = 1;
                missingVals = 1;
            }
        }
        else if (level == DifficultyLevel.hard)
        {
            noOfTypes = Random.Range(1, 3);
            if (gameType == Minigame.tool)
            {
                patternLength = 6;
                incorrectVals = 0;
                missingVals = 1;
            }
            else
            {
                patternLength = Random.Range(5, 10);
                incorrectVals = 0;
                missingVals = 2;
            }
        }

        if (gameType != Minigame.tool)
        {
            patternLength = 4;
        }

        StaticData.patternLength = patternLength;
        StaticData.incorrectVals = incorrectVals;
        StaticData.missingVals = missingVals;
        StaticData.noOfTypes = noOfTypes;
    }

    public void GenerateAndStorePattern()
    {

        ConfigureDifficulty(out int patternLength, out int incorrectVals, out int missingVals, out int noOfTypes, Minigame.tool);

        currentPattern = GeneratePatternArray(patternLength);

        Debug.Log("Pattern generated on game start: " + string.Join(", ", currentPattern));
        StaticData.toolPattern = currentPattern; // Store in StaticData for tool pattern
    }

    public void StartNewLevel()
    {
        OrderManager.Instance.SetStatus(false);
        level++;
        StaticData.dayNo = level;
        
        if(level >= 1 && level < 5)
        {
            Debug.Log("Static Data for difficulty is easy!");
            StaticData.diffInt = 0; // Easy
        }
        else if (level >= 5 && level < 10)
        {
            Debug.Log("Static Data for difficulty is medium!");
            StaticData.diffInt = 1; // Medium
        }
        else if (level >= 10)
        {
            Debug.Log("Static Data for difficulty is hard!");
            StaticData.diffInt = 2; // Hard
        }

        GenerateAndStorePattern();


        OrderManager.Instance.orderReceived = false; // Reset order received status
        //OrderManager.Instance.SetStatus(false);
        RaycastInteractor ri = Object.FindFirstObjectByType<RaycastInteractor>();
        dayNumber.text = "Day: " + this.level;
        moneyText.text = "Money: " + this.money;
        Debug.Log("Starting Level " + level);
        if (TimerScript.instance != null && TimerScript.instance.timer != null)
        {
            TimerScript.instance.timer.gameObject.SetActive(false); // hide
        }
        TimerScript.instance.ResetTimer();
    }
    
    public void CompleteLevel()
    {
        Debug.Log("Level " + level + " complete!");
        StartNewLevel();
    }

    /*
    public void OnTimerExpired()
    {
        Debug.Log("Time's up! Game Over or auto-submit.");
        // Call any fail screen or auto-fail logic here
        // Example: Load failure screen or retry?
    }
    */
}
