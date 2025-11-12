using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Data.SqlTypes;
using System.Drawing;
using static GameLoopManager;
using System.Threading;

public class GameLoopManager : MonoBehaviour, IDataPersistence
{
    //Pause Button and Panel
    public Button pauseButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button exitButton;
    [SerializeField] public Button shopButton;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private GameObject wireShow;

    [SerializeField] private GameObject toolShow;

    [SerializeField] private GameObject paintShow;



    [SerializeField] private Camera cam;

    [SerializeField] private int base_Lowest = 1;
    [SerializeField] private int base_Highest = 10;

    [SerializeField] private int diff_Lowest = 1;
    [SerializeField] private int diff_Highest = 5;

    private float sceneEnterTime;
    private bool hasRecordedThisVisit = false;

    private int generatedDifference;

    public OrderManager om;

    [SerializeField] private TimerScript ts;

    public RaycastInteractor ri;

    //public RaycastController rc;

    public GameObject TV;

    private List<int> currentPattern;

    private List<int> currentPaintPattern;

    private List<int> currentP2Pattern;

    private List<int> currentWirePattern;

    //public static GameLoopManager Instance;

    //public DataPersistenceManager dpm;

    //[SerializeField] private String fileName;

    public int level = 0;

    public int medValue = 0;

    public int money = 0;

    public bool isPatternStarted;

    public TextMeshProUGUI dayNumber;

    public TextMeshProUGUI prizeText;

    public Image prizeImej;

    private TimerScript timer;

    public Image calendar;

    public int toolScore;

    public int paintScore;

    public int wireScore;

    public SoundEffectsManager soundEffectsManager;

    public Button tutorialButton;

    public void ToggleShow()
    {
        wireShow.GetComponent<StationShow>().enabled = !wireShow.GetComponent<StationShow>().enabled;
        toolShow.GetComponent<StationShow>().enabled = !toolShow.GetComponent<StationShow>().enabled;
        paintShow.GetComponent<StationShow>().enabled = !paintShow.GetComponent<StationShow>().enabled;
    }
    private void Awake()
    {
        pauseButton.onClick.AddListener(() =>
        {
            //rc.DisableRaycasting();
            tutorialButton.gameObject.SetActive(false);
            pausePanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        });

        StaticData.isOnHigherOrderGame = false;
        StaticData.isOnHigherOrder = false;
        StaticData.isOnLowerOrder = true;

        SceneManager.sceneLoaded += OnSceneLoaded;

        // DataPersistenceManager.Instance.SaveGame();
    }

    private void Start()
    {
        Debug.Log("I am calling data at GLM, with values of tooldone, paintdone and wiredone" + StaticData.isToolDone + StaticData.isPaintDone + StaticData.isWireDone);

        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogError("The DataPersistence.Instance is NULL!!!");
        }

        StaticData.isToolDone = StaticData.debugTool;
        StaticData.isPaintDone = StaticData.debugPaint;
        StaticData.isWireDone = StaticData.debugWire;

        Debug.Log("Again again!" + StaticData.isToolDone + StaticData.isPaintDone + StaticData.isWireDone);
    }

    private void OnDestroy()
    {
        DataPersistenceManager.Instance.SaveGame();
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

            StartCoroutine(SaveMinigameRecordAfterLoad());

            GameObject dayTextObject = GameObject.Find("DayNumber");
            if (dayTextObject != null)
            {
                dayNumber = dayTextObject.GetComponent<TextMeshProUGUI>();
                if (dayNumber != null)
                {
                    dayNumber.text = level.ToString();
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


            dayNumber.gameObject.SetActive(true);
            calendar.gameObject.SetActive(true);
            tutorialButton.gameObject.SetActive(true);

            Debug.Log("[LOOK HERE] Pending orders count: " + om.pendingOrders.Count);

            StartCoroutine(UpdateStationsNextFrame());

        }
        else
        {
            if (dayNumber != null) dayNumber.gameObject.SetActive(false);
            if (calendar != null) calendar.gameObject.SetActive(false);
            ri.readyIndicator.gameObject.SetActive(false);
            ri.readyText.gameObject.SetActive(false);
            ShowTV(false);
            tutorialButton.gameObject.SetActive(false);
            if (ts != null && ts.timer != null)
            {
                ts.timer.gameObject.SetActive(false); // hide
            }
            if (ri.ToolIndicator != null) ri.ToolIndicator.gameObject.SetActive(false);
            if (ri.WireIndicator != null) ri.WireIndicator.gameObject.SetActive(false);
            if (ri.PaintIndicator != null) ri.PaintIndicator.gameObject.SetActive(false);
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

    private IEnumerator SaveMinigameRecordAfterLoad()
    {
        // Wait for all LoadData calls to finish
        yield return new WaitForEndOfFrame();

        SaveMinigameRecord();
    }
    private void SaveMinigameRecord()
    {
        /*
        if (DataPersistenceManager.Instance == null)
        {
            Debug.LogWarning("Cannot save record - DataPersistenceManager or minigame type is null");
            return;
        }

        List<int> correctPattern = new List<int>();
        List<int> playerAnswer = new List<int>();
        int level = StaticData.dayNo;
        int wrongs = 0;

        switch (StaticData.enteredStation)
        {
            case 0:
                correctPattern = new List<int>(StaticData.toolPattern ?? new List<int>());
                playerAnswer = new List<int>(StaticData.playerToolPattern ?? new List<int>());
                wrongs = StaticData.pendingToolWrongs;
                break;

            case 1:
                correctPattern = new List<int>(StaticData.paintPattern ?? new List<int>());
                playerAnswer = new List<int>(StaticData.playerPaintPattern ?? new List<int>());
                wrongs = StaticData.pendingPaintWrongs;
                break;

            case 2:
                correctPattern = new List<int>(StaticData.wirePattern ?? new List<int>());
                playerAnswer = new List<int>(StaticData.playerWirePattern ?? new List<int>());
                wrongs = StaticData.pendingWireWrongs;
                break;

            default:
                Debug.LogWarning("Where have you been? There's no soldering station!");
                return;
        }

        StaticData.pendingGameRecord = new GameData.GameRecord(
            correctPattern,
            playerAnswer,
            StaticData.timeSpent,
            level,
            wrongs,
            StaticData.enteredStation,
            StaticData.orderNumber
        );

        Debug.Log($"  Created pending {StaticData.enteredStation} GameRecord:");
        Debug.Log($"  Day: {level}");
        Debug.Log($"  Time spent: {StaticData.timeSpent:F2}s");
        Debug.Log($"  Correct pattern: [{string.Join(", ", correctPattern)}]");
        Debug.Log($"  Player answer: [{string.Join(", ", playerAnswer)}]");
        */
        // Trigger save through the proper pipeline
        DataPersistenceManager.Instance.SaveGame();

        StaticData.timeSpent = 0f; // Reset after applying
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

        StaticData.startOfDay = data.startOfDay;

        StaticData.medValue = data.medValue;
        StaticData.dayNo = this.level;
        StaticData.orderNumber = data.orderNumber;


        if (dayNumber != null)
        {
            dayNumber.text = this.level.ToString();
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

        if (paintScore >= 0 && paintScore < 200) //originally 0-199
        {
            StaticData.paintDifficulty = 0; // Easy
            Debug.Log("Static Data for paint difficulty is easy!");
        }
        else if (paintScore >= 200 && paintScore < 500) //originally 200-499
        {
            StaticData.paintDifficulty = 1; // Medium
            Debug.Log("Static Data for paint difficulty is medium!");
        }
        else if (paintScore >= 500)
        {
            StaticData.paintDifficulty = 2; // Hard
            Debug.Log("Static Data for paint difficulty is hard!");
        }

        if (wireScore >= 0 && wireScore < 200) 
        {
            StaticData.wireDifficulty = 0; // Easy
            Debug.Log("Static Data for wire difficulty is easy!");
        }
        else if (wireScore >= 200 && wireScore < 500)
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

        StaticData.valuestoChange = data.valuestoChange;

        StaticData.isBlueHammerBought = data.isBlueHammerBought;
        StaticData.isGreenHammerBought = data.isGreenHammerBought;
        StaticData.isRedHammerBought = data.isRedHammerBought;
        StaticData.isGreenPhilipsBought = data.isGreenPhilipsBought;
        StaticData.isYellowPhilipsBought = data.isYellowPhilipsBought;
        StaticData.isRedPhilipsBought = data.isRedPhilipsBought;
        StaticData.isYellowFlatBought = data.isYellowFlatBought;
        StaticData.isRedFlatBought = data.isRedFlatBought;
        StaticData.isGreenFlatBought = data.isGreenFlatBought;
        StaticData.isRedWrenchBought = data.isRedWrenchBought;
        StaticData.isBlueWrenchBought = data.isBlueWrenchBought;
        StaticData.isGreenWrenchBought = data.isGreenWrenchBought;
        StaticData.equippedHammer = data.equippedHammer;
        StaticData.equippedPhilipsScrewdriver = data.equippedPhilipsScrewdriver;
        StaticData.equippedFlatScrewdriver = data.equippedFlatScrewdriver;
        StaticData.equippedWrench = data.equippedWrench;
        StaticData.isFirstWS = data.isFirstWS;
        StaticData.isFirstTool = data.isFirstTool;
        StaticData.isFirstPaint = data.isFirstPaint;
        StaticData.isFirstWire = data.isFirstWire;

        Debug.Log("Tool wrongs: " + StaticData.toolWrong + ", Paint wrongs: " + StaticData.paintWrong + ", Wire wrongs: " + StaticData.wireWrong);

        StaticData.paintpatternLength = data.paintpatternLength;
        StaticData.toolpatternLength = data.toolpatternLength;
        StaticData.wirepatternLength = data.wirepatternLength;
        StaticData.selectedFastenerIndex = data.selectedFastenerIndex;
        StaticData.selectedStickerIndex = data.selectedStickerIndex;
        StaticData.selectedStickerIndexTwo = data.selectedStickerIndexTwo;
        if (data.correctPattern != null)
        {
            currentPattern = new List<int>(data.correctPattern);
            StaticData.toolPattern = new List<int>(data.correctPattern);
        }
        else
        {
            currentPattern = new List<int>();
            StaticData.toolPattern = new List<int>();
        }

        if(data.paintPattern != null)
        {
            currentPaintPattern = new List<int>(data.paintPattern);
            StaticData.paintPattern = new List<int>(data.paintPattern);
        }
        else
        {
            currentPaintPattern = new List<int>();
            StaticData.paintPattern = new List<int>();
        }

        if (data.paint2Pattern != null)
        {
            currentP2Pattern = new List<int>(data.paint2Pattern);
            StaticData.paint2Pattern = new List<int>(data.paint2Pattern);
        }
        else
        {
            currentP2Pattern = new List<int>();
            StaticData.paint2Pattern = new List<int>();
        }

        if (data.wirePattern != null)
        {
            currentWirePattern = new List<int>(data.wirePattern);
            StaticData.wirePattern = new List<int>(data.wirePattern);
        }
        else
        {
            currentWirePattern = new List<int>();
            StaticData.wirePattern = new List<int>();
        }

        if (data.incorrectPattern != null)
        {
            StaticData.incorrectToolPattern = new List<int>(data.incorrectPattern);
        }
        else
        {
            StaticData.incorrectToolPattern = new List<int>();
        }

        if (data.incorrectIndices != null)
        {
            StaticData.incorrectIndices = new List<int>(data.incorrectIndices);
        }
        else
        {
            StaticData.incorrectIndices = new List<int>();
        }

        if (data.incorrectValues != null)
        {
            StaticData.incorrectValues = new List<int>(data.incorrectValues);
        }
        else
        {
            StaticData.incorrectValues = new List<int>();
        }


        StaticData.orderReceived = data.orderReceived;
        StaticData.isOrderChecked = data.isOrderChecked;

        if (data.loGameHistory != null)
        {
            Debug.Log($"Loaded {data.loGameHistory.Count} game records from save file");
        }
        else
        {
            Debug.LogWarning("gameHistory is null in loaded data!");
            data.loGameHistory = new List<GameData.GameRecord>();
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
        data.paint2Pattern = new List<int>(currentP2Pattern); // Store the current paint2 pattern
        data.wirePattern = new List<int>(currentWirePattern); // Store the current wire pattern
        data.paintScore = this.paintScore;
        data.toolScore = this.toolScore;
        data.wireScore = this.wireScore;
        data.incorrectValues = new List<int>(StaticData.incorrectValues); // Store incorrect values
        data.selectedFastenerIndex = StaticData.selectedFastenerIndex;
        data.selectedStickerIndex = StaticData.selectedStickerIndex;
        data.selectedStickerIndexTwo = StaticData.selectedStickerIndexTwo;
        data.paintpatternLength = StaticData.paintpatternLength;
        data.toolpatternLength = StaticData.toolpatternLength;
        data.wirepatternLength = StaticData.wirepatternLength;
        data.startOfDay = StaticData.startOfDay;
        data.toolWrong = StaticData.toolWrong;
        data.paintWrong = StaticData.paintWrong;
        data.wireWrong = StaticData.wireWrong;
        data.valuestoChange = StaticData.valuestoChange;
        data.isOrderChecked = StaticData.isOrderChecked;
        data.orderReceived = StaticData.orderReceived;
        data.isBlueHammerBought = StaticData.isBlueHammerBought;
        data.isGreenHammerBought = StaticData.isGreenHammerBought;
        data.isRedHammerBought = StaticData.isRedHammerBought;
        data.isGreenPhilipsBought = StaticData.isGreenPhilipsBought;
        data.isYellowPhilipsBought = StaticData.isYellowPhilipsBought;
        data.isRedPhilipsBought = StaticData.isRedPhilipsBought;
        data.isYellowFlatBought = StaticData.isYellowFlatBought;
        data.isRedFlatBought = StaticData.isRedFlatBought;
        data.isGreenFlatBought = StaticData.isGreenFlatBought;
        data.isRedWrenchBought = StaticData.isRedWrenchBought;
        data.isBlueWrenchBought = StaticData.isBlueWrenchBought;
        data.isGreenWrenchBought = StaticData.isGreenWrenchBought;
        data.equippedWrench = StaticData.equippedWrench;
        data.equippedHammer = StaticData.equippedHammer;
        data.equippedPhilipsScrewdriver = StaticData.equippedPhilipsScrewdriver;
        data.equippedFlatScrewdriver = StaticData.equippedFlatScrewdriver;
        data.isFirstWS = StaticData.isFirstWS;
        data.isFirstTool = StaticData.isFirstTool;
        data.isFirstPaint = StaticData.isFirstPaint;
        data.isFirstWire = StaticData.isFirstWire;
        data.orderNumber = StaticData.orderNumber;

        Debug.Log($"[SAVEDATA] Current gameHistory count: {data.loGameHistory?.Count ?? 0}");

        if (StaticData.pendingGameRecord != null)
        {
            if (data.loGameHistory == null)
            {
                data.loGameHistory = new List<GameData.GameRecord>();
                Debug.Log("Initialized gameHistory list");
            }

            data.loGameHistory.Add(StaticData.pendingGameRecord);
            Debug.Log($"Added GameRecord to history. Total records: {data.loGameHistory.Count}");
            Debug.Log($"[SAVEDATA]   Added NEW GameRecord to history. Total records: {data.loGameHistory.Count}");
            Debug.Log($"[SAVEDATA]   - Day: {StaticData.pendingGameRecord.day}");
            Debug.Log($"[SAVEDATA]   - Station: {StaticData.pendingGameRecord.station}");
            Debug.Log($"[SAVEDATA]   - Time: {StaticData.pendingGameRecord.timeSpent:F2}s");

            // Clear the pending record after adding
            StaticData.pendingGameRecord = null;
        }
        else
        {
            Debug.Log("Do you exist mr. PendingGameRecord? Are you there???");
        }
    }

    public void ShowTutorialBot()
    {
        tutorialButton.gameObject.SetActive(true);
    }

    private List<int> GeneratePatternArray(int patternLen) //This is for tool
    {
        generatedDifference = Random.Range(diff_Lowest, diff_Highest);
        StaticData.sequenceDiff = generatedDifference; // Store in StaticData for sequence difference
        int baseHolder = Random.Range(base_Lowest, base_Highest);

        List<int> numberPatternList = new List<int>();

        DifficultyLevel difficulty = DifficultyLevel.easy;

        switch (StaticData.toolDifficulty)
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
            int isNega = Random.Range(0, 1) * 2 - 1;
            /*
            for (int i = 1; i <= patternLen; i++)
            {
                numberPatternList.Add(baseHolder + isNega * (generatedDifference * i));
            }
            */

            if (StaticData.medValue <= 5){
                if (isNega == -1)
                {
                    baseHolder += patternLen * generatedDifference;
                }

                for (int i = 1; i <= patternLen; i++)
                {
                    numberPatternList.Add(baseHolder + isNega * (generatedDifference * i));
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
            int isNega = Random.Range(0, 1) * 2 - 1;

            if (isNega == -1)
            {
                for (int i = 1; i <= patternLen; i++)
                {
                    baseHolder += (generatedDifference + i);
                }
            }

            for (int i = 1; i <= patternLen; i++)
            {
                baseHolder += isNega * (generatedDifference + i);
                numberPatternList.Add(baseHolder);
            }
        }

        return numberPatternList;
    }

    private List<int> GeneratePaintPatternArray(int patternLen)
    {
        generatedDifference = (Random.Range(diff_Lowest, diff_Highest));
        StaticData.sequenceDiff = generatedDifference; // Store in StaticData for sequence difference
        int baseHolder = Random.Range(base_Lowest, base_Highest);

        generatedDifference = generatedDifference / 2; //formally wasn't divided by 2

        Debug.Log("Generated difference between numbers: " + generatedDifference);

        if(generatedDifference < 1)
        {
            Debug.Log("Why did 1 get divided by 2? TO get a generated difference of 0!");
            generatedDifference = 1;
        }


        baseHolder = baseHolder / 2;

        Debug.Log("Base for sticker pattern: " + baseHolder);

        List<int> numberPaintPatternList = new List<int>();

        DifficultyLevel difficulty = DifficultyLevel.easy;

        switch (StaticData.paintDifficulty)
        {
            case 0: difficulty = DifficultyLevel.easy; break;
            case 1: difficulty = DifficultyLevel.medium; break;
            case 2: difficulty = DifficultyLevel.hard; break;
        }

        if (difficulty == DifficultyLevel.easy)
        {
            for (int i = 0; i < patternLen; i++)
            {
                numberPaintPatternList.Add(baseHolder + (generatedDifference * i));
            }
        }
        else if (difficulty == DifficultyLevel.medium)
        {
            int isNega = Random.Range(0, 1) * 2 - 1;
            int medValue = Random.Range(1,10);

            if(medValue <= 5)
            {
                if (isNega == -1)
                {
                    baseHolder += patternLen * generatedDifference;
                }

                for (int i = 1; i <= patternLen; i++)
                {
                    numberPaintPatternList.Add(baseHolder + isNega * (generatedDifference * i));
                }
            }
            else
            {
                for (int i = 1; i <= patternLen; i++)
                {
                    baseHolder += (generatedDifference + i);
                    numberPaintPatternList.Add(baseHolder);
                }
            }
        }
        else if (difficulty == DifficultyLevel.hard)
        {
            int isNega = Random.Range(0, 1) * 2 - 1;

            if (isNega == -1)
            {
                for (int i = 1; i <= patternLen; i++)
                {
                    baseHolder += (generatedDifference + i);
                }
            }

            for (int i = 1; i <= patternLen; i++)
            {
                baseHolder += isNega * (generatedDifference + i);
                numberPaintPatternList.Add(baseHolder);
            }
        }


        return numberPaintPatternList;
    }

    private List<int> GenerateWirePatternArray(int patternLen) //This is for tool
    {
        generatedDifference = Random.Range(diff_Lowest, diff_Highest);
        StaticData.sequenceDiff = generatedDifference; // Store in StaticData for sequence difference
        int baseHolder = Random.Range(base_Lowest, base_Highest);

        List<int> numberWirePatternList = new List<int>();

        DifficultyLevel difficulty = DifficultyLevel.easy;

        switch (StaticData.wireDifficulty)
        {
            case 0: difficulty = DifficultyLevel.easy; break;
            case 1: difficulty = DifficultyLevel.medium; break;
            case 2: difficulty = DifficultyLevel.hard; break;
        }

        if (difficulty == DifficultyLevel.easy || difficulty == DifficultyLevel.medium)
        {
            for (int i = 1; i <= patternLen; i++)
            {
                numberWirePatternList.Add(baseHolder + (generatedDifference * i));
            }
        }

        else if (difficulty == DifficultyLevel.hard)
        {
            for (int i = 1; i <= patternLen; i++)
            {
                baseHolder += (generatedDifference + i);
                numberWirePatternList.Add(baseHolder);
            }

        }

        return numberWirePatternList;
    }

    public List<int> GetPattern(int length)
    {
        return GeneratePatternArray(length);
    }

    public List<int> GetPaintPattern(int length)
    {
        return GeneratePaintPatternArray(length);
    }

    public List<int> GetWirePattern(int length)
    {
        return GenerateWirePatternArray(length);
    }

    private void ConfigureDifficulty(out int incorrectVals, out int missingVals, out int noOfTypes, Minigame gameType)
    {
        Debug.Log("hello from configure");
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
                StaticData.toolpatternLength = 7;
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
                StaticData.toolpatternLength = Random.Range(7, 9);
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
                StaticData.toolpatternLength = Random.Range(7, 10);
                incorrectVals = 0;
                missingVals = 1;
            }
            else
            {
                //patternLength = Random.Range(5, 6);
                incorrectVals = 0;
                missingVals = 2;
            }
        }


        if (gameType == Minigame.paint)
        {
            StaticData.paintpatternLength = 6;
        }

        if (gameType == Minigame.wire)
        {
            StaticData.wirepatternLength = 6;
        }

        //StaticData.patternLength = patternLength;
        //Debug.Log("Checking Pattern Length: " + StaticData.patternLength);
        StaticData.incorrectVals = incorrectVals;
        StaticData.missingVals = missingVals;
        StaticData.noOfTypes = noOfTypes;
        Debug.Log("Final values: " + StaticData.incorrectVals + ", Missing Vals: " + StaticData.missingVals);

    }

    public void GenerateAndStorePattern()
    {
        Debug.Log("Tooly tooly");
        ConfigureDifficulty(out int incorrectVals, out int missingVals, out int noOfTypes, Minigame.tool);
        currentPattern = GeneratePatternArray(StaticData.toolpatternLength);
        StaticData.toolPattern = currentPattern;

       

        Debug.Log("Pattern generated on game start: " + string.Join(", ", currentPattern));
        StaticData.toolPattern = currentPattern; // Store in StaticData for tool pattern
        StaticData.paintPattern = currentPaintPattern;
        StaticData.paint2Pattern = currentP2Pattern;

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

        if(StaticData.paintDifficulty == 0)
        {
            Debug.Log("Painty painty");
            ConfigureDifficulty(out incorrectVals, out missingVals, out noOfTypes, Minigame.paint);
            currentPaintPattern = GeneratePaintPatternArray(StaticData.paintpatternLength);
            Debug.Log("Current Generated Pattern: " + currentPaintPattern);
            StaticData.paintPattern = currentPaintPattern;
            currentP2Pattern = GeneratePaintPatternArray(StaticData.paintpatternLength);
            StaticData.paint2Pattern = currentP2Pattern;

        }
        else if(StaticData.paintDifficulty == 1 || StaticData.paintDifficulty == 2)
        {
            /*
            for (int i = 0; i<2; i++)
            {
                currentPaintPattern = GeneratePaintPatternArray(StaticData.paintpatternLength);
                //Debug.Log("Current Generated Pattern " + i + ": " + currentPaintPattern[0]);
            }
            */

            currentPaintPattern = GeneratePaintPatternArray(StaticData.paintpatternLength);
            StaticData.paintPattern = currentPaintPattern;
            currentP2Pattern = GeneratePaintPatternArray(StaticData.paintpatternLength);
            StaticData.paint2Pattern = currentP2Pattern;
        }

        StaticData.selectedFastenerIndex = Random.Range(0, 4); //based on LoToolMinigame, array size is 4
        StaticData.selectedStickerIndex = Random.Range(0, 4);
        StaticData.selectedStickerIndexTwo = Random.Range(0, 4);

        while(StaticData.selectedStickerIndexTwo == StaticData.selectedStickerIndex) // Ensure different indices
        {
            StaticData.selectedStickerIndexTwo = Random.Range(0, 4);
        }

    /*
        if (StaticData.selectedStickerIndex == StaticData.selectedFastenerIndex) // Ensure different indices
        {
            StaticData.selectedStickerIndex = (StaticData.selectedStickerIndex + 1) % 3; // Wrap around if same index
        }
    */

        Debug.Log("Wire wire, pants on fire");
        ConfigureDifficulty(out incorrectVals, out missingVals, out noOfTypes, Minigame.wire);
        currentWirePattern = GenerateWirePatternArray(StaticData.wirepatternLength);
        StaticData.wirePattern = currentWirePattern;
        StaticData.valuestoChange = Random.Range(2, 6);

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
        om.SetStatus(false);
        om.orderReceived = false; // Reset order received status
        om.prize = 0;
        StaticData.orderReceived = false;
        level++;
        StaticData.dayNo = level;
        StaticData.orderNumber = 1;

        if (StaticData.startOfDay == false)
        {
            Debug.Log("Good job, let's start the day again!");
            ri.readyIndicator.gameObject.SetActive(true);
            ri.readyText.gameObject.SetActive(true);
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

        dayNumber.text = this.level.ToString();
        Debug.Log("Starting Level " + level);
        if (ts != null && ts.timer != null)
        {
            ts.timer.gameObject.SetActive(false); // hide
        }
        ts.ResetTimer();

        StartCoroutine(EnableInteractorAfterFrame());
    }


    private IEnumerator EnableInteractorAfterFrame()
    {
        yield return null; // wait one frame
        RaycastInteractor ri = Object.FindFirstObjectByType<RaycastInteractor>();
        if (ri != null) ri.enabled = true;
    }

    public void HideWorkshopElements()
    {
        if (dayNumber != null) dayNumber.gameObject.SetActive(false);
        if (calendar != null) calendar.gameObject.SetActive(false);
        ri.readyIndicator.gameObject.SetActive(false);
        ri.readyText.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);
        ShowTV(false);
        if (ts != null && ts.timer != null)
        {
            ts.timer.gameObject.SetActive(false); // hide
        }
        if (ri.ToolIndicator != null) ri.ToolIndicator.gameObject.SetActive(false);
        if (ri.WireIndicator != null) ri.WireIndicator.gameObject.SetActive(false);
        if (ri.PaintIndicator != null) ri.PaintIndicator.gameObject.SetActive(false);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
    }

    public void ShowWorkshopElements()
    {
        if (om.GetActiveOrder() != null) //om != null && om.GetActiveOrder() != null
        {
            Debug.Log("Is this for real???");
            if (ri.TVSprite != null)
            {
                ri.TVSprite.sprite = ri.TVSpriteIP;
                ri.readyIndicator.gameObject.SetActive(false);
                ri.readyText.gameObject.SetActive(false);
            }

            if (ts != null && ts.timer != null)
            {
                ts.timer.gameObject.SetActive(true); // hide
            }

            if (ri.currentOrder.needsTool && !StaticData.isToolDone)
            {
                ri.ToolIndicator.gameObject.SetActive(true);
            }
            if (ri.currentOrder.needsPaint && !StaticData.isPaintDone)
            {
                ri.PaintIndicator.gameObject.SetActive(true);
            }
            if (ri.currentOrder.needsWire && !StaticData.isWireDone)
            {
                ri.WireIndicator.gameObject.SetActive(true);
            }
        }

        else if (om.GetActiveOrder() == null) // != null && om.GetActiveOrder() == null
        {
            Debug.Log("No shot sherlock!");
            if (ri.TVSprite != null)
            {
                ri.TVSprite.sprite = ri.TVSpriteNoOrder;
            }

            if (ri.ToolIndicator != null) ri.ToolIndicator.gameObject.SetActive(false);
            if (ri.WireIndicator != null) ri.WireIndicator.gameObject.SetActive(false);
            if (ri.PaintIndicator != null) ri.PaintIndicator.gameObject.SetActive(false);


            if (StaticData.startOfDay == true)
            {
                Debug.Log("It is the start of day indeed!");
                ri.readyIndicator.gameObject.SetActive(true);
                ri.readyText.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("No it ain't the start of the day!");
                ri.readyIndicator.gameObject.SetActive(false);
                ri.readyText.gameObject.SetActive(false);
            }

        }

        dayNumber.gameObject.SetActive(true);
        calendar.gameObject.SetActive(true);
        ShowTV(true);
        tutorialButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);

        if (ts != null && ts.timer != null)
        {
            ts.timer.gameObject.SetActive(false); // hide
        }
    }
    public void CompleteLevel()
    {
        Debug.Log("Level " + level + " complete!");
        StartNewLevel();
    }

}
