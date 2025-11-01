using System.Collections.Generic;
using UnityEngine;

public class StaticData
{

    private const int STAGE_COUNT = 5;

    public static List<int> stageLives, stageRestarts;
    public static List<float> stageTime;
    public static int numStageDone = 0;

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
    public static List<int[]> paintPattern;
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


    public static Sprite lastTVSprite;

    public static int maxNumber, coefficient, constant, prePressedCount, stageNum;
    public static float cycleInterval;
    public static float cycleLeniency;
    public static bool isFormulaSeen, lockCoefficient, lockConstant, isRandomSequence;

    /* static StaticData()
     {
         ResetStageData();
     }
    */
    static StaticData()
    {
        stageLives = new List<int>(new int[STAGE_COUNT]);
        stageRestarts = new List<int>(new int[STAGE_COUNT]);
        stageTime = new List<float>(new float[STAGE_COUNT]);
        //numStageDone = 0;
    }

    public static void EnsureStageListSizes()
    {
        EnsureListSize(stageLives, STAGE_COUNT);
        EnsureListSize(stageRestarts, STAGE_COUNT);
        EnsureListSize(stageTime, STAGE_COUNT);
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
