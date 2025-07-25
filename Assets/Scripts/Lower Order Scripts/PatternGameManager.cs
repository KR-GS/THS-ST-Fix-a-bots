using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class PatternGameManager : MonoBehaviour
{
    [SerializeField]
    private int base_Lowest;

    [SerializeField]
    private int base_Highest;

    [SerializeField]
    private int diff_Lowest;

    [SerializeField] 
    private int diff_Highest;

    [SerializeField]
    private DifficultyManager difficulty;

    private int generatedDifference;

    //private List<int> numberPatternList = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public int ReturnDifference()
    {
        return generatedDifference;
    }

    private List<int> GeneratePatternArray(int patternLen)
    {
        generatedDifference = Random.Range(diff_Lowest, diff_Highest);

        int baseHolder = Random.Range(base_Lowest, base_Highest);

        List<int> numberPatternList = new List<int>();

        if (difficulty.GetMinigame() == "paint")
        {
            for (int i = 1; i <= patternLen; i++)
            {
                numberPatternList.Add(baseHolder + (generatedDifference * i));
            }
        }
        else
        {
            if (difficulty.GetDifficulty() == "easy")
            {
                for (int i = 1; i <= patternLen; i++)
                {
                    numberPatternList.Add(baseHolder + (generatedDifference * i));
                }
            }
            else if (difficulty.GetDifficulty() == "hard")
            {
                for (int i = 1; i <= patternLen; i++)
                {
                    baseHolder = baseHolder + (generatedDifference + i);
                    numberPatternList.Add(baseHolder);
                }
            }
        }

        return numberPatternList;
    }

    public List<int> ReturnPatternArray(int intInput)
    {
        List<int> numberPatternList = GeneratePatternArray(intInput);
        return numberPatternList;
    }
}
