using System.Globalization;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    private enum Difficulty
    {
        tutorial,
        easy,
        medium,
        hard
    }

    private enum Minigame
    {
        tool,
        wire,
        paint
    }

    [SerializeField]
    private Difficulty level;

    [SerializeField]
    private Minigame gameType;

    private int missingVals;

    private int incorrectVals;

    private int noOfTypes;

    private int patternLength;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Debug.Log("Static Data for Day Number is....: " + StaticData.dayNo);
        if(StaticData.dayNo > 0 && StaticData.dayNo < 5)
        {
            level = Difficulty.easy;
        }
        else if (StaticData.dayNo >= 5 && StaticData.dayNo < 10)
        {
            level = Difficulty.medium;
        }
        else if (StaticData.dayNo >= 10)
        {
            level = Difficulty.hard;
        }
        

        if (level == Difficulty.easy)
        {
            missingVals = 0;
            noOfTypes = 1;
            
            if (gameType == Minigame.tool)
            {
                incorrectVals = Random.Range(1, 2);
                patternLength = Random.Range(4, 7);
            }
            else
            {
                incorrectVals = 1;
            }
        }
        else if (level == Difficulty.medium)
        {
            noOfTypes = 2;

            if (gameType == Minigame.tool)
            {
                patternLength = Random.Range(5, 10);
                incorrectVals = Random.Range(2, 3);
                missingVals = Random.Range(1, 3);
            }
            else
            {
                incorrectVals = 1;
                missingVals = 1;
            }
        }
        else if (level == Difficulty.hard)
        {
            noOfTypes = Random.Range(1, 3);
            if (gameType == Minigame.tool)
            {
                patternLength = Random.Range(6, 12);
                incorrectVals = 0;
                missingVals = Random.Range(3, 5);
            }
            else
            {
                patternLength = Random.Range(5, 10);
                incorrectVals = 0;
                missingVals = 2;
            }
        }
        else
        {
            incorrectVals = 1;
            missingVals = 0;
            noOfTypes = 1;
        }

        if (gameType != Minigame.tool)
        {
            patternLength = 4;
        }
    }

    public int GetLengthOfPattern()
    {
        return patternLength;
    }

    public int GetNumberOfMissingVal()
    {
        return missingVals;
    }

    public int GetNumberOfIncorrectVal()
    {
        return incorrectVals;
    }

    public int GetNumberOfTypes()
    {
        return noOfTypes;
    }

    //Returns difficulty of level in string format
    public string GetDifficulty()
    {
        return level.ToString();
    }
}
