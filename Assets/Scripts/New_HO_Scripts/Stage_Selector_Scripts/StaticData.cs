using System.Collections.Generic;
using UnityEngine;

public class StaticData
{
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
    
}
