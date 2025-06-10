using System.Collections;
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

    [SerializeField]
    private float speed;

    private List<int> nextAnswers = new List<int>();

    private List<int> generatedList = new List<int>();

    private GameObject tool;

    private int currentInt;

    private int[] numberToDisplay;

    private int patternLength;

    private int slotToFill;

    private int slotToFix;

    private int difference;

    private GameObject[] tiledParts;

    private int[] fastenerValues;

    private int[] originalHitValues;

    private GameObject currentTool;

    private Fastener[] fastenerList = new Fastener[4];

    private Vector3 originalPosition;

    private bool isFocused = true;

    private Vector3 newCameraPos;

    private bool isMoving = false;

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

        originalHitValues = new int[patternLength];

        numberToDisplay = new int[patternLength];

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
                numberToDisplay[i] = generatedList[i];
                Debug.Log(i + " Value: " + numberToDisplay[i]);
            }

            for (int i = patternLength - slotToFill; i < patternLength; i++)
            {
                numberToDisplay[i] = 0;
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
            numberToDisplay = generatedList.ToArray();

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

        originalHitValues = numberToDisplay;


        textCounter.text = numberToDisplay[currentInt].ToString();

        for (int i = 0; i < patternLength; i++)
        {
            Debug.Log("Creating Object number " + i);
            fastenerObj[i] = new GameObject("Fastener " + (i + 1).ToString());
            fastenerObj[i].transform.position = new Vector2(tiledParts[i].transform.position.x, hitCountManager.transform.position.y);
            
            Debug.Log(i+ " value: " + numberToDisplay[i]);
            if (numberToDisplay[i] > 0)
            {
                hitCountManager.PresetCounter(numberToDisplay[i], fastenerObj[i], fastenerList[0].GetHitIcon());
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

        if (isMoving)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, newCameraPos, speed * Time.deltaTime);
        }

        if (Vector3.Distance(Camera.main.transform.position, newCameraPos)<0.001f && isMoving)
        {
            fastenerObj[currentInt].SetActive(true);
            isMoving = false;
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
                    hitCountManager.IncreaseChildCount(fastenerObj[currentInt], fastenerList[0].GetHitIcon());
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
        newCameraPos = new Vector3(fastenerObj[0].transform.position.x, 0, Camera.main.transform.position.z);

        StartCoroutine(ValueCheckCoroutine());
    }

    public void ChangeToLeftElement()
    {
        if (currentInt>0)
        {
            fastenerObj[currentInt].SetActive(false);
            currentInt--;
            textCounter.text = numberToDisplay[currentInt].ToString();
            //fastenerObj[currentInt].SetActive(true);
            //Camera.main.transform.position = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

            newCameraPos = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

            isMoving = true;

            toolHolder.position = new Vector3(fastenerObj[currentInt].transform.position.x, toolHolder.position.y, toolHolder.position.z);

            if (currentTool != null)
            {
                Destroy(currentTool);
            }
        }
    }

    public void ChangeToRightElement()
    {
        if (currentInt < numberToDisplay.Length-1)
        {
            fastenerObj[currentInt].SetActive(false);
            currentInt++;
            textCounter.text = numberToDisplay[currentInt].ToString();
            //fastenerObj[currentInt].SetActive(true);
            //Camera.main.transform.position = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

            newCameraPos = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

            isMoving = true;

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
        int toolUsed = toolBtn.GetComponent<ToolBtn>().GetToolType();
        int currentFastenerVal = fastenerValues[currentInt];
        Debug.Log("Tool Chosen: " + toolUsed);
        Debug.Log("Fastener in place: " + currentFastenerVal);

        if (toolUsed == currentFastenerVal)
        {
            currentTool = Instantiate(toolBtn.GetComponent<ToolBtn>().GetToolSprite(), toolHolder);

            Debug.Log("What is happening???");
        }
        else
        {
            Debug.Log("Can't use tool T_T");
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

    public void UndoHitCounts()
    {

        numberToDisplay[currentInt] = originalHitValues[currentInt];

        Debug.Log("Undo Value: " + originalHitValues[currentInt]);

        StartCoroutine(ResetHitCoroutine());
    }

    private IEnumerator ResetHitCoroutine()
    {
        foreach (Transform child in fastenerObj[currentInt].transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        yield return null;

        Debug.Log("Current Child Count before reset: " + fastenerObj[currentInt].transform.childCount);

        Debug.Log(fastenerObj[currentInt].transform.position);

        if (numberToDisplay[currentInt] < 24)
        {
            hitCountManager.PresetCounter(numberToDisplay[currentInt], fastenerObj[currentInt], fastenerList[0].GetHitIcon());
        }
        else
        {
            textCounter.text = numberToDisplay[currentInt].ToString();
        }
    }

    private IEnumerator ValueCheckCoroutine()
    {
        int totalCorrect = 0;
        int i = 0;

        Camera.main.GetComponent<ToolCamera>().CameraTrigger(newCameraPos, speed);
        yield return null;

        while (i<patternLength)
        {
            fastenerObj[i].SetActive(true);

            if (numberToDisplay[i] != generatedList[i])
            {
                Debug.Log("Incorrect Number");
            }
            else
            {
                totalCorrect++;
                Debug.Log("Correct!!");
            }

            yield return new WaitForSeconds(2);

            fastenerObj[i].SetActive(false);

            i++;

            if (i < patternLength)
            {
                newCameraPos = new Vector3(fastenerObj[i].transform.position.x, 0, Camera.main.transform.position.z);

                yield return null;

                Debug.Log(Camera.main.transform.name);

                Camera.main.GetComponent<ToolCamera>().SubmitCameraMovement(newCameraPos, speed);

                yield return null;
            }
        }
    }
}
