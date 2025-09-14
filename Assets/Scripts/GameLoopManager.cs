using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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

    public int medValue = 0;

    public int money = 0;

    public bool isPatternStarted;

    public TextMeshProUGUI dayNumber;

    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI remainingOrders;

    public TextMeshProUGUI ordersOnboard;

    private TimerScript timer;

    public Image moneyImage;

    public int toolScore;

    public int paintScore;

    public int wireScore;

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
            if(StaticData.cutscenePlay == true)
            {
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
                        moneyText.text = money.ToString();
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

                Debug.Log("[LOOK HERE] Pending orders count: " + OrderManager.Instance.pendingOrders.Count);

                if (StaticData.startOfDay == true)
                {
                    Debug.Log("I went to true cuz it is...!");
                    Debug.Log("It is the start of day indeed!");
                    RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(true);
                    RaycastInteractor.Instance.readyText.gameObject.SetActive(true);
                }
                else if (StaticData.startOfDay == false)
                {
                    Debug.Log("I just went here cuz it ain't! Ain't what?");
                    Debug.Log("No it ain't the start of the day!");
                    RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(false);
                    RaycastInteractor.Instance.readyText.gameObject.SetActive(false);
                }

                StartCoroutine(UpdateStationsNextFrame());
            }

        }
        else
        {
            if (dayNumber != null) dayNumber.gameObject.SetActive(false);
            if (moneyText != null) moneyText.gameObject.SetActive(false);
            if (remainingOrders != null) remainingOrders.gameObject.SetActive(false);
            if (ordersOnboard != null) ordersOnboard.gameObject.SetActive(false);
            if (moneyImage != null) moneyImage.gameObject.SetActive(false);
            RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(false);
            RaycastInteractor.Instance.readyText.gameObject.SetActive(false);
            ShowTV(false);
            if (TimerScript.instance != null && TimerScript.instance.timer != null)
            {
                TimerScript.instance.timer.gameObject.SetActive(false); // hide
            }
            if (RaycastInteractor.Instance.ToolIndicator != null) RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(false);
            if (RaycastInteractor.Instance.WireIndicator != null) RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(false);
            if (RaycastInteractor.Instance.PaintIndicator != null) RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(false);

        }

    }

   
    public void HandleSceneInitialization()
    {
        if (SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            Debug.Log("Post-load Init: IsPatternStarted value is..." + StaticData.isPatternStarted);

            if (!StaticData.isPatternStarted)
            {
                GenerateAndStorePattern();
                //isPatternStarted = true;
                StaticData.isPatternStarted = true;
                DataPersistenceManager.Instance.SaveGame();
                Debug.Log("Pattern generated and flag set to true.");
            }
            else
            {
                Debug.Log("Pattern already exists, not regenerating.");
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
        //this.isPatternStarted = data.isPatternStarted;
        StaticData.isPatternStarted = data.isPatternStarted;
        this.toolScore = data.toolScore;
        this.paintScore = data.paintScore;
        this.wireScore = data.wireScore;

        StaticData.cutscenePlay = data.cutscenePlay;

        StaticData.startOfDay = data.startOfDay;

        StaticData.medValue = data.medValue;
        StaticData.dayNo = this.level;
        

        if (dayNumber != null)
        {
            dayNumber.text = "Day: " + this.level;
        }

        if (moneyText != null)
        {
            moneyText.text = this.money.ToString();
        }
        Debug.Log("Level: " + level);

        if (toolScore >= 0 && toolScore < 200)
        {
            StaticData.toolDifficulty = 0; // Easy
            Debug.Log("Static Data for tool difficulty is easy!");
        }
        else if (toolScore >= 200 && toolScore < 500)
        {
            StaticData.toolDifficulty = 1; // Medium
            Debug.Log("Static Data for tool difficulty is medium!");
        }
        else if (toolScore >= 500)
        {
            StaticData.toolDifficulty = 2; // Hard
            Debug.Log("Static Data for tool difficulty is hard!");
        }

        if (paintScore >= 0 && paintScore < 200)
        {
            StaticData.paintDifficulty = 0; // Easy
            Debug.Log("Static Data for paint difficulty is easy!");
        }
        else if (paintScore >= 200 && paintScore < 500)
        {
            StaticData.paintDifficulty = 1; // Medium
            Debug.Log("Static Data for paint difficulty is medium!");
        }
        else if (paintScore >= 500)
        {
            StaticData.paintDifficulty = 2; // Hard
            Debug.Log("Static Data for paint difficulty is hard!");
        }

        if (wireScore >= 0 && wireScore < 250) //We do not have wire yet
        {
            StaticData.wireDifficulty = 0; // Easy
            Debug.Log("Static Data for wire difficulty is easy!");
        }
        else if (wireScore >= 250 && wireScore < 500)
        {
            StaticData.wireDifficulty = 1; // Medium
            Debug.Log("Static Data for wire difficulty is medium!");
        }
        else if (wireScore >= 500)
        {
            StaticData.wireDifficulty = 2; // Hard
            Debug.Log("Static Data for wire difficulty is hard!");
        }


        Debug.Log("Money: " + money);

        StaticData.toolWrong = data.toolWrong;
        StaticData.paintWrong = data.paintWrong;
        StaticData.wireWrong = data.wireWrong;

        Debug.Log("Tool wrongs: " + StaticData.toolWrong + ", Paint wrongs: " + StaticData.paintWrong + ", Wire wrongs: " + StaticData.wireWrong);


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
        data.medValue = StaticData.medValue;
        data.isPatternStarted = StaticData.isPatternStarted;
        data.correctPattern = new List<int>(currentPattern); // Store the current pattern
        data.incorrectPattern = new List<int>(StaticData.incorrectToolPattern); // Store the incorrect pattern
        data.incorrectIndices = new List<int>(StaticData.incorrectIndices); // Store incorrect indices
        data.paintPattern = new List<int>(currentPaintPattern); // Store the current paint pattern
        data.paintScore = this.paintScore;
        data.toolScore = this.toolScore;
        data.wireScore = this.wireScore;
        data.incorrectValues = new List<int>(StaticData.incorrectValues); // Store incorrect values
        data.selectedFastenerIndex = StaticData.selectedFastenerIndex;
        data.selectedStickerIndex = StaticData.selectedStickerIndex;
        data.selectedStickerIndexTwo = StaticData.selectedStickerIndexTwo;
        data.patternLength = StaticData.patternLength;
        data.startOfDay = StaticData.startOfDay;
        data.toolWrong = StaticData.toolWrong;
        data.paintWrong = StaticData.paintWrong;
        data.wireWrong = StaticData.wireWrong;
        data.cutscenePlay = StaticData.cutscenePlay;
    }

    public void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString();
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
    private List<int> GeneratePatternArray(int patternLen) //This is for tool
    {
        generatedDifference = Random.Range(diff_Lowest, diff_Highest);
        StaticData.sequenceDiff = generatedDifference; // Store in StaticData for sequence difference
        int baseHolder = Random.Range(base_Lowest, base_Highest);

        List<int> numberPatternList = new List<int>();

        DifficultyLevel difficulty = DifficultyLevel.easy;

        switch (StaticData.paintDifficulty)
        {
            case 0: difficulty = DifficultyLevel.easy; break;
            case 1: difficulty = DifficultyLevel.medium; break;
            case 2: difficulty = DifficultyLevel.hard; break;
        }

        

        if (difficulty == DifficultyLevel.easy)
        {
            for (int i = 1; i <= patternLen; i++)
            {
                numberPatternList.Add(baseHolder + (generatedDifference * i));
            }
        }
        else if (difficulty == DifficultyLevel.medium)
        {
            if (StaticData.medValue <= 5){
                for (int i = 1; i <= patternLen; i++)
                {
                    numberPatternList.Add(baseHolder + (generatedDifference * i));
                }
            }
            else
            {
                for (int i = 1; i <= patternLen; i++)
                {
                    baseHolder += (generatedDifference + i);
                    numberPatternList.Add(baseHolder);
                }
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

        switch (gameType)
        {
            case Minigame.tool:
                switch (StaticData.toolDifficulty)
                {
                    case 0: level = DifficultyLevel.easy; break;
                    case 1: level = DifficultyLevel.medium; break;
                    case 2: level = DifficultyLevel.hard; break;
                }
                break;

            case Minigame.paint:
                switch (StaticData.paintDifficulty)
                {
                    case 0: level = DifficultyLevel.easy; break;
                    case 1: level = DifficultyLevel.medium; break;
                    case 2: level = DifficultyLevel.hard; break;
                }
                break;

            case Minigame.wire:
                switch (StaticData.wireDifficulty)
                {
                    case 0: level = DifficultyLevel.easy; break;
                    case 1: level = DifficultyLevel.medium; break;
                    case 2: level = DifficultyLevel.hard; break;
                }
                break;
        }

        patternLength = 6;
        incorrectVals = 1;
        missingVals = 0;
        noOfTypes = 1;

        if (level == DifficultyLevel.easy)
        {
            missingVals = 0;
            noOfTypes = 1;
            Debug.Log("Ezpz wins the race");

            if (gameType == Minigame.tool)
            {
                patternLength = 6;
                incorrectVals = Random.Range(1, 2);
            }
            else
            {
                incorrectVals = 1;
            }
        }
        else if (level == DifficultyLevel.medium)
        {
            noOfTypes = 2;
            StaticData.medValue = Random.Range(1, 10);
            Debug.Log("Medium with value: " + StaticData.medValue);
            if (gameType == Minigame.tool)
            {
                patternLength = 6;
                incorrectVals = Random.Range(2, 3);
                missingVals = Random.Range(1, 2);
                Debug.Log("Hey look, I went here! Incorrect vals: " + incorrectVals + ", Missing Vals: " + missingVals);
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
            Debug.Log("Hardcore indeed!");
            if (gameType == Minigame.tool)
            {
                patternLength = 6;
                incorrectVals = 0;
                missingVals = 1;
            }
            else
            {
                patternLength = Random.Range(5, 6);
                incorrectVals = 0;
                missingVals = 2;
            }
        }

        if (gameType != Minigame.tool)
        {
            patternLength = 6;
        }

        StaticData.patternLength = patternLength;
        StaticData.incorrectVals = incorrectVals;
        StaticData.missingVals = missingVals;
        StaticData.noOfTypes = noOfTypes;
        Debug.Log("Final values: " + StaticData.incorrectVals + ", Missing Vals: " + StaticData.missingVals);

    }

    public void GenerateAndStorePattern()
    {
        Debug.Log("Tooly tooly");
        ConfigureDifficulty(out int patternLength, out int incorrectVals, out int missingVals, out int noOfTypes, Minigame.tool);
        currentPattern = GeneratePatternArray(patternLength);
        StaticData.toolPattern = currentPattern;

       

        Debug.Log("Pattern generated on game start: " + string.Join(", ", currentPattern));
        StaticData.toolPattern = currentPattern; // Store in StaticData for tool pattern
        StaticData.paintPattern = currentPaintPattern;

        if (StaticData.toolDifficulty == 0 || (StaticData.toolDifficulty == 1 && StaticData.medValue <= 5))
        {
           
            Debug.Log($"Starting incorrect pattern generation. incorrectVals: {StaticData.incorrectVals}");
            Debug.Log($"Current pattern: " + string.Join(", ", currentPattern));

            bool isValidIncorrect = false;
       
            if (StaticData.incorrectIndices != null && StaticData.incorrectValues != null)
            {
                Debug.Log($"Existing data found - indices: {StaticData.incorrectIndices.Count}, values: {StaticData.incorrectValues.Count}");
                if (StaticData.incorrectIndices.Count == StaticData.incorrectValues.Count &&
                    StaticData.incorrectIndices.Count == StaticData.incorrectVals) 
                {
                    isValidIncorrect = true;
                    for (int i = 0; i < StaticData.incorrectValues.Count; i++)
                    {
                        int idx = StaticData.incorrectIndices[i];
                        if (idx < 0 || idx >= currentPattern.Count)
                        {
                            Debug.Log($"Invalid index {idx} at position {i}");
                            isValidIncorrect = false;
                            break;
                        }
                        int original = currentPattern[idx];
                        int loaded = StaticData.incorrectValues[i];
                        // Must be different and non-negative
                        if (loaded == original || loaded < 0)
                        {
                            Debug.Log($"Invalid value {loaded} for index {idx} (original: {original})");
                            isValidIncorrect = false;
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("Count mismatch - regenerating");
                }
            }
            else
            {
                Debug.Log("No existing data found - generating new");
            }

            if (isValidIncorrect)
            {
    
                List<int> loadedPattern = new List<int>(currentPattern);
                for (int i = 0; i < StaticData.incorrectIndices.Count; i++)
                {
                    loadedPattern[StaticData.incorrectIndices[i]] = StaticData.incorrectValues[i];
                }
                StaticData.incorrectToolPattern = loadedPattern;
                Debug.Log("Loaded incorrect pattern from StaticData: " + string.Join(", ", loadedPattern));
            }
            else
            {
                // Generate a new incorrect pattern
                List<int> incorrectPattern = new List<int>(currentPattern);
                StaticData.incorrectIndices = new List<int>();
                StaticData.incorrectValues = new List<int>();
                HashSet<int> changedIndices = new HashSet<int>();

                Debug.Log($"Generating {StaticData.incorrectVals} incorrect values");

               
                int attempts = 0;
                int maxAttempts = 1000;

       
                while (changedIndices.Count < StaticData.incorrectVals && attempts < maxAttempts)
                {
                    attempts++;
                    int randIndex = Random.Range(0, incorrectPattern.Count);
                    Debug.Log($"Attempt {attempts}: Trying index {randIndex}, already changed: {changedIndices.Contains(randIndex)}");

                    if (!changedIndices.Contains(randIndex))
                    {
                        int original = incorrectPattern[randIndex];
                        int newVal;
                        int innerAttempts = 0;
                        int maxInnerAttempts = 100;

                        Debug.Log($"Original value at index {randIndex}: {original}");

          
                        if (original <= 1)
                        {
        
                            Debug.LogWarning($"Cannot generate incorrect value for {original} (too small)");
                            continue;
                        }

                        do
                        {
                            innerAttempts++;
                            newVal = Random.Range(1, original); 
                            Debug.Log($"Inner attempt {innerAttempts}: newVal = Random.Range(1, {original}) = {newVal}");
                        } while (newVal == original && innerAttempts < maxInnerAttempts);

                        if (innerAttempts >= maxInnerAttempts)
                        {
                            Debug.LogError($"Could not generate valid value for index {randIndex} after {maxInnerAttempts} attempts");
      
                            newVal = Mathf.Max(1, original - 1);
                        }

                        Debug.Log($"Setting index {randIndex} from {original} to {newVal}");
                        incorrectPattern[randIndex] = newVal;
                        changedIndices.Add(randIndex);

                        // Save wrong index and value
                        StaticData.incorrectIndices.Add(randIndex);
                        StaticData.incorrectValues.Add(newVal);

                        Debug.Log($"Progress: {changedIndices.Count}/{StaticData.incorrectVals} changes made");
                    }
                }

                if (attempts >= maxAttempts)
                {
                    Debug.LogError($"Could not generate {StaticData.incorrectVals} incorrect values after {maxAttempts} attempts");
                }

                StaticData.incorrectToolPattern = incorrectPattern;
                Debug.Log("Final incorrect pattern generated: " + string.Join(", ", incorrectPattern));
                Debug.Log("Changed indices: " + string.Join(", ", StaticData.incorrectIndices));
                Debug.Log("Changed values: " + string.Join(", ", StaticData.incorrectValues));
            }
        }
        else if (StaticData.toolDifficulty == 2 || (StaticData.toolDifficulty == 1 && StaticData.medValue > 5))
        {
            Debug.Log("Zero zero zero zero zero!");
            bool isValidMissing = false;

            if (StaticData.incorrectIndices != null && StaticData.incorrectValues != null)
            {
                if (StaticData.incorrectIndices.Count == StaticData.incorrectValues.Count &&
                    StaticData.incorrectIndices.Count == StaticData.missingVals)
                {
                    isValidMissing = true;
                    for (int i = 0; i < StaticData.incorrectValues.Count; i++)
                    {
                        int idx = StaticData.incorrectIndices[i];

                        if (idx < 0 || idx >= currentPattern.Count)
                        {
                            isValidMissing = false;
                            break;
                        }

                        int original = currentPattern[idx];
                        int loaded = StaticData.incorrectValues[i];

                        if (loaded == original || loaded < 0)
                        {
                            isValidMissing = false;
                            break;
                        }
                    }
                }
            }


            if (isValidMissing)
            {
                List<int> loadedPattern = new List<int>(currentPattern);
                for (int i = 0; i < StaticData.incorrectIndices.Count; i++)
                {
                    loadedPattern[StaticData.incorrectIndices[i]] = StaticData.incorrectValues[i];
                }
                StaticData.incorrectToolPattern = loadedPattern;
                Debug.Log("Loaded incorrect pattern from StaticData: " + string.Join(", ", loadedPattern));
            }
            else
            {
                List<int> incorrectPattern = new List<int>(currentPattern);
                StaticData.incorrectIndices = new List<int>();
                StaticData.incorrectValues = new List<int>();

                HashSet<int> changedIndices = new HashSet<int>();

                while (changedIndices.Count < StaticData.missingVals)
                {
                    int randIndex = Random.Range(0, incorrectPattern.Count);

                    if (!changedIndices.Contains(randIndex))
                    {
                        int original = incorrectPattern[randIndex];
                        int newVal;

                        do
                        {
                            newVal = 0; 
                        } while (newVal == original || newVal < 0);

                        incorrectPattern[randIndex] = newVal;
                        changedIndices.Add(randIndex);

                        
                        StaticData.incorrectIndices.Add(randIndex);
                        StaticData.incorrectValues.Add(newVal);
                    }
                }

                StaticData.incorrectToolPattern = incorrectPattern;
                Debug.Log("Missing pattern generated: " + string.Join(", ", incorrectPattern));
            }
        }


        Debug.Log("Painty painty");
        ConfigureDifficulty(out patternLength, out incorrectVals, out missingVals, out noOfTypes, Minigame.paint);
        currentPaintPattern = GeneratePaintPatternArray(6);
        StaticData.paintPattern = currentPaintPattern;

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
        OrderManager.Instance.orderReceived = false; // Reset order received status
        level++;
        StaticData.dayNo = level;

        if (StaticData.startOfDay == false)
        {
            Debug.Log("Good job, let's start the day again!");
            RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(true);
            RaycastInteractor.Instance.readyText.gameObject.SetActive(true);
            StaticData.startOfDay = true;
        }

        StaticData.incorrectIndices.Clear();
        StaticData.incorrectValues.Clear();
        StaticData.missingVals = 0; 
        StaticData.incorrectVals = 0; 
        StaticData.selectedFastenerIndex = Random.Range(0, 3);
        StaticData.paintWrong = 0;
        StaticData.toolWrong = 0;
        StaticData.wireWrong = 0;


        if (toolScore >= 0 && toolScore < 200) //Originally 200
        {
            StaticData.toolDifficulty = 0; // Easy
            Debug.Log("Static Data for tool difficulty is easy!");
        }
        else if (toolScore >= 200 && toolScore < 500)
        {
            StaticData.toolDifficulty = 1; // Medium
            Debug.Log("Static Data for tool difficulty is medium!");
        }
        else if (toolScore >= 500)
        {
            StaticData.toolDifficulty = 2; // Hard
            Debug.Log("Static Data for tool difficulty is hard!");
        }

        if (paintScore >= 0 && paintScore < 200)
        {
            StaticData.paintDifficulty = 0; // Easy
            Debug.Log("Static Data for paint difficulty is easy!");
        }
        else if (paintScore >= 200 && paintScore < 500)
        {
            StaticData.paintDifficulty = 1; // Medium
            Debug.Log("Static Data for paint difficulty is medium!");
        }
        else if (paintScore >= 500)
        {
            StaticData.paintDifficulty = 2; // Hard
            Debug.Log("Static Data for paint difficulty is hard!");
        }

        if (wireScore >= 0 && wireScore < 250) //We do not have wire yet
        {
            StaticData.wireDifficulty = 0; // Easy
            Debug.Log("Static Data for wire difficulty is easy!");
        }
        else if (wireScore >= 250 && wireScore < 500)
        {
            StaticData.wireDifficulty = 1; // Medium
            Debug.Log("Static Data for wire difficulty is medium!");
        }
        else if (wireScore >= 500)
        {
            StaticData.wireDifficulty = 2; // Hard
            Debug.Log("Static Data for wire difficulty is hard!");
        }


        foreach (Station station in Station.AllStations)
        {
            station.SetStationVisibility();
        }
        GenerateAndStorePattern();

        dayNumber.text = "Day: " + this.level;
        moneyText.text = this.money.ToString();
        Debug.Log("Starting Level " + level);
        if (TimerScript.instance != null && TimerScript.instance.timer != null)
        {
            TimerScript.instance.timer.gameObject.SetActive(false); // hide
        }
        TimerScript.instance.ResetTimer();

        StartCoroutine(EnableInteractorAfterFrame());
    }


    private IEnumerator EnableInteractorAfterFrame()
    {
        yield return null; // wait one frame
        RaycastInteractor ri = Object.FindFirstObjectByType<RaycastInteractor>();
        if (ri != null) ri.enabled = true;
    }


    public void CompleteLevel()
    {
        Debug.Log("Level " + level + " complete!");
        StartNewLevel();
    }

}
