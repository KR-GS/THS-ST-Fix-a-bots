using System.Collections.Generic;
using UnityEngine;

public class StaticData
{
    // USED TO SEE WHICH SCENE IT IS
    public static bool isOnLowerOrder;
    public static bool isOnHigherOrder;

    //HO SETTINGS
    private const int STAGE_COUNT = 10; 

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
        { 1, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right" } },
        { 2, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 3, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 4, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 5, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 6, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 7, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 8, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 9, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
        { 10, new List<string> { "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right", "Up", "Down", "Left", "Right"} },
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
            2, 
            3, 
            2, 
            1, 
            2, 
            1, 
            2,
            3, 
            3, 
            2 
        };

        stageMaxCoefficientValue = new List<int>()
        {
            1,  
            2,  
            3,  
            4,  
            3,  
            5,  
            3,  
            4, 
            3, 
            4  
        };

        stageRandomConstantCount = new List<int>()
        {
            1, 
            2, 
            2, 
            3, 
            1, 
            2, 
            2, 
            2, 
            2, 
            3  
        };

        stageMaxConstantValue = new List<int>()
        {
            3,  
            3,   
            4,   
            4,  
            5,  
            5,  
            4, 
            3,  
            4,  
            4   
        };
    }

    public static void EnsureStageListSizes()
    {
        EnsureListSize(stageLives, STAGE_COUNT);
        EnsureListSize(stageRestarts, STAGE_COUNT);
        EnsureListSize(stageTime, STAGE_COUNT);
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
    

   /*
    //EVERYTHING HERE IS THE ONLY ONE TO BE SAVED
    public static List<int> stageLives, stageRestarts;
    public static List<float> stageTime;
    public static int numStageDone = 0;
    //ONLY THING SAVED ARE THE THINGS ABOVE

    public static int maxNumber, coefficient, constant, prePressedCount, stageNum;
    public static float cycleInterval;
    public static float cycleLeniency;
    public static bool isFormulaSeen, lockCoefficient, lockConstant, isRandomSequence;

    
    static StaticData()
    {
        int stageCount = 5; 
        stageLives = new List<int>(new int[stageCount]);
        stageRestarts = new List<int>(new int[stageCount]);
        stageTime = new List<float>(new float[stageCount]);
    }
   */

}
