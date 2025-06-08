using UnityEngine;

public class HOStageData : MonoBehaviour
{
    private int stageNum, numLives, numRestarts;
    private float elapsedTime;

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
    
}
