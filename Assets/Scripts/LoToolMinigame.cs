using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LoToolMinigame : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textCounter;

    [SerializeField]
    private List<int> numberArray = new List<int>();

    private List<int> numberToCheck = new List<int>();

    private int currentInt;

    private int patternLength;

    private int difference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInt = numberArray.Count;

        patternLength = numberArray.Count;

        for (int i = 0; i<3; i++)
        {
            numberArray.Add(0);
        }

        difference = numberArray[currentInt - 1] - numberArray[currentInt - 2];

        for (int i = 0; i < 3; i++)
        {
            numberToCheck.Add(numberArray[currentInt - 1] + difference*(i+1));
            Debug.Log(numberToCheck[i]);
        }

        Debug.Log(currentInt);
        Debug.Log(numberToCheck);
        Debug.Log(numberArray[currentInt-1] + ", " + numberArray[currentInt]);
        textCounter.text = numberArray[currentInt].ToString();
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
    }

    private void HandleClickEvent()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if(rayHit.transform.name == "Tool")
            {
                numberArray[currentInt]++;
                textCounter.text = numberArray[currentInt].ToString();
            }
        }
    }

    public void CheckNumber()
    {
        int totalCorrect = 0 ;
        for(int i=0; i<3; i++)
        {
            if (numberArray[patternLength+i] != numberToCheck[i])
            {
                Debug.Log("Incorrect Number");
                numberArray[patternLength+i] = 0;
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
            textCounter.text = numberArray[currentInt].ToString();
        }
    }

    public void ChangeToRightElement()
    {
        if (currentInt < numberArray.Count-1)
        {
            currentInt++;
            textCounter.text = numberArray[currentInt].ToString();
        }
    }
}
