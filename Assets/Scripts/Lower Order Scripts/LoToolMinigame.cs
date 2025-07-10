using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoToolMinigame : MonoBehaviour
{
    [Header("Minigame Managers")]

    [SerializeField]
    private PatternGameManager patternGameManager;

    [SerializeField]
    private HitCountManager hitCountManager;

    [SerializeField]
    private ToolTilingManager toolTilingManager;

    [SerializeField]
    private DifficultyManager toolDifficulty;

    [Header("Tool Objects")]

    [SerializeField]
    private TextMeshProUGUI textCounter;

    [SerializeField]
    private GameObject[] fastenerObj;

    [SerializeField]
    private Transform toolHolder;

    [SerializeField]
    private float speed;

    

    private GameObject[] counterHolder;
    private List<int> generatedList = new List<int>();
    private int currentInt;
    private int[] numberToDisplay;
    private int patternLength;
    private int slotToFill = 0;
    private int slotToFix = 0;
    private GameObject[] tiledParts;
    private int[] fastenerValues;
    private int[] originalHitValues;
    private GameObject currentTool;
    private FastenerBtn[] fastenerList = new FastenerBtn[4];
    private int[] fastenerCheckVal;

    [SerializeField]
    private List<ToolBtn> toolButtons;
    

    void Awake()
    {
        fastenerList = FindObjectsByType<FastenerBtn>(FindObjectsSortMode.None);
        int index;
        FastenerBtn temp;

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

        foreach (FastenerBtn fastener in fastenerList)
        {
            Debug.Log("Fastener ID: " + fastener.GetFastenerType());
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int medValue;
        int randomValue;
        int randFastenerVal;
        bool isFix=false;
        int[] tempArr;
        GameObject originalCounter = FindFirstObjectByType<OverviewCounter>().gameObject;

        int difference = patternGameManager.ReturnDifference();
        patternLength = toolDifficulty.GetLengthOfPattern();
        fastenerObj = new GameObject[patternLength];
        fastenerValues = new int[patternLength];
        originalHitValues = new int[patternLength];
        numberToDisplay = new int[patternLength];
        counterHolder = new GameObject[patternLength];
        fastenerCheckVal = new int[patternLength];

        generatedList = patternGameManager.ReturnPatternArray(patternLength);

        Debug.Log(toolDifficulty.GetDifficulty());

        //Checks for minigame's difficulty
        switch (toolDifficulty.GetDifficulty())
        {
            case "easy":
                slotToFix = toolDifficulty.GetNumberOfIncorrectVal();
                isFix = true;
                Debug.Log("Fixing in " + toolDifficulty.GetDifficulty());
                break;
            case "medium":
                //Randomize between missing and incorrect value where:
                //  0 = incorrect value
                //  1 = missing value
                medValue = Random.Range(1, 10);
                Debug.Log("Choosing: " + medValue);
                if (medValue <= 5)
                {
                    slotToFill = 0;
                    slotToFix = toolDifficulty.GetNumberOfIncorrectVal();
                    isFix= true;
                    Debug.Log("Method to follow: fix");
                }
                else
                {
                    slotToFix = 0;
                    slotToFill = toolDifficulty.GetNumberOfMissingVal();
                    isFix = false;
                    Debug.Log("Method to follow: fill");
                }
                break;
            case "hard":
                slotToFix = 0;
                slotToFill = toolDifficulty.GetNumberOfMissingVal();
                isFix = false;
                Debug.Log("Filling");
                break;
            default:
                Debug.Log("Playing tutorial code");
                break;
        }
        
        currentInt = 0;

        toolTilingManager.SpawnPartTiled(patternLength);

        tiledParts = toolTilingManager.GetTileList();
        switch (isFix)
        {
            case true:
                numberToDisplay = generatedList.ToArray();

                tempArr = new int[slotToFix];

                for (int i = 0; i < slotToFix; i++)
                {
                    randomValue = Random.Range(0, patternLength);

                    while (tempArr.Contains(randomValue))
                    {
                        randomValue = Random.Range(0, patternLength);
                    }

                    numberToDisplay[randomValue] = numberToDisplay[randomValue] - difference;

                    tempArr[i] = randomValue;
                }
                break;
            case false: 
                if(toolDifficulty.GetDifficulty() == "hard")
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
                }
                else
                {
                    numberToDisplay = generatedList.ToArray();

                    tempArr = new int[slotToFill];

                    for (int i = 0; i < slotToFill; i++)
                    {
                        randomValue = Random.Range(0, patternLength);

                        while (tempArr.Contains(randomValue))
                        {
                            randomValue = Random.Range(0, patternLength);
                        }

                        numberToDisplay[randomValue] = 0;

                        tempArr[i] = randomValue;
                    }
                }
                break;
        }

        for(int i=0; i<patternLength; i++)
        {
            originalHitValues[i] = numberToDisplay[i];
        }

        textCounter.text = numberToDisplay[currentInt].ToString();

        //randFastenerVal = Random.Range(0, fastenerList.Length);
        randFastenerVal = 0;

        for (int i = 0; i < patternLength; i++)
        {
            Debug.Log("Creating Object number " + i);
            fastenerObj[i] = new GameObject("Fastener " + (i + 1).ToString());
            fastenerObj[i].transform.position = new Vector2(tiledParts[i].transform.position.x, hitCountManager.transform.position.y);
            
            Debug.Log(i+ " value: " + numberToDisplay[i]);
            if (numberToDisplay[i] > 0)
            {
                hitCountManager.PresetCounter(numberToDisplay[i], fastenerObj[i], fastenerList[randFastenerVal].GetHitIcon());
            }

            if (i != currentInt)
            {
                fastenerObj[i].SetActive(false);
            }
        }

        for (int i = 0; i< patternLength; i++)
        {
            if (numberToDisplay[i] > 0)
            {
                Instantiate(fastenerList[randFastenerVal].GetFastenerSprite(), tiledParts[i].GetComponent<PartTile>().GetFastenerPosition());
                tiledParts[i].GetComponent<PartTile>().SetFastenerPosition(-0.7f);


                Debug.Log("Adding Fastener");
                fastenerValues[i] = randFastenerVal+1;
            }

            fastenerCheckVal[i] = randFastenerVal+1;
        }

        for(int i = 0; i<patternLength; i++)
        {
            Vector3 position = new Vector3(tiledParts[i].transform.position.x, originalCounter.transform.position.y, originalCounter.transform.position.z);
            counterHolder[i] = Instantiate(originalCounter);

            counterHolder[i].transform.position = position;

            counterHolder[i].GetComponent<OverviewCounter>().SetCounterVal(numberToDisplay[i], fastenerList[fastenerCheckVal[i] - 1].GetHitIcon());
        }

        Destroy(originalCounter);

        Camera.main.GetComponent<ToolCamera>().OverheadCameraView();
        OverheadView();
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
            if(rayHit.transform.gameObject.TryGetComponent(out Tool tool))
            {
                bool useCountManager = false;
                numberToDisplay[currentInt]++;
                if (numberToDisplay[currentInt]<24)
                {
                    fastenerObj[currentInt].SetActive(true);
                    textCounter.gameObject.SetActive(false);
                    //hitCountManager.IncreaseChildCount(fastenerObj[currentInt], fastenerList[0].GetHitIcon());
                    useCountManager = true;
                }
                else
                {
                    fastenerObj[currentInt].SetActive(false);
                    textCounter.gameObject.SetActive(true);
                    useCountManager = false;
                }

                StartCoroutine(HitAnimation(tool, useCountManager));
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
                    ToggleOverviewCounters(false);
                }

                fastenerObj[currentInt].SetActive(true);

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
    }

    private IEnumerator HitAnimation(Tool tool, bool value)
    {
        if (value)
        {
            hitCountManager.IncreaseChildCount(fastenerObj[currentInt], fastenerList[0].GetHitIcon());
        }
        else
        {
            textCounter.text = numberToDisplay[currentInt].ToString();
        }
        yield return StartCoroutine(tool.TriggerToolAnimation(tiledParts[currentInt].GetComponent<PartTile>()));
    }


    public void CheckNumber(Transform tools)
    {
        ToggleOverviewCounters(false);

        foreach (Transform child in tools)
        {
            if(child.GetComponent<ToolBtn>().GetToolType() == 1)
            {
                currentTool = Instantiate(child.GetComponent<ToolBtn>().GetToolSprite(), toolHolder);
                break;
            }
        }

        StartCoroutine(ValueCheckCoroutine());    
    }

    public void ChangeToLeftElement(Button button)
    {
        if (currentInt>0)
        {
            
            fastenerObj[currentInt].SetActive(false);
            currentInt--;
            textCounter.text = numberToDisplay[currentInt].ToString();
            
            StartCoroutine(TriggerFastenerChange(button));

            if (currentTool != null)
            {
                Destroy(currentTool);
            }
        }
    }

    public void ChangeToRightElement(Button button)
    {
        if (currentInt < numberToDisplay.Length-1)
        {
            fastenerObj[currentInt].SetActive(false);
            currentInt++;
            textCounter.text = numberToDisplay[currentInt].ToString();
            
            StartCoroutine(TriggerFastenerChange(button));

            if(currentTool != null)
            {
                Destroy(currentTool);
            }
        }
    }

    public void SelectFastener(Button fastenerBtn)
    {
        Transform holder = null;
        Debug.Log(fastenerBtn.GetComponent<FastenerBtn>().GetFastenerType());
        fastenerValues[currentInt] = fastenerBtn.GetComponent<FastenerBtn>().GetFastenerType();

        holder = tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition();

        if (holder.childCount > 0)
        {
            Destroy(holder.transform.GetChild(0).gameObject);
        }

        Instantiate(fastenerBtn.GetComponent<FastenerBtn>().GetFastenerSprite(), holder);

        numberToDisplay[currentInt] = 0;
        foreach(Transform child in fastenerObj[currentInt].transform) 
        {
            Destroy(child.gameObject);    
        }
    }

    public void SelectTool(Button toolBtn)
    {
        ToolBtn selectedToolBtn = toolBtn.GetComponent<ToolBtn>();
        int toolUsed = toolBtn.GetComponent<ToolBtn>().GetToolType();
        int currentFastenerVal = fastenerValues[currentInt];
        Debug.Log("Tool Chosen: " + toolUsed);
        Debug.Log("Fastener in place: " + currentFastenerVal);
        toolHolder.position = new Vector3(fastenerObj[currentInt].transform.position.x, toolHolder.position.y, toolHolder.position.z);


        //remove other visible tools from toolHolder
        

        //unselect all buttons but this one 


        if (toolUsed == currentFastenerVal)
        {
            if(currentTool != null)
            {
                Destroy(currentTool);
            }

            currentTool = Instantiate(toolBtn.GetComponent<ToolBtn>().GetToolSprite(), toolHolder);
            currentTool.GetComponent<Tool>().SetHeightValue(-0.7f);

            toolBtn.GetComponent<ToolBtn>().Select();

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

        foreach (GameObject part in tiledParts)
        {
            part.layer = LayerMask.NameToLayer("Default");
        }

        ToggleOverviewCounters(true);

        if (currentTool != null)
        {
            Destroy(currentTool);
        }
    }

    public void UndoHitCounts()
    {
        Transform holder = tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition();

        if (currentInt < patternLength - slotToFill)
        {
            numberToDisplay[currentInt] = originalHitValues[currentInt];

            fastenerValues[currentInt] = fastenerCheckVal[currentInt];

            if (holder.childCount > 0)
            {
                Destroy(holder.transform.GetChild(0).gameObject);
            }

            Instantiate(fastenerList[fastenerValues[currentInt]-1].GetFastenerSprite(), holder);
            tiledParts[currentInt].GetComponent<PartTile>().SetFastenerPosition(-0.7f);

            Debug.Log("Undo Value: " + originalHitValues[currentInt]);

            StartCoroutine(ResetHitCoroutine());
        }
        else
        {
            foreach (Transform child in fastenerObj[currentInt].transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            if (holder.childCount > 0)
            {
                Destroy(holder.transform.GetChild(0).gameObject);
            }
        }
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

        Vector3 newCameraPos = new Vector3(fastenerObj[0].transform.position.x, 0, Camera.main.transform.position.z);

        Camera.main.GetComponent<ToolCamera>().CameraTrigger(newCameraPos, speed);
        yield return null;

        currentTool.GetComponent<Tool>().SetHeightValue(-1f);

        while (i<patternLength)
        {
            fastenerObj[i].SetActive(true);

            toolHolder.position = new Vector3(fastenerObj[i].transform.position.x, toolHolder.position.y, toolHolder.position.z);

            if (numberToDisplay[i] != generatedList[i] || fastenerCheckVal[i] != fastenerValues[i])
            {
                tiledParts[i].GetComponent<PartTile>().SetIsWrong(false);
                Debug.Log("Incorrect!!");
            }
            else
            {
                totalCorrect++;
                tiledParts[i].GetComponent<PartTile>().SetIsWrong(true);
                Debug.Log("Correct!!");
            }

            yield return StartCoroutine(currentTool.GetComponent<Tool>().TriggerToolAnimation(tiledParts[i].GetComponent<PartTile>()));

            yield return new WaitForSeconds(1.5f);

            fastenerObj[i].SetActive(false);

            i++;

            if (i < patternLength)
            {
                newCameraPos = new(fastenerObj[i].transform.position.x, 0, Camera.main.transform.position.z);

                yield return null;

                Debug.Log(Camera.main.transform.name);

                StartCoroutine(Camera.main.GetComponent<ToolCamera>().SubmitCameraMovement(newCameraPos, speed));

                yield return null;
            }
        }

        yield return null;

        Camera.main.GetComponent<ToolCamera>().OverheadCameraView();

        yield return null;

        ToggleOverviewCounters(true);

        Destroy(currentTool);

        if (totalCorrect == patternLength)
        {
            Debug.Log("All correct!");

            Camera.main.GetComponent<ToolCamera>().TriggerDoneCanvas();

            StaticData.isToolDone = true;

            if (DataPersistenceManager.Instance != null)
            {
                DataPersistenceManager.Instance.SaveGame();
                Debug.Log("Tool station completion saved to StaticData.");
            }
        }
        else
        {
            Debug.Log("Something is wrong!");
            foreach (GameObject partTile in tiledParts) {
                partTile.GetComponent<PartTile>().GetFastenerPosition().GetComponentInChildren<Fastener>().SetFixedSprite();
                partTile.GetComponent<PartTile>().SetIsWrong(false);
                partTile.GetComponent<PartTile>().SetFastenerPosition(-0.7f);

            }
        }
    }

    private IEnumerator TriggerFastenerChange(Button button)
    {
        Vector3 newCameraPos = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        button.interactable = false;
        yield return null;
        StartCoroutine(Camera.main.GetComponent<ToolCamera>().SubmitCameraMovement(newCameraPos, speed));
        yield return null;
        fastenerObj[currentInt].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        button.interactable = true;
    }

    //toggles the view of the overview counter
    private void ToggleOverviewCounters(bool isShowing)
    {
        if (isShowing)
        {
            for (int i = 0; i < patternLength; i++)
            {
                if(fastenerValues[i] > 0)
                {
                    counterHolder[i].GetComponent<OverviewCounter>().SetCounterVal(numberToDisplay[i], fastenerList[fastenerValues[i] - 1].GetHitIcon());
                }
                else
                {
                    counterHolder[i].GetComponent<OverviewCounter>().SetCounterVal(numberToDisplay[i], fastenerList[fastenerCheckVal[i] - 1].GetHitIcon());
                }
                
                counterHolder[i].SetActive(true);
            }
        }
        else
        {
            foreach(GameObject count in counterHolder)
            {
                count.SetActive(false);
            }
        }
    }
}
