using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class PatternGameManager : MonoBehaviour
{
    private int generatedDifference;

    private List<int> numberPatternList = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        generatedDifference = Random.Range(1, 5);
    }

    public int ReturnDifference()
    {
        return generatedDifference;
    }

    private void GeneratePatternArray(int patternLen)
    {
        int baseHolder = Random.Range(1, 4);
        for(int i = 1; i<= patternLen; i++)
        {
            numberPatternList.Add(baseHolder + (generatedDifference * i));
        }
    }

    public List<int> ReturnPatternArray(int intInput)
    {
        GeneratePatternArray(intInput);
        return numberPatternList;
    }
}
