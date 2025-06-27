using System.Collections.Generic;
using UnityEngine;

public class HOStageData : MonoBehaviour
{
    private int stageNum, numLives, numRestarts;
    private float elapsedTime;
    private List<FormulaAttempt> attempts = new List<FormulaAttempt>();

    public List<FormulaAttempt> GetAttempts()
    {
        return attempts;
    }
    public HOStageData()
    {
        stageNum = 0;
        numLives = 0;
        numRestarts = 0;
        elapsedTime = 0f;
    }

    
    public void SetStageNum(int s)
    {
        stageNum = s;
    }
    public void SetNumLives(int s)
    {
        numLives = s;
    }
    public void SetNumRestarts(int s)
    {
        numRestarts = s;
    }
    public void SetElapsedTime(float f)
    {
        elapsedTime = f;
    }

    public int GetStageNum()
    {
        return stageNum;
    }
    public int GetNumLives()
    {
        return numLives;
    }
    public int GetNumRestarts()
    {
        return numRestarts;
    }
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void LoadFromStaticData()
    {
        StaticData.EnsureStageListSizes();

        numLives = StaticData.stageLives[stageNum];
        numRestarts = StaticData.stageRestarts[stageNum];
        elapsedTime = StaticData.stageTime[stageNum];
    }
}
