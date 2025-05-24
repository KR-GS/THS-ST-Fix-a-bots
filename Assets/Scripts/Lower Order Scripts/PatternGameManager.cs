using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class PatternGameManager : MonoBehaviour
{
    private int generatedDifference;

    private List<int> numberPatternList = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generatedDifference = Random.Range(1, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int ReturnDifference()
    {
        return generatedDifference;
    }

    private void generatePatternArray(int patternLen)
    {
        for(int i = 1; i<= patternLen; i++)
        {
            numberPatternList.Add(generatedDifference * i);
        }
    }

    public List<int> returnPatternArray(int intInput)
    {
        generatePatternArray(intInput);
        return numberPatternList;
    }
}
