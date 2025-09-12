using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoToolRevised : MonoBehaviour
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
    private GameObject[] fastenerObj;

    [SerializeField]
    private Transform toolHolder;

    [SerializeField]
    private float speed;

    private GameObject[] counterHolder;
    private GameObject[] gapHolder;
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
    private int[] gapToDisplay;
    private int[] originalGaps;

    private Vector3 offset;
    private Vector3 distance;
    private bool isDragging = false;
    private Transform draggingObj;

    //private Vector3[] spawn_points;

    private int focusPoint;

    private float prevPos;

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
        //GameObject originalCounter = FindFirstObjectByType<OverviewCounter>().gameObject;

        //GameObject originalGap = FindFirstObjectByType<GapHolder>().gameObject;

        //int difference = StaticData.sequenceDiff;
        int difference = patternGameManager.ReturnDifference();
        //patternLength = StaticData.patternLength;
        patternLength = toolDifficulty.GetLengthOfPattern();
        fastenerObj = new GameObject[patternLength];
        fastenerValues = new int[patternLength];
        originalHitValues = new int[patternLength];
        numberToDisplay = new int[patternLength];
        counterHolder = new GameObject[patternLength];
        fastenerCheckVal = new int[patternLength];

        Debug.Log("patternLength: " + patternLength);
        Debug.Log("numberToDisplay.Length: " + (numberToDisplay?.Length ?? -1));
        Debug.Log("generatedList.Count: " + (generatedList?.Count ?? -1));

        patternLength = numberToDisplay.Length; // Ensure consistency
        gapToDisplay = new int[patternLength - 1];
        originalGaps = new int[patternLength - 1];

        

        gapHolder = new GameObject[patternLength - 1];

        //generatedList = StaticData.toolPattern;
        generatedList = patternGameManager.ReturnPatternArray(patternLength);

        if (toolDifficulty.GetDifficulty() == "easy")
        {
            Debug.Log("easy");
        }
        else if (toolDifficulty.GetDifficulty() == "medium")
        {
            Debug.Log("medium");
        }
        else if (toolDifficulty.GetDifficulty() == "hard")
        {
            Debug.Log("hard");
        }


        //Checks for minigame's difficulty
        switch (toolDifficulty.GetDifficulty())
        {
            case "easy":
                slotToFix = StaticData.incorrectVals;
                isFix = true;
                Debug.Log("Fixing in easy!");
                break;
            case "medium":
                Debug.Log("Fixing in medium!");

                medValue = StaticData.medValue;
                if (medValue <= 5)
                {
                    slotToFill = 0;
                    slotToFix = StaticData.incorrectVals;
                    isFix = true;
                    Debug.Log("Method to follow: fix");
                }
                else
                {
                    //slotToFix = 0;
                    slotToFix = StaticData.missingVals;
                    isFix = true;
                    Debug.Log("Method to follow: fill");
                }
                break;
            case "hard":
                /*
                Debug.Log("Fixing in hard!");
                slotToFix = 0;
                slotToFill = StaticData.missingVals;
                isFix = true;
                Debug.Log("Filling");
                */
                //slotToFix = 0;

                slotToFix = StaticData.missingVals;
                isFix = true;
                Debug.Log("Filling");

                break;
            default:
                Debug.Log("Playing tutorial code");
                break;
        }
        
        currentInt = 0;

        Debug.Log("Highest No. Count: " + generatedList[generatedList.Count - 1]);

        int tilesToSpawn = (generatedList[generatedList.Count - 1] / 2) + 1;

        toolTilingManager.SpawnPartTiled(tilesToSpawn);

        //spawn_points = toolTilingManager.GetFastenerPoints();

        tiledParts = toolTilingManager.GetTileList();
        switch (isFix)
        {
            case true:
                Debug.Log("Fixing!");
                numberToDisplay = generatedList.ToArray();

                difference = numberToDisplay[1] - numberToDisplay[0];
                Debug.Log("Diff val: " + difference);

                if (toolDifficulty.GetNumberOfIncorrectVal() == 0)
                {
                    /*
                    HashSet<int> usedIndices = new HashSet<int>();
                    while (StaticData.incorrectIndices.Count < slotToFix)
                    {
                        int randIndex = Random.Range(0, patternLength);
                        if (!usedIndices.Contains(randIndex))
                        {
                            usedIndices.Add(randIndex);
                            StaticData.incorrectIndices.Add(randIndex);
                            StaticData.incorrectValues.Add(generatedList[randIndex] - difference);
                        }
                    }
                    */
                }

                numberToDisplay = generatedList.ToArray();

                for (int i = 0; i < toolDifficulty.GetNumberOfIncorrectVal(); i++)
                {
                    //int index = StaticData.incorrectIndices[i];
                    //numberToDisplay[index] = StaticData.incorrectValues[i];
                }
                break;
            case false:
                if(toolDifficulty.GetDifficulty() == "hard")
                //Debug.Log("Filling!");
                //if(StaticData.toolDifficulty == 1 || StaticData.toolDifficulty == 2)
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

        randFastenerVal = Random.Range(0,3);

        /*
        for (int i = 0; i < patternLength; i++)
        {
            Debug.Log("Creating Object number " + i);
            fastenerObj[i] = new GameObject("Fastener " + (i + 1).ToString());
            fastenerObj[i].transform.position = new Vector2(spawn_points[numberToDisplay[i]].x, hitCountManager.transform.position.y);
            
            Debug.Log(i+ " value: " + numberToDisplay[i]);
            if (numberToDisplay[i] > 0)
            {
                Debug.Log($"Index i = {i}, fastenerCheckVal[i] = {fastenerCheckVal[i]}, fastenerList.Length = {fastenerList.Length}");
                hitCountManager.PresetCounter(numberToDisplay[i], fastenerObj[i], fastenerList[randFastenerVal].GetHitIcon());
            }

            if (i != currentInt)
            {
                fastenerObj[i].SetActive(false);
            }
        }
        */

        for (int i = 0; i< patternLength; i++)
        {
            if (numberToDisplay[i] > 0)
            {
                Instantiate(fastenerList[randFastenerVal].GetFastenerSprite(), toolTilingManager.SetFastener(numberToDisplay[i]));
                //tiledParts[i].GetComponent<PartTile>().SetFastenerPosition(-0.7f);


                Debug.Log("Adding Fastener");
                fastenerValues[i] = randFastenerVal+1;
            }

            fastenerCheckVal[i] = randFastenerVal+1;
        }

        toolTilingManager.SetCenterFocus(numberToDisplay[0]-1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject() || HandleUIClickEvent())
                {
                    HandleClickEvent();
                }
            } 
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                        isDragging = false;
                        draggingObj = null;
                        Debug.Log("Not scrolling");
                        toolTilingManager.IsNotDragged();
                }
            }
            else
            {
                if (isDragging)
                {
                    Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    bool atEdge = false;
                    /*
                    if (newPos.x - prevPos < 0f)
                    {
                        Debug.Log("Moving to left");

                        if (toolTilingManager.CheckPointPosition(numberToDisplay[0] - 1))
                        {
                            atEdge = true;
                        }
                    }
                    else
                    {
                        Debug.Log("Moving to right");
                    }
                    

                    if (toolTilingManager.CheckPointPosition(numberToDisplay[0] - 1).x<=0f && toolTilingManager.CheckPointPosition(numberToDisplay[numberToDisplay.Length - 1] - 1).x >=0f)
                    {
                        draggingObj.position = new Vector3(newPos.x + offset.x, draggingObj.position.y, draggingObj.position.z);
                        Debug.Log("Scrolling is Working");
                        prevPos = newPos.x;
                    }
                    else
                    {
                        draggingObj.position = new Vector3(0 + offset.x, draggingObj.position.y, draggingObj.position.z);
                    }
                    */
                    /**/
                    draggingObj.position = new Vector3(newPos.x + offset.x, draggingObj.position.y, draggingObj.position.z);
                    Debug.Log("Scrolling is Working");
                    prevPos = newPos.x;
                    

                }
            }
        }
        
    }

    private bool HandleUIClickEvent()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.GetTouch(0).position;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (var go in raycastResults)
            {
                if (go.gameObject.transform.root.TryGetComponent(out OverviewCounter overviewCounter))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void HandleClickEvent()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if(rayHit.transform.gameObject.TryGetComponent(out PartTile roboPart))
            {
                draggingObj = rayHit.transform.parent;
                isDragging = true;
                offset = draggingObj.position - Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                toolTilingManager.IsDragged();
                prevPos = rayHit.transform.position.x;
            }else if (rayHit.transform.gameObject.TryGetComponent(out RulerLines ruler))
            {
                Transform fastenerButtons = fastenerList[0].transform.parent;

                Debug.Log(int.Parse(ruler.GetComponentInChildren<TextMeshProUGUI>().text));

                focusPoint = int.Parse(ruler.GetComponentInChildren<TextMeshProUGUI>().text);

                toolTilingManager.SetCenterFocus(int.Parse(ruler.GetComponentInChildren<TextMeshProUGUI>().text) -1);

                fastenerButtons.parent.GetComponent<Canvas>().enabled = true;

                foreach(GameObject part in tiledParts)
                {
                    part.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }

    public void SetFastenerInPlace(Button fastenerBtn)
    {
        Transform buttonParent = fastenerBtn.transform.parent;
        Transform currentPoint = toolTilingManager.SetFastener(focusPoint);
        if (currentPoint.childCount>0)
        {
            foreach(Transform child in currentPoint)
            {
                Destroy(child.gameObject);
            }
        }

        Instantiate(fastenerBtn.GetComponent<FastenerBtn>().GetFastenerSprite(), currentPoint);

        buttonParent.parent.GetComponent<Canvas>().enabled = false;

        foreach (GameObject part in tiledParts)
        {
            part.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
