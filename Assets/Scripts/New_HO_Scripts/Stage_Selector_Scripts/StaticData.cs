using System.Collections.Generic;
using UnityEngine;

public class StaticData
{
    
    private const int STAGE_COUNT = 5; 

    public static List<int> stageLives, stageRestarts, stageStars;
    public static List<float> stageTime;
    public static List<string> formulaAttempts;
    public static int numStageDone = 0;

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
        stageStars = new List<int>(new int[STAGE_COUNT]);
        stageTime = new List<float>(new float[STAGE_COUNT]);
        formulaAttempts = new List<string>(new string[STAGE_COUNT]);
        //numStageDone = 0;
    }

    public static void EnsureStageListSizes()
    {
        EnsureListSize(stageLives, STAGE_COUNT);
        EnsureListSize(stageRestarts, STAGE_COUNT);
        EnsureListSize(stageTime, STAGE_COUNT);
        EnsureListSize(formulaAttempts, STAGE_COUNT);
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
