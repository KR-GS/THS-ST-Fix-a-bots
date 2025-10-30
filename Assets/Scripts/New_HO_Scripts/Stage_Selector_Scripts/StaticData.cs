using System.Collections.Generic;
using UnityEngine;

public class StaticData
{
    // USED TO SEE WHICH SCENE IT IS
    public static bool isOnLowerOrder;
    public static bool isOnLowerOrderGame;
    public static bool isOnHigherOrder;
    public static bool isOnHigherOrderGame;

    //HO SETTINGS
    private const int STAGE_COUNT = 45; 

    public static List<int> stageLives, stageRestarts, stageStars;
    public static List<float> stageTime;
    public static List<string> formulaAttempts;
    public static int numStageDone = 0;
    public static int stageNum;
    public static List<int> maxNumber, coefficient, constant, prePressedCount, tutorialType;
    public static float cycleInterval, cycleLeniency;
    public static List<bool> isFormulaSeen, lockCoefficient, lockConstant, isRandomSequence, refSeen;

    public static int dayNo = 0;

    public static bool isToolDone = false;
    public static bool isWireDone = false;
    public static bool isPaintDone = false;
    public static int selectedFastenerIndex;

    public static int medValue = 0;

    public static bool isOrderChecked = false;

    public static bool sendNewOrder = false;

    public static bool isPatternStarted;

    public static int diffInt = 0; // 0 = easy, 1 = medium, 2 = hard

    public static int toolDifficulty = 0; // 0 = easy, 1 = medium, 2 = hard
    public static int paintDifficulty = 0; // 0 = easy, 1 = medium, 2 = hard
    public static int wireDifficulty = 0; // 0 = easy, 1 = medium, 2 = hard

    public static List<int> toolPattern;
    public static List<int> incorrectToolPattern;
    public static List<int> paintPattern;
    public static List<int> incorrectPaintPattern;
    public static List<int> wirePattern;
    public static List<int> incorrectWirePattern;
    public static int selectedStickerIndex, selectedStickerIndexTwo;
    public static List<int> incorrectIndices = new List<int>();
    public static List<int> incorrectValues = new List<int>();

    public static int paintDiff;
    public static int sequenceDiff;
    public static int paintpatternLength;
    public static int toolpatternLength;
    public static int wirepatternLength;
    public static int incorrectVals;
    public static int missingVals;
    public static int noOfTypes;
    public static int valuestoChange;

    public static bool startOfDay = false;
    public static int toolWrong = 0;
    public static int paintWrong = 0;
    public static int wireWrong = 0;
    public static bool orderReceived = false;


    public static Sprite lastTVSprite;

    //USED FOR RANDOM BLOCK SPAWNING

    public static List<int> stageRandomCoefficientCount;
    public static List<int> stageMaxCoefficientValue;
    public static List<int> stageRandomConstantCount;
    public static List<int> stageMaxConstantValue;

    // FOR IF THE HO SWIPE HINT IS SEEN
    public static List<bool> hintSeen;

    //How swipes are managed per stage in HO
    // UPDATE: As of now "Up" and "Down" and "Left" and "Right" are functionally the same, so they can be used interchangeably
    public static Dictionary<int, List<string>> stageSwipes = new Dictionary<int, List<string>>()
    {
        //SCENARIO 1
        { 1, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 2, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 3, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 4, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 5, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 6, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 7, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 8, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 9, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 10, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 11, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 12, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 13, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 14, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 15, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },

        //SCENARIO 2
        { 16, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 17, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 18, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 19, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 20, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 21, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 22, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 23, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 24, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 25, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 26, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 27, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 28, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 29, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 30, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 31, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },

        //SCENARIO 3
        { 32, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 33, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 34, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 35, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 36, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 37, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 38, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 39, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 40, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 41, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 42, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 43, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 44, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 45, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } }
    };

    static StaticData()
    {
        stageLives = new List<int>(new int[STAGE_COUNT]);
        stageRestarts = new List<int>(new int[STAGE_COUNT]);
        stageStars = new List<int>(new int[STAGE_COUNT]);
        stageTime = new List<float>(new float[STAGE_COUNT]);
        formulaAttempts = new List<string>(new string[STAGE_COUNT]);

        // HOW TO USE
        // Each list below contains 45 entries, one for each stage
        // Example, maxNumber[0] = 15 means that stage 1 has a max number of 15

        // Max number of "periods" in the sequence
        maxNumber = new List<int>()
        {
            // SCENARIO 1
            15, 15, 18, 20, 20, 20, 20, 25, 25, 25, 20, 20, 25, 25, 25,
            // SCENARIO 2
            15, 15, 18, 20, 20, 20, 20, 25, 25, 25, 20, 20, 25, 25, 25,
            // SCENARIO 3
            15, 15, 18, 20, 20, 20, 20, 25, 25, 25, 20, 20, 25, 25, 25
        };

        // Coefficient of the formula
        coefficient = new List<int>()
        {
            // SCENARIO 1
            2, 3, 4, 3, 4, 3, 3, 3, 4, 5, 3, 3, 3, 4, 5,
            // SCENARIO 2
            2, 3, 4, 3, 4, 3, 3, 3, 4, 5, 3, 3, 3, 4, 5,
            // SCENARIO 3
            2, 3, 4, 3, 4, 3, 3, 3, 4, 5, 3, 3, 3, 4, 5
        };

        // Constant of the formula
        constant = new List<int>()
        {
            // SCENARIO 1
            3, 1, 1, -1, -3, 2, -2, -1, 2, -3, 2, -2, -1, 2, -3,
            // SCENARIO 2
            3, 1, 1, -1, -3, 2, -2, -1, 2, -3, 2, -2, -1, 2, -3,
            // SCENARIO 3
            3, 1, 1, -1, -3, 2, -2, -1, 2, -3, 2, -2, -1, 2, -3
        };

        // Number of pre-pressed blocks at the start (From left to right)
        prePressedCount = new List<int>()
        {
            // SCENARIO 1
            3, 2, 0, 3, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1,
            // SCENARIO 2
            3, 2, 0, 3, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1,
            // SCENARIO 3
            3, 2, 0, 3, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1
        };

        // Type of tutorial (NOT USED ANYMORE)
        tutorialType = new List<int>()
        {
            // SCENARIO 1
            0, 1, 1, 2, 2, 0, 0, 3, 0, 3, 0, 0, 3, 0, 3,
            // SCENARIO 2
            0, 1, 1, 2, 2, 0, 0, 3, 0, 3, 0, 0, 3, 0, 3,
            // SCENARIO 3
            0, 1, 1, 2, 2, 0, 0, 3, 0, 3, 0, 0, 3, 0, 3
        };

        // Is the formula helper seen (NOT USED ANYMORE BECAUSE OF NEW UI)
        isFormulaSeen = new List<bool>()
        {
            // SCENARIO 1
            true, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            // SCENARIO 2
            true, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            // SCENARIO 3
            true, false, false, false, false, false, false, false, false, false, false, false, false, false, false
        };

        // Whether the coefficient is locked or not (locked meaning already given and cannot be changed by the player)
        lockCoefficient = new List<bool>()
        {
            // SCENARIO 1
            true, false, false, true, true, true, false, false, false, false, true, false, false, false, false,
            // SCENARIO 2
            true, false, false, true, true, true, false, false, false, false, true, false, false, false, false,
            // SCENARIO 3
            true, false, false, true, true, true, false, false, false, false, true, false, false, false, false
        };

        // Whether the constant is locked or not (locked meaning already given and cannot be changed by the player)
        lockConstant = new List<bool>()
        {
            // SCENARIO 1
            true, true, true, false, false, false, true, false, false, false, false, true, false, false, false,
            // SCENARIO 2
            true, true, true, false, false, false, true, false, false, false, false, true, false, false, false,
            // SCENARIO 3
            true, true, true, false, false, false, true, false, false, false, false, true, false, false, false
        };

        // Whether the sequence is random or not (TO BE USED FOR RANDOM STAGE GENERATION IN THE FUTURE)
        isRandomSequence = new List<bool>()
        {
            // SCENARIO 1
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            // SCENARIO 2
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            // SCENARIO 3
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
        };

        // Whether the reference is seen (NOT USED)
        refSeen = new List<bool>()
        {
            // SCENARIO 1
            true, true, true, false, true, false, true, false, true, false, false, true, false, true, false,
            // SCENARIO 2
            true, true, true, false, true, false, true, false, true, false, false, true, false, true, false,
            // SCENARIO 3
            true, true, true, false, true, false, true, false, true, false, false, true, false, true, false
        };

        // How may wrong coefficient blocks are spawned in the formula input screen
        stageRandomCoefficientCount = new List<int>()
        {
            // SCENARIO 1
            2, 3, 2,  1, 2, 1, 2, 3, 3, 2, 1, 2, 3, 3, 2,
            
            // SCENARIO 2
            2, 3, 2,  1, 2, 1, 2, 3, 3, 2, 1, 2, 3, 3, 2,
            
            //SCENARIO 3
            2, 3, 2,  1, 2, 1, 2, 3, 3, 2, 1, 2, 3, 3, 2,
        };

        // The maximum value that the distraction coefficient can take in the formula input screen
        stageMaxCoefficientValue = new List<int>()
        {
            //SCENARIO 1
            1, 2, 3, 4, 3, 5, 3, 4, 3, 4, 5, 3, 4, 3, 4,

            //SCENARIO 2
            1, 2, 3, 4, 3, 5, 3, 4, 3, 4, 5, 3, 4, 3, 4,

            //SCENARIO 3
            1, 2, 3, 4, 3, 5, 3, 4, 3, 4, 5, 3, 4, 3, 4,
        };

        // How may wrong constant blocks are spawned in the formula input screen
        stageRandomConstantCount = new List<int>()
        {
            //SCENARIO 1
            1, 2, 2, 3, 1, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3,

            //SCENARIO 2
            1, 2, 2, 3, 1, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3,

            //SCENARIO 3
            1, 2, 2, 3, 1, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3,
        };

        // The maximum value that the distraction constant can take in the formula input screen
        stageMaxConstantValue = new List<int>()
        {
            //SCENARIO 1
            3, 3, 4, 4, 5, 5, 4, 3, 4, 4, 5, 4, 3, 4, 4,

            //SCENARIO 2
            3, 3, 4, 4, 5, 5, 4, 3, 4, 4, 5, 4, 3, 4, 4,

            //SCENARIO 3
            3, 3, 4, 4, 5, 5, 4, 3, 4, 4, 5, 4, 3, 4, 4,
        };

        // Whether the swipe hint has been seen for each stage
        hintSeen = new List<bool>()
        {
            //SCENARIO 1
            true, true, true, true, true, true, true, false, false, false, false, false, false, false, false,

            //SCENARIO 2
            true, true, true, true, true, true, true, false, false, false, false, false, false, false, false,

            //SCENARIO 3
            true, true, true, true, true, true, true, false, false, false, false, false, false, false, false,
        };
    }

    public static void EnsureStageListSizes()
    {
        EnsureListSize(stageLives, STAGE_COUNT);
        EnsureListSize(stageRestarts, STAGE_COUNT);
        EnsureListSize(stageTime, STAGE_COUNT);
        EnsureListSize(stageStars, STAGE_COUNT);
        EnsureListSize(formulaAttempts, STAGE_COUNT);

        EnsureListSize(stageRandomCoefficientCount, STAGE_COUNT);
        EnsureListSize(stageMaxCoefficientValue, STAGE_COUNT);
        EnsureListSize(stageRandomConstantCount, STAGE_COUNT);
        EnsureListSize(stageMaxConstantValue, STAGE_COUNT);
    }

    private static void EnsureListSize<T>(List<T> list, int size)
    {
        if (list == null)
        {
            list = new List<T>(new T[size]);
            return;
        }

        while (list.Count < size)
            list.Add(default);

        if (list.Count > size)
            list.RemoveRange(size, list.Count - size);
    }


}
