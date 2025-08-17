using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    public GameObject TV;

    private List<int> currentPattern;

    private List<int> currentPaintPattern;

    public static GameLoopManager Instance;

    public static DataPersistenceManager dpm;

    //[SerializeField] private String fileName;

    public int level = 0;

    public int money = 0;

    public bool isPatternStarted;

    public TextMeshProUGUI dayNumber;

    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI remainingOrders;

    public TextMeshProUGUI ordersOnboard;

    private TimerScript timer;

    public Button nextdayButton;

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
            GameObject dayTextObject = GameObject.Find("DayNumber");
            if (dayTextObject != null)
            {
                dayNumber = dayTextObject.GetComponent<TextMeshProUGUI>();
                if (dayNumber != null)
                {
                    dayNumber.text = "Day: " + level;
                }
                else
                {
                    Debug.LogWarning("TextMeshProUGUI component not found on DayText object.");
                }
            }
            else
            {
                Debug.LogWarning("DayText object not found in LO_Workshop.");
            }

            GameObject moneyTextObject = GameObject.Find("Money_Text");
            if (moneyTextObject != null)
            {
                moneyText = moneyTextObject.GetComponent<TextMeshProUGUI>();
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
                remainingOrders = remainingOrderObj.GetComponent<TextMeshProUGUI>();
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

            GameObject ordersOnboardObj = GameObject.Find("OrdersOnboard");
            if (ordersOnboardObj != null)
            {
                ordersOnboard = ordersOnboardObj.GetComponent<TextMeshProUGUI>();
                if (ordersOnboard != null)
                {
                    UpdateOrdersOnboard();
                    StartCoroutine(UpdateOrdersOnboardPeriodically());
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on OrdersOnboard object.");
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

            StartCoroutine(UpdateStationsNextFrame());

        }
        else
        {
            if (dayNumber != null) dayNumber.gameObject.SetActive(false);
            if (moneyText != null) moneyText.gameObject.SetActive(false);
            if (remainingOrders != null) remainingOrders.gameObject.SetActive(false);
            if (ordersOnboard != null) ordersOnboard.gameObject.SetActive(false);
            ShowTV(false);
            if (RaycastInteractor.Instance.ToolIndicator != null) RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(false);
            if (RaycastInteractor.Instance.WireIndicator != null) RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(false);
            if (RaycastInteractor.Instance.PaintIndicator != null) RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(false);
        }

    }

    public void HandleSceneInitialization()
    {
        if (SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            Debug.Log("Post-load Init: IsPatternStarted value is..." + isPatternStarted);

            if (!isPatternStarted)
            {
                GenerateAndStorePattern();
                isPatternStarted = true;
                StaticData.isPatternStarted = true;

                DataPersistenceManager.Instance.SaveGame();

                Debug.Log("Pattern generated and flag set to true.");
            }

            Debug.Log($"Correct pattern: {string.Join(", ", StaticData.toolPattern ?? new List<int>())}");
            Debug.Log($"Incorrect pattern: {string.Join(", ", StaticData.incorrectToolPattern ?? new List<int>())}");
        }
    }

    private IEnumerator UpdateStationsNextFrame()
    {
        yield return null; // wait one frame
        foreach (Station station in Station.AllStations)
        {
            station.SetStationVisibility();
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
        StaticData.isPatternStarted = this.isPatternStarted;

        if (dayNumber != null)
        {
            dayNumber.text = "Day: " + this.level;
        }

        if (moneyText != null)
        {
            moneyText.text = "Money: " + this.money;
        }
        Debug.Log("Level: " + level);

        if (level >= 1 && level < 6)
        {
            Debug.Log("Static Data for difficulty is easy!");
            StaticData.diffInt = 0; // Easy
        }
        else if (level >= 6 && level < 11)
        {
            Debug.Log("Static Data for difficulty is medium!");
            StaticData.diffInt = 1; // Medium
        }
        else if (level >= 11)
        {
            Debug.Log("Static Data for difficulty is hard!");
            StaticData.diffInt = 2; // Hard
        }

        Debug.Log("Money: " + money);

        StaticData.patternLength = data.patternLength;
        StaticData.selectedFastenerIndex = data.selectedFastenerIndex;
        StaticData.selectedStickerIndex = data.selectedStickerIndex;
        StaticData.selectedStickerIndexTwo = data.selectedStickerIndexTwo;
        if (data.correctPattern != null)
        {
            currentPattern = new List<int>(data.correctPattern);
            StaticData.toolPattern = new List<int>(data.correctPattern);
        }
        if(data.paintPattern != null)
        {
            currentPaintPattern = new List<int>(data.paintPattern);
            StaticData.paintPattern = new List<int>(data.paintPattern);
        }
        if (data.incorrectPattern != null)
        {
            StaticData.incorrectToolPattern = new List<int>(data.incorrectPattern);
        }
        if (data.incorrectIndices != null)
        {
            StaticData.incorrectIndices = new List<int>(data.incorrectIndices);
        }
        if (data.incorrectValues != null)
        {
            StaticData.incorrectValues = new List<int>(data.incorrectValues);
        }
        
        

        HandleSceneInitialization();
    }

    public void SaveData(ref GameData data)
    {
        data.level = this.level;
        data.money = this.money;
        data.isPatternStarted = this.isPatternStarted;
        data.correctPattern = new List<int>(currentPattern); // Store the current pattern
        data.incorrectPattern = new List<int>(StaticData.incorrectToolPattern); // Store the incorrect pattern
        data.incorrectIndices = new List<int>(StaticData.incorrectIndices); // Store incorrect indices
        data.paintPattern = new List<int>(currentPaintPattern); // Store the current paint pattern

        data.incorrectValues = new List<int>(StaticData.incorrectValues); // Store incorrect values
        data.selectedFastenerIndex = StaticData.selectedFastenerIndex;
        data.selectedStickerIndex = StaticData.selectedStickerIndex;
        data.selectedStickerIndexTwo = StaticData.selectedStickerIndexTwo;
        data.patternLength = StaticData.patternLength;
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

    private IEnumerator UpdateOrdersOnboardPeriodically()
    {
        while (true)
        {
            UpdateOrdersOnboard();
            yield return new WaitForSeconds(1f); // update every 1 second
        }
    }

    public void UpdateOrdersOnboard()
    {
        if (ordersOnboard != null && OrderManager.Instance != null)
        {
            ordersOnboard.text = "Pending Orders: " + OrderManager.Instance.activeOrders.Count;
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

    private List<int> GeneratePaintPatternArray(int patternLen)
    {
        generatedDifference = Random.Range(diff_Lowest, diff_Highest);
        StaticData.sequenceDiff = generatedDifference; // Store in StaticData for sequence difference
        int baseHolder = Random.Range(base_Lowest, base_Highest);

        List<int> numberPaintPatternList = new List<int>();
        
        for (int i = 1; i <= patternLen; i++)
        {
            numberPaintPatternList.Add(baseHolder + (generatedDifference * i));
        }
        

        return numberPaintPatternList;
    }

    public List<int> GetPattern(int length)
    {
        return GeneratePatternArray(length);
    }

    public List<int> GetPaintPattern(int length)
    {
        return GeneratePaintPatternArray(length);
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

        
        StaticData.patternLength = patternLength;
        StaticData.incorrectVals = incorrectVals;
        StaticData.missingVals = missingVals;
        StaticData.noOfTypes = noOfTypes;
    }

    public void GenerateAndStorePattern()
    {

        ConfigureDifficulty(out int patternLength, out int incorrectVals, out int missingVals, out int noOfTypes, Minigame.tool);
        currentPattern = GeneratePatternArray(patternLength);
        StaticData.toolPattern = currentPattern;

        // Paint generation
        currentPaintPattern = GeneratePaintPatternArray(6);
        StaticData.paintPattern = currentPaintPattern;

        Debug.Log("Pattern generated on game start: " + string.Join(", ", currentPattern));
        StaticData.toolPattern = currentPattern; // Store in StaticData for tool pattern
        StaticData.paintPattern = currentPaintPattern;

        if (StaticData.incorrectIndices != null && StaticData.incorrectValues != null)
        {
            for (int i = 0; i < StaticData.incorrectValues.Count; i++)
            {
                int idx = StaticData.incorrectIndices[i];
                if (idx >= currentPattern.Count) // Prevent out-of-range crash
                {
                    StaticData.incorrectIndices = null;
                    StaticData.incorrectValues = null;
                    break;
                }

                int original = currentPattern[idx];
                int loaded = StaticData.incorrectValues[i];
                if (loaded >= original)
                {
                    Debug.LogWarning("Old incorrect pattern is invalid — regenerating.");
                    StaticData.incorrectIndices = null;
                    StaticData.incorrectValues = null;
                    break;
                }
            }
        }

        if (StaticData.incorrectIndices != null && StaticData.incorrectIndices.Count > 0 &&
        StaticData.incorrectValues != null && StaticData.incorrectValues.Count == StaticData.incorrectIndices.Count)
        {
            // Already generated; use stored values
            List<int> loadedPattern = new List<int>(currentPattern);
            for (int i = 0; i < StaticData.incorrectIndices.Count; i++)
            {
                int idx = StaticData.incorrectIndices[i];
                loadedPattern[idx] = StaticData.incorrectValues[i];
            }
            StaticData.incorrectToolPattern = loadedPattern;
            Debug.Log("Loaded incorrect pattern from StaticData: " + string.Join(", ", loadedPattern));
        }
        else
        {
            // First time: generate incorrect pattern
            List<int> incorrectPattern = new List<int>(currentPattern);
            StaticData.incorrectIndices = new List<int>();
            StaticData.incorrectValues = new List<int>();

            HashSet<int> changedIndices = new HashSet<int>();
            while (changedIndices.Count < incorrectVals)
            {
                int randIndex = Random.Range(0, incorrectPattern.Count);
                if (!changedIndices.Contains(randIndex))
                {
                    int original = incorrectPattern[randIndex];
                    int newVal;
                    do
                    {
                        newVal = original + Random.Range(-4, -1);
                        //newVal = original + Random.Range(-3, 4); //values can either be up or down of the value
                    } while (newVal == original || newVal < 0);

                    incorrectPattern[randIndex] = newVal;
                    changedIndices.Add(randIndex);

                    // Store incorrect index and value
                    StaticData.incorrectIndices.Add(randIndex);
                    StaticData.incorrectValues.Add(newVal);
                }
            }

            StaticData.incorrectToolPattern = incorrectPattern;
            Debug.Log("Incorrect pattern saved: " + string.Join(", ", incorrectPattern));
        }

        StaticData.selectedFastenerIndex = Random.Range(0, 3); //based on LoToolMinigame, array size is 4
        StaticData.selectedStickerIndex = Random.Range(0, 3);
        StaticData.selectedStickerIndex = Random.Range(0, 3);
        if (StaticData.selectedStickerIndex == StaticData.selectedFastenerIndex) // Ensure different indices
        {
            StaticData.selectedStickerIndex = (StaticData.selectedStickerIndex + 1) % 3; // Wrap around if same index
        }
    }

    public void ShowTV(bool response)
    {
        if (response == false)
        {
            if (TV != null)
            {
                TV.SetActive(false);
            }
        }
        else if (response == true)
        {
            if (TV != null)
            {
                TV.SetActive(true);
            }
        }
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
        foreach (Station station in Station.AllStations)
        {
            station.SetStationVisibility();
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
