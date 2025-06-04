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
    private HitCountManager hitCountManager;

    [SerializeField]
    private GameObject[] fastenerObj;

    [SerializeField]
    private ToolTilingManager toolTilingManager;

    private List<int> nextAnswers = new List<int>();

    private PatternGameManager patternGameManager = new PatternGameManager();

    private List<int> generatedList = new List<int>();

    public float speed;

    private GameObject tool;

    private int currentInt;

    private List<int> numberToDisplay = new List<int>();

    private int patternLength;

    private int slotToFill;

    private int difference;

    private GameObject[] tiledParts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patternGameManager.GenerateValueForPattern();

        difference = patternGameManager.ReturnDifference();

        patternLength = Random.Range(6, 10);

        fastenerObj = new GameObject[patternLength];

        generatedList = patternGameManager.ReturnPatternArray(patternLength);

        slotToFill = patternLength-3;

        currentInt = 0;

        toolTilingManager.SpawnPartTiled(patternLength);

        tiledParts = toolTilingManager.GetTileList();

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

        for (int i = 0; i < patternLength; i++)
        {
            fastenerObj[i] = new GameObject("Fastener " + (i + 1).ToString());
            fastenerObj[i].transform.position = new Vector2(tiledParts[i].transform.position.x, hitCountManager.transform.position.y);
            hitCountManager.presetCounter(numberToDisplay[i], fastenerObj[i]);

            if (i != currentInt)
            {
                fastenerObj[i].SetActive(false);
            }
        }
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

        if (numberToDisplay[currentInt] > 24)
        {
            fastenerObj[currentInt].SetActive(false);
            textCounter.gameObject.SetActive(true);
        }
        else
        {
            textCounter.gameObject.SetActive(false);
            fastenerObj[currentInt].SetActive(true);
        }
    }

    private void HandleClickEvent()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if(rayHit.transform.name == "Tool")
            {
                if (numberToDisplay[currentInt]<24)
                {
                    numberToDisplay[currentInt]++;
                    hitCountManager.increaseChildCount(fastenerObj[currentInt]);
                }
                else
                {
                    numberToDisplay[currentInt]++;
                    textCounter.text = numberToDisplay[currentInt].ToString();
                }
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
            fastenerObj[currentInt].SetActive(false);
            currentInt--;
            textCounter.text = numberToDisplay[currentInt].ToString();
            fastenerObj[currentInt].SetActive(true);
            
            Camera.main.transform.position = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
    }

    public void ChangeToRightElement()
    {
        Vector3 newCameraPos;
        bool isMoving = false;
        if (currentInt < numberToDisplay.Count-1)
        {
            fastenerObj[currentInt].SetActive(false);
            currentInt++;
            textCounter.text = numberToDisplay[currentInt].ToString();
            fastenerObj[currentInt].SetActive(true);
            Camera.main.transform.position = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
    }
}
