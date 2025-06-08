using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private PatternGameManager patternGameManager;

    [SerializeField]
    private DifficultyManager toolDifficulty;

    [SerializeField]
    private Transform toolHolder;

    private List<int> nextAnswers = new List<int>();

    private List<int> generatedList = new List<int>();

    private GameObject tool;

    private int currentInt;

    private List<int> numberToDisplay = new List<int>();

    private int patternLength;

    private int slotToFill;

    private int slotToFix;

    private int difference;

    private GameObject[] tiledParts;

    private int[] fastenerValues;

    private GameObject currentTool;

    private Fastener[] fastenerList = new Fastener[4];

    private Vector3 originalPosition;

    private bool isFocused = true;

    void Awake()
    {
        fastenerList = FindObjectsByType<Fastener>(FindObjectsSortMode.None);
        int index;
        Fastener temp;

        for (int i=0; i<4-1; i ++)
        {
            index = i;

            for(int j = i+1; j<4; j++)
            {
                if (fastenerList[j].GetFastenerType()< fastenerList[index].GetFastenerType())
                {
                    index = j;
                }
            }

            temp = fastenerList[i];
            fastenerList[i] = fastenerList[index];
            fastenerList[index] = temp;
        }

        foreach (Fastener fastener in fastenerList)
        {
            Debug.Log("Fastener ID: " + fastener.GetFastenerType());
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int valueToFollow;

        int medValue;

        int randomValue = 1;

        difference = patternGameManager.ReturnDifference();

        patternLength = toolDifficulty.GetLengthOfPattern();

        fastenerObj = new GameObject[patternLength];

        fastenerValues = new int[patternLength];

        generatedList = patternGameManager.ReturnPatternArray(patternLength);

        if(toolDifficulty.GetNumberOfMissingVal() == 0)
        {
            slotToFill = 0;
            slotToFix = toolDifficulty.GetNumberOfIncorrectVal();
            Debug.Log("Fixing");
        }
        else if(toolDifficulty.GetNumberOfIncorrectVal() == 0)
        {
            slotToFix = 0;
            slotToFill = toolDifficulty.GetNumberOfMissingVal();
            Debug.Log("Filling");
        }
        else
        {
            //Randomize between missing and incorrect value where:
            //  0 = incorrect value
            //  1 = missing value
            medValue = Random.Range(1, 10);
            Debug.Log("Choosing: " + medValue);
            if (medValue <= 5)
            {
                slotToFill = 0;
                slotToFix = toolDifficulty.GetNumberOfIncorrectVal();
                Debug.Log("Method to follow: fix");
            }
            else
            {
                slotToFix = 0;
                slotToFill = toolDifficulty.GetNumberOfMissingVal();
                Debug.Log("Method to follow: fill");
            }
        }

        currentInt = 0;

        toolTilingManager.SpawnPartTiled(patternLength);

        tiledParts = toolTilingManager.GetTileList();

        if (slotToFill!=0)
        {
            for (int i = 0; i < patternLength - slotToFill; i++)
            {
                numberToDisplay.Add(generatedList[i]);
                Debug.Log(i + " Value: " + numberToDisplay[i]);
            }

            for (int i = 0; i < slotToFill; i++)
            {
                numberToDisplay.Add(0);
            }

            for (int i = patternLength - slotToFill; i < patternLength; i++)
            {
                nextAnswers.Add(generatedList[i]);
            }

            foreach (int number in nextAnswers)
            {
                Debug.Log("Answer Set:" + number);
            }

            valueToFollow = patternLength - slotToFill;
        }
        else
        {
            numberToDisplay = generatedList;

            for (int i=0; i < slotToFix; i++)
            {
                randomValue = Random.Range(0, patternLength);

                while (nextAnswers.Contains(randomValue))
                {
                    randomValue = Random.Range(0, patternLength);
                }

                numberToDisplay[randomValue] = numberToDisplay[randomValue] - difference;
            }

            valueToFollow = patternLength;
            
        }
        
        textCounter.text = numberToDisplay[currentInt].ToString();

        for (int i = 0; i < patternLength; i++)
        {
            Debug.Log("Creating Object number " + i);
            fastenerObj[i] = new GameObject("Fastener " + (i + 1).ToString());
            fastenerObj[i].transform.position = new Vector2(tiledParts[i].transform.position.x, hitCountManager.transform.position.y);
            
            Debug.Log(i+ " value: " + numberToDisplay[i]);
            if (numberToDisplay[i] > 0)
            {
                hitCountManager.presetCounter(numberToDisplay[i], fastenerObj[i], fastenerList[0].GetHitIcon());
            }

            if (i != currentInt)
            {
                fastenerObj[i].SetActive(false);
            }
        }

        for (int i = 0; i< valueToFollow; i++)
        {
            Instantiate(fastenerList[0].GetFastenerSprite(), tiledParts[i].GetComponent<PartTile>().GetFastenerPosition());
            fastenerValues[i] = 1;
        }

        originalPosition = Camera.main.transform.position;
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

        if (isFocused)
        {
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
    }

    private void HandleClickEvent()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if(rayHit.transform.gameObject.TryGetComponent(out Tool tool))
            {
                if (numberToDisplay[currentInt]<24)
                {
                    numberToDisplay[currentInt]++;
                    hitCountManager.increaseChildCount(fastenerObj[currentInt], fastenerList[0].GetHitIcon());
                }
                else
                {
                    numberToDisplay[currentInt]++;
                    textCounter.text = numberToDisplay[currentInt].ToString();
                }
            }
            else if(rayHit.transform.gameObject.TryGetComponent(out PartTile roboPart))
            {
                Camera.main.GetComponent<ToolCamera>().FocusedCameraView(roboPart.transform.position.x);

                for (int i=0; i< tiledParts.Length; i++)
                {
                    tiledParts[i].layer = LayerMask.NameToLayer("Ignore Raycast");
                    if(roboPart.gameObject == tiledParts[i])
                    {
                        currentInt = i;
                    }
                }

                fastenerObj[currentInt].SetActive(true);

                isFocused = true;
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

            toolHolder.position = new Vector3(fastenerObj[currentInt].transform.position.x, toolHolder.position.y, toolHolder.position.z);

            if (currentTool != null)
            {
                Destroy(currentTool);
            }
        }
    }

    public void ChangeToRightElement()
    {
        if (currentInt < numberToDisplay.Count-1)
        {
            fastenerObj[currentInt].SetActive(false);
            currentInt++;
            textCounter.text = numberToDisplay[currentInt].ToString();
            fastenerObj[currentInt].SetActive(true);
            Camera.main.transform.position = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

            toolHolder.position = new Vector3(fastenerObj[currentInt].transform.position.x, toolHolder.position.y, toolHolder.position.z);

            if(currentTool != null)
            {
                Destroy(currentTool);
            }
        }
    }

    public void SelectFastener(Button fastenerBtn)
    {
        Transform holder = null;
        if (currentInt>=slotToFill)
        {
            Debug.Log(fastenerBtn.GetComponent<Fastener>().GetFastenerType());
            fastenerValues[currentInt] = fastenerBtn.GetComponent<Fastener>().GetFastenerType();

            holder = tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition();

            Instantiate(fastenerBtn.GetComponent<Fastener>().GetFastenerSprite(), holder);
        }
    }

    public void SelectTool(Button toolBtn)
    {
        if (toolBtn.GetComponent<ToolBtn>().GetToolType() == fastenerValues[currentInt])
        {
            if (slotToFill == 0)
            {
                Debug.Log(toolBtn.GetComponent<ToolBtn>().GetToolType() + " can fix this");

                currentTool = Instantiate(toolBtn.GetComponent<ToolBtn>().GetToolSprite(), toolHolder);
            }
            else
            {
                if (currentInt >= patternLength - slotToFill)
                {
                    Debug.Log(toolBtn.GetComponent<ToolBtn>().GetToolType() + " to fill in");

                    currentTool = Instantiate(toolBtn.GetComponent<ToolBtn>().GetToolSprite(), toolHolder);
                }
            }

        }
    }

    public void OverheadView()
    {
        fastenerObj[currentInt].SetActive(false);
        currentInt = -1;
        originalPosition = Camera.main.transform.position;

        foreach (GameObject part in tiledParts)
        {
            part.layer = LayerMask.NameToLayer("Default");
        }

        isFocused = false;
    }
}
