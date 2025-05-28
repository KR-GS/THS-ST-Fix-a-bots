using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LoToolMinigame : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textCounter;

    private List<int> numberArray = new List<int>();

    [SerializeField]
    private GameObject hitPrefab;

    [SerializeField]
    private Transform spawnPoint;

    private List<int> nextAnswers = new List<int>();

    private PatternGameManager patternGameManager = new PatternGameManager();

    private List<int> generatedList = new List<int>();

    private int currentInt;

    private List<int> numberToDisplay = new List<int>();

    private int patternLength;

    private int slotToFill;

    private int difference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patternGameManager.GenerateValueForPattern();

        difference = patternGameManager.ReturnDifference();

        patternLength = Random.Range(6, 10);

        generatedList = patternGameManager.ReturnPatternArray(patternLength);

        currentInt = slotToFill = patternLength-3;

        Debug.Log("Current Index: " + currentInt);
        Debug.Log("Current Difference: " + difference);
        Debug.Log("Current Length: " + patternLength);
        Debug.Log("Current Array Length: " + generatedList.Count);

        for (int i=0; i<patternLength-3; i++)
        {
            numberToDisplay.Add(generatedList[i]);
            Debug.Log(i+" Value: " + numberToDisplay[i]);
        }

        for (int i = 0; i<3; i++)
        {
            numberToDisplay.Add(0);
            //Debug.Log(i + " Value: " + numberToDisplay[i]);
        }

        for (int i = patternLength - 3; i < patternLength; i++)
        {
            nextAnswers.Add(generatedList[i]);
            //Debug.Log(i + " answer: " + nextAnswers[]);
            //Debug.Log("Current value:" + i);
        }

        foreach(int number in nextAnswers)
        {
            Debug.Log("Answer Set:" + number);
        }
        
        textCounter.text = numberToDisplay[currentInt].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                HandleClickEvent();
            }
        }
        textCounter.text = numberToDisplay[currentInt].ToString();
    }

    private void HandleClickEvent()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if(rayHit.transform.name == "Tool")
            {
                numberToDisplay[currentInt]++;
                textCounter.text = numberToDisplay[currentInt].ToString();
            }
        }
    }

    public void CheckNumber()
    {
        int totalCorrect = 0 ;
        for(int i=0; i<3; i++)
        {
            if (numberToDisplay[slotToFill+i] != nextAnswers[i])
            {
                Debug.Log("Incorrect Number");
                numberToDisplay[slotToFill+i] = 0;
            }
            else
            {
                totalCorrect++;
                Debug.Log("Correct!!");
            }
        }

        if(totalCorrect == 3)
        {
            Debug.Log("All Correct!");
        }
        else
        {
            Debug.Log("Theres a mistake");
        }
    }

    public void ChangeToLeftElement()
    {
        if (currentInt>0)
        {
            currentInt--;
            textCounter.text = numberToDisplay[currentInt].ToString();
        }
    }

    public void ChangeToRightElement()
    {
        if (currentInt < numberToDisplay.Count-1)
        {
            currentInt++;
            textCounter.text = numberToDisplay[currentInt].ToString();
        }
    }
}
