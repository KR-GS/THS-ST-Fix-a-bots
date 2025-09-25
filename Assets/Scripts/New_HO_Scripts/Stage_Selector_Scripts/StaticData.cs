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

    public static int maxNumber, coefficient, constant, prePressedCount, stageNum, tutorialType;
    public static float cycleInterval;
    public static float cycleLeniency;
    public static bool isFormulaSeen, lockCoefficient, lockConstant, isRandomSequence, refSeen;

    //USED FOR RANDOM BLOCK SPAWNING

    public static List<int> stageRandomCoefficientCount;
    public static List<int> stageMaxCoefficientValue;
    public static List<int> stageRandomConstantCount;
    public static List<int> stageMaxConstantValue;

    //How swipes are managed per stage in HO
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

    //

    /* static StaticData()
     {
         ResetStageData();
     }
    */
    static StaticData()
    {
        stageLives = new List<int>(new int[STAGE_COUNT]);
        stageRestarts = new List<int>(new int[STAGE_COUNT]);
        stageStars = new List<int>(new int[STAGE_COUNT]);
        stageTime = new List<float>(new float[STAGE_COUNT]);
        formulaAttempts = new List<string>(new string[STAGE_COUNT]);

         stageRandomCoefficientCount = new List<int>()
        {
            // SCENARIO 1
            2,
            3,
            2,
            1,
            2,
            1,
            2,
            3,
            3,
            2,
            1,
            2,
            3,
            3,
            2,
            
            // SCENARIO 2
            2,
            3,
            2,
            1,
            2,
            1,
            2,
            3,
            3,
            2,
            1,
            2,
            3,
            3,
            2,
            
            //SCENARIO 3
            2,
            3,
            2,
            1,
            2,
            1,
            2,
            3,
            3,
            2,
            1, 
            2,
            3, 
            3, 
            2 
        };

        stageMaxCoefficientValue = new List<int>()
        {
            //SCENARIO 1
            1,
            2,
            3,
            4,
            3,
            5,
            3,
            4,
            3,
            4,
            5,
            3,
            4,
            3,
            4,

            //SCENARIO 2
            1,
            2,
            3,
            4,
            3,
            5,
            3,
            4,
            3,
            4,
            5,
            3,
            4,
            3,
            4,

            //SCENARIO 3
            1,
            2,
            3,
            4,
            3,
            5,
            3,
            4,
            3,
            4,
            5,  
            3,  
            4, 
            3, 
            4,  
        };

        stageRandomConstantCount = new List<int>()
        {
            //SCENARIO 1
            1,
            2,
            2,
            3,
            1,
            2,
            2,
            2,
            2,
            3,
            2,
            2,
            2,
            2,
            3,

            //SCENARIO 2
            1,
            2,
            2,
            3,
            1,
            2,
            2,
            2,
            2,
            3,
            2,
            2,
            2,
            2,
            3,

            //SCENARIO 3
            1,
            2,
            2,
            3,
            1,
            2,
            2,
            2,
            2,
            3,
            2, 
            2, 
            2, 
            2, 
            3,  
        };

        stageMaxConstantValue = new List<int>()
        {
            //SCENARIO 1
            3,
            3,
            4,
            4,
            5,
            5,
            4,
            3,
            4,
            4,
            5,
            4,
            3,
            4,
            4,

            //SCENARIO 2
            3,
            3,
            4,
            4,
            5,
            5,
            4,
            3,
            4,
            4,
            5,
            4,
            3,
            4,
            4,

            //SCENARIO 3
            3,
            3,
            4,
            4,
            5,
            5,
            4,
            3,
            4,
            4,
            5,  
            4, 
            3,  
            4,  
            4,   
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
