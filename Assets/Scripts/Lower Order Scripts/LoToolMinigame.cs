using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoToolMinigame : MonoBehaviour
{
    [Header("Minigame Managers")]

    //[SerializeField]
    //private PatternGameManager patternGameManager;

    [SerializeField]
    private HitCountManager hitCountManager;

    [SerializeField]
    private ToolTilingManager toolTilingManager;

    //[SerializeField]
    //private DifficultyManager toolDifficulty;

    [Header("Tool Objects")]

    [SerializeField]
    private Transform textCounter;

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
    [SerializeField]
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
    private bool isDragging = false;
    private bool isOnObject = false;

    private Vector2[] endPoints = new Vector2[2];

    private GameObject robotPart;

    private Vector3 offset;

    private bool showGaps = false;

    [SerializeField]
    private List<ToolBtn> toolButtons;

    [SerializeField]
    private Button addTenBtn;

    [SerializeField]
    private GameObject randMissing_Prefab;

    [SerializeField]
    private int max_HitCount;

    void Awake()
    {
        fastenerList = FindObjectsByType<FastenerBtn>(FindObjectsSortMode.None);
        int index;
        FastenerBtn temp;

        for (int i=0; i<4; i ++)
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

        robotPart = new GameObject("Complete Part");
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

        GameObject originalGap = FindFirstObjectByType<GapHolder>().gameObject;

        int difference = StaticData.sequenceDiff;
        //int difference = patternGameManager.ReturnDifference();
        patternLength = StaticData.toolpatternLength;
        //patternLength = toolDifficulty.GetLengthOfPattern();
        fastenerObj = new GameObject[patternLength];
        fastenerValues = new int[patternLength];
        originalHitValues = new int[patternLength];
        numberToDisplay = new int[patternLength];
        counterHolder = new GameObject[patternLength];
        fastenerCheckVal = new int[patternLength];

        Debug.Log("patternLength: " + StaticData.toolpatternLength);
        Debug.Log("numberToDisplay.Length: " + (numberToDisplay?.Length));
        Debug.Log("generatedList.Count: " + (generatedList?.Count));

        //patternLength = numberToDisplay.Length; // Ensure consistency
        gapToDisplay = new int[patternLength - 1];
        originalGaps = new int[patternLength - 1];

        gapHolder = new GameObject[patternLength - 1];

        generatedList = StaticData.toolPattern;
        //generatedList = patternGameManager.ReturnPatternArray(patternLength);

        Debug.Log("generatedList.Count: " + generatedList.Count);

        if (StaticData.toolDifficulty == 0)
        {
            Debug.Log("easy");
        }
        else if (StaticData.toolDifficulty == 1)
        {
            Debug.Log("medium");
        }
        else if (StaticData.toolDifficulty == 2)
        {
            Debug.Log("hard");
        }

        //Checks for minigame's difficulty
        switch (StaticData.toolDifficulty)
        {
            case 0:
                //slotToFix = StaticData.incorrectVals;
                slotToFix = 1;
                //isFix = true;
                Debug.Log("Fixing in easy!");
                break;
            case 1:
                Debug.Log("Fixing in medium!");

                /*
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
                */

                slotToFix = Random.Range(1,2);

                break;
            case 2:
                /*
                Debug.Log("Fixing in hard!");
                slotToFix = 0;
                slotToFill = StaticData.missingVals;
                isFix = true;
                Debug.Log("Filling");
                */
                slotToFix = 3;

                //slotToFix = StaticData.missingVals;
                isFix = true;
                Debug.Log("Filling");

                break;
            default:
                Debug.Log("Playing tutorial code");
                break;
        }
        
        currentInt = 0;

        toolTilingManager.SpawnPartTiled(patternLength);

        tiledParts = toolTilingManager.GetTileList();

        /*
        switch (isFix)
        {
            case true:
                Debug.Log("Fixing!");
                numberToDisplay = generatedList.ToArray();

                difference = numberToDisplay[1] - numberToDisplay[0];
                Debug.Log("Diff val: " + difference);

                if (StaticData.incorrectIndices.Count == 0)
                {
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
                }

                numberToDisplay = generatedList.ToArray();

                for (int i = 0; i < StaticData.incorrectIndices.Count; i++)
                {
                    int index = StaticData.incorrectIndices[i];
                    numberToDisplay[index] = StaticData.incorrectValues[i];
                }
                break;
            case false:
                //if(toolDifficulty.GetDifficulty() == "hard")
                Debug.Log("Filling!");
                if(StaticData.toolDifficulty == 1 || StaticData.toolDifficulty == 2)
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
        */

        for (int i = 0; i < patternLength - slotToFix; i++)
        {
            numberToDisplay[i] = generatedList[i];
            Debug.Log(i + " Value: " + numberToDisplay[i]);
        }

        for (int i = patternLength - slotToFix; i < patternLength; i++)
        {
            numberToDisplay[i] = 0;
        }

        for (int i=0; i<patternLength; i++)
        {
            originalHitValues[i] = numberToDisplay[i];
        }

        for(int i=0; i<patternLength-1; i++)
        {
            gapToDisplay[i] = numberToDisplay[i + 1] - numberToDisplay[i];
            originalGaps[i] = generatedList[i + 1] - generatedList[i];
        }

        //randFastenerVal = Random.Range(0, fastenerList.Length-1);
        randFastenerVal = StaticData.selectedFastenerIndex;
        /*
        if (!StaticData.selectedFastenerIndex.HasValue)
        {
            StaticData.selectedFastenerIndex = Random.Range(0, fastenerList.Length);

        }*/

        //randFastenerVal = StaticData.selectedFastenerIndex.Value;
        //randFastenerVal = 0;

        for (int i = 0; i < patternLength; i++)
        {
            Debug.Log("Creating Object number " + i);
            fastenerObj[i] = new GameObject("Fastener " + (i + 1).ToString());
            fastenerObj[i].transform.position = new Vector2(tiledParts[i].transform.position.x, hitCountManager.transform.position.y);
            
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

        for (int i = 0; i< patternLength; i++)
        {
            if (numberToDisplay[i] > 0)
            {
                Instantiate(fastenerList[randFastenerVal].GetFastenerSprite(), tiledParts[i].GetComponent<PartTile>().GetFastenerPosition());
                tiledParts[i].GetComponent<PartTile>().SetFastenerPosition(-0.7f);


                Debug.Log("Adding Fastener " + i);
                fastenerValues[i] = randFastenerVal+1;
            }
            else
            {
                if (StaticData.toolDifficulty == 0)
                {
                    Instantiate(fastenerList[randFastenerVal].GetMissingPrefab(), tiledParts[i].GetComponent<PartTile>().GetFastenerPosition());
                }else if (StaticData.toolDifficulty == 2 || StaticData.toolDifficulty == 1)
                {
                    Instantiate(randMissing_Prefab, tiledParts[i].GetComponent<PartTile>().GetFastenerPosition());
                }
                    

                Debug.Log("Adding Missing Fastener");
            }

            fastenerCheckVal[i] = randFastenerVal+1;
        }

        counterHolder[0] = originalCounter;

        for (int i = 0; i<patternLength; i++)
        {
            Vector3 position = new Vector3(tiledParts[i].transform.position.x, originalCounter.transform.position.y, originalCounter.transform.position.z);

            if (i > 0)
            {
                counterHolder[i] = Instantiate(originalCounter);

                counterHolder[i].transform.position = position;
            }
                

            counterHolder[i].GetComponent<OverviewCounter>().SetCounterVal(numberToDisplay[i], fastenerList[fastenerCheckVal[i] - 1].GetHitIcon());
        }

        gapHolder[0] = originalGap;

        for (int i = 0;i<patternLength-1; i++)
        {
            Vector3 position = new Vector3((tiledParts[i].transform.position.x + tiledParts[i+1].transform.position.x)/2, originalGap.transform.position.y, originalGap.transform.position.z);
            if (i > 0)
            {
                gapHolder[i] = Instantiate(originalGap);
            }

            gapHolder[i].transform.position = position;

            gapHolder[i].GetComponent<GapHolder>().SetGapVal(gapToDisplay[i], originalGaps[i]);
        }

        robotPart.transform.position = new Vector2(toolTilingManager.TileMidPoint(), tiledParts[0].transform.position.y);

        foreach (GameObject part in tiledParts)
        {
            part.transform.parent = robotPart.transform;
        }

        foreach (GameObject counter in counterHolder)
        {
            counter.transform.parent = robotPart.transform;
        }

        foreach (GameObject gap in gapHolder)
        {
            gap.transform.parent = robotPart.transform;
        }

        foreach (GameObject fastener in fastenerObj)
        {
            fastener.transform.parent = robotPart.transform;
        }

        switch (StaticData.toolDifficulty)
        {
            case 0:
                showGaps = true;

                Debug.Log("Gaps will be shown");
                break;
            case 1:
                showGaps = false;

                Debug.Log("Will be hidden as hints");
                break;
            case 2:
                showGaps = false;

                Debug.Log("Wont be shown");
                break;
            default:
                Debug.Log("Playing tutorial code");
                break;
        }

        robotPart.AddComponent<BoxCollider2D>();

       // robotPart.AddComponent<Rigidbody2D>();

        robotPart.GetComponent<BoxCollider2D>().size = toolTilingManager.TileLength();

        robotPart.GetComponent<BoxCollider2D>().offset = new Vector2(0, -6f);

        robotPart.GetComponent<BoxCollider2D>().layerOverridePriority = 1;

        //robotPart.GetComponent<Rigidbody2D>().gravityScale = 0;


        //Set for left dragging end point
        endPoints[0] = robotPart.transform.position - toolTilingManager.GetTileDistance();

        //Set for right dragging end point
        endPoints[1] = toolTilingManager.GetTileDistance() - robotPart.transform.position;

        robotPart.transform.position = new Vector2(endPoints[1].x, robotPart.transform.position.y);

        Camera.main.GetComponent<ToolCamera>().OverheadCameraView();
        OverheadView();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject() || HandleUIClickEvent())
                {
                    HandleDragEvent();
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (isOnObject)
                {
                    Debug.Log("Dragging Object");

                    isDragging = true;

                    Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                    //draggingObj.position = new Vector3(newPos.x + offset.x, draggingObj.position.y, draggingObj.position.z);

                    robotPart.transform.position = new Vector2(newPos.x + offset.x, robotPart.transform.position.y);
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (!isDragging)
                {
                    if (!EventSystem.current.IsPointerOverGameObject() || HandleUIClickEvent())
                    {
                        HandleClickEvent();
                    }
                }
                else
                {
                    if (tiledParts[2].transform.position.x > 0)
                    {
                        Debug.Log("Limit reached");

                        robotPart.transform.position = new Vector2(endPoints[0].x, robotPart.transform.position.y);
                    }
                    else if (tiledParts[tiledParts.Length-2].transform.position.x < 0)
                    {
                        robotPart.transform.position = new Vector2(endPoints[1].x, robotPart.transform.position.y);
                    }
                    isDragging = false;
                    isOnObject = false;
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

    private void HandleDragEvent()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if (rayHit.transform.name == "Complete Part")
            {
                offset = robotPart.transform.position - Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                isOnObject = true;
                Debug.Log("Interacting with draggable object");
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
                bool useCountManager;
                numberToDisplay[currentInt]++;

                UpdateGapCounters();

                CheckCounterToDisplay();

                if (numberToDisplay[currentInt] <= max_HitCount)
                {
                    useCountManager = true;
                }
                else
                {
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
                }

                ToggleOverviewCounters(false);
                ToggleGapHolder(false);

                robotPart.layer = LayerMask.NameToLayer("Ignore Raycast");

                CheckIfEditable();

                fastenerObj[currentInt].SetActive(true);

                SetZoomedInTextCounter(currentInt);

                CheckCounterToDisplay();
            }
        }
    }

    private void CheckIfEditable()
    {
        if (currentInt >= patternLength - slotToFix)
        {
            fastenerList[0].transform.parent.gameObject.SetActive(true);

            if (fastenerValues[currentInt] > 0)
            {
                Transform holder = tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition();
                SelectTool(fastenerValues[currentInt] - 1, currentInt, holder);
                addTenBtn.gameObject.SetActive(true);

                addTenBtn.GetComponent<Image>().sprite = fastenerList[fastenerValues[currentInt] - 1].GetHitIcon().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                addTenBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            fastenerList[0].transform.parent.gameObject.SetActive(false);
        }
    }

    private void UpdateGapCounters()
    {
        if (currentInt == patternLength - 1)
        {
            gapToDisplay[currentInt - 1] = numberToDisplay[currentInt] - numberToDisplay[currentInt - 1];
        }
        else
        {
            if (currentInt > 0)
            {
                gapToDisplay[currentInt - 1] = numberToDisplay[currentInt] - numberToDisplay[currentInt - 1];
                gapToDisplay[currentInt] = numberToDisplay[currentInt + 1] - numberToDisplay[currentInt];
            }
            else
            {
                gapToDisplay[currentInt] = numberToDisplay[currentInt + 1] - numberToDisplay[currentInt];
            }

        }
    }

    private IEnumerator HitAnimation(Tool tool, bool value)
    {
        if (value)
        {
            hitCountManager.IncreaseChildCount(fastenerObj[currentInt], fastenerList[fastenerValues[currentInt]-1].GetHitIcon());
        }
        else
        {
            SetZoomedInTextCounter(currentInt);
        }

        Debug.Log("Playing animation");
        yield return StartCoroutine(tool.TriggerToolAnimation(tiledParts[currentInt].GetComponent<PartTile>()));
    }


    public void CheckNumber(Transform tools)
    {
        //ToggleOverviewCounters(true);

        ToggleGapHolder(false);

        foreach (GameObject tile_pos in tiledParts)
        {
            Vector3 position = tile_pos.GetComponent<PartTile>().GetFastenerPosition().position;
            Camera.main.GetComponent<ToolCamera>().CreateLoadingIcons(Camera.main.WorldToScreenPoint(position));
        }

        StartCoroutine(ValueCheckCoroutine());    
    }

    public void ChangeToLeftElement(Button button)
    {
        if (currentInt>0)
        {
            int prevInt = 1;
                
            currentInt--;

            addTenBtn.gameObject.SetActive(false);

            StartCoroutine(TriggerFastenerChange(button, prevInt));

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
            int prevInt = -1;

            currentInt++;

            addTenBtn.gameObject.SetActive(false);

            //CheckCounterToDisplay();

            //SetZoomedInTextCounter(currentInt);

            StartCoroutine(TriggerFastenerChange(button, prevInt));

            if (currentTool != null)
            {
                Destroy(currentTool);
            }
        }
    }

    private void CheckCounterToDisplay()
    {
        if (numberToDisplay[currentInt] <= max_HitCount)
        {
            Debug.Log("Displaying hit counts in objects of " + currentInt);
            fastenerObj[currentInt].SetActive(true);
            textCounter.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Displaying hit counts in text " + currentInt);
            fastenerObj[currentInt].SetActive(false);
            textCounter.gameObject.SetActive(true);
            SetZoomedInTextCounter(currentInt);
        }
    }

    public void SelectFastener(Button fastenerBtn)
    {
        if (currentInt >= patternLength - slotToFix)
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

            SelectTool(fastenerBtn.GetComponent<FastenerBtn>().GetFastenerType() - 1, currentInt, holder);

            numberToDisplay[currentInt] = 0;
            foreach (Transform child in fastenerObj[currentInt].transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void SelectTool(int value, int i, Transform holder)
    {
        int currentFastenerVal = fastenerValues[i];
        Debug.Log("Fastener in place: " + currentFastenerVal);
        toolHolder.position = holder.position;

        if (currentTool != null)
        {
            Destroy(currentTool);
        }

        addTenBtn.gameObject.SetActive(true);

        addTenBtn.GetComponent<Image>().sprite = fastenerList[value].GetHitIcon().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

        currentTool = Instantiate(fastenerList[value].GetToolToUse(), toolHolder);

        currentTool.transform.localPosition = new Vector2(0, 0);
        //currentTool.GetComponent<Tool>().SetHeightValue(-0.7f);

        addTenBtn.transform.position = currentTool.GetComponent<Tool>().GetAddTenPos();

        Debug.Log("What is happening???");

        holder.gameObject.SetActive(false);
    }

    public void OverheadView()
    {
        Transform holder = tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition();

        holder.gameObject.SetActive(true);

        Debug.Log(currentInt);
        fastenerObj[currentInt].SetActive(false);
        currentInt = -1;

        foreach (GameObject part in tiledParts)
        {
            part.layer = LayerMask.NameToLayer("Default");
        }

        ToggleOverviewCounters(true);

        ToggleGapHolder(true);

        robotPart.layer = LayerMask.NameToLayer("Default");

        if (currentTool != null)
        {
            Destroy(currentTool);
        }
    }

    public void UndoHitCounts()
    {
        Transform holder = tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition();

        if (currentTool != null)
        {
            Destroy(currentTool);
        }

        /*
        if (currentInt < patternLength - slotToFix)
        {
            numberToDisplay[currentInt] = originalHitValues[currentInt];

            fastenerValues[currentInt] = fastenerCheckVal[currentInt];

            if (holder.childCount > 0)
            {
                Destroy(holder.transform.GetChild(0).gameObject);
            }

            Instantiate(fastenerList[fastenerValues[currentInt]-1].GetFastenerSprite(), tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition());
            tiledParts[currentInt].GetComponent<PartTile>().SetFastenerPosition(-0.7f);

            SelectTool(fastenerList[fastenerValues[currentInt] - 1].GetFastenerType() - 1, currentInt);

            Debug.Log("Undo Value: " + originalHitValues[currentInt]);

            StartCoroutine(ResetHitCoroutine());
        }
        else
        */

        if (currentInt >= patternLength - slotToFix)
        {
            foreach (Transform child in fastenerObj[currentInt].transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            if (StaticData.toolDifficulty == 0)
            {
                Instantiate(fastenerList[fastenerCheckVal[currentInt] - 1].GetMissingPrefab(), tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition());
            }
            else if (StaticData.toolDifficulty == 2 || StaticData.toolDifficulty == 1)
            {
                Instantiate(randMissing_Prefab, tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition());
            }

            //Instantiate(fastenerList[fastenerCheckVal[currentInt] - 1].GetMissingPrefab(), tiledParts[currentInt].GetComponent<PartTile>().GetFastenerPosition());

            if (holder.childCount > 0)
            {
                Destroy(holder.transform.GetChild(0).gameObject);
            }

            numberToDisplay[currentInt] = 0;

            addTenBtn.gameObject.SetActive(false);

            StartCoroutine(ResetHitCoroutine());
        }

        UpdateGapCounters();
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
        if(numberToDisplay[currentInt] <= max_HitCount)
        {
            textCounter.gameObject.SetActive(false);
            if (numberToDisplay[currentInt]>0)
            {
                fastenerObj[currentInt].SetActive(true);
                hitCountManager.PresetCounter(numberToDisplay[currentInt], fastenerObj[currentInt], fastenerList[fastenerValues[currentInt] - 1].GetHitIcon());
            }
            else
            {
                fastenerObj[currentInt].SetActive(false);
            }
            
        }
        else if(numberToDisplay[currentInt] > max_HitCount)
        {
            SetZoomedInTextCounter(currentInt);
            fastenerObj[currentInt].SetActive(false);
            textCounter.gameObject.SetActive(true);
        }
    }

    private void SetZoomedInTextCounter(int value)
    {
        if (fastenerValues[value] > 0)
        {
            textCounter.GetComponentInChildren<TextMeshProUGUI>().text = numberToDisplay[value].ToString();
            textCounter.GetComponentInChildren<Image>().sprite = fastenerList[fastenerValues[value] - 1].GetHitIcon().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            textCounter.GetComponentInChildren<TextMeshProUGUI>().text = "?";
            textCounter.GetComponentInChildren<Image>().sprite = null;
        }
    }

    private IEnumerator ValueCheckCoroutine()
    {
        int totalCorrect = 0;
        int i = 0;

        Vector3 newCameraPos = new Vector3(fastenerObj[0].transform.position.x, 0, Camera.main.transform.position.z);

        Camera.main.GetComponent<ToolCamera>().CameraTrigger(newCameraPos, speed);

        Camera.main.GetComponent<ToolCamera>().ToggleNoteCanvas();

        //Camera.main.GetComponent<ToolCamera>().ToggleCounterCanvas();

        Camera.main.GetComponent<ToolCamera>().ToggleCanvas();

        yield return new WaitForSeconds(1f);

        while (i<patternLength)
        {
            /*
            SelectTool(fastenerCheckVal[i] - 1, i);

            currentTool.GetComponent<Tool>().SetHeightValue(-1f);

            
                if (numberToDisplay[i] < 24)
                {
                    fastenerObj[i].SetActive(true);
                    textCounter.gameObject.SetActive(false);
                }
                else
                {
                    textCounter.gameObject.SetActive(true);
                    fastenerObj[i].SetActive(false);
                    SetZoomedInTextCounter(i);
                }
            

            toolHolder.position = new Vector3(fastenerObj[i].transform.position.x, toolHolder.position.y, toolHolder.position.z);
            */

            

            if (numberToDisplay[i] != generatedList[i] || fastenerCheckVal[i] != fastenerValues[i])
            {
                tiledParts[i].GetComponent<PartTile>().SetIsWrong(false);
                tiledParts[i].GetComponent<PartTile>().GetFastenerPosition().GetComponentInChildren<Fastener>().SetBrokenSprite();
                tiledParts[i].GetComponent<PartTile>().SetFastenerPosition(0.1f);
                Debug.Log("Incorrect!!");
            }
            else
            {
                totalCorrect++;
                tiledParts[i].GetComponent<PartTile>().SetIsWrong(true);
                tiledParts[i].GetComponent<PartTile>().GetFastenerPosition().GetComponentInChildren<Fastener>().SetFixedSprite();
                tiledParts[i].GetComponent<PartTile>().SetFastenerPosition(-1f);
                Debug.Log("Correct!!");
            }

            //yield return StartCoroutine(currentTool.GetComponent<Tool>().TriggerToolAnimation(tiledParts[i].GetComponent<PartTile>()));



            yield return new WaitForSeconds(0.5f);

            Camera.main.GetComponent<ToolCamera>().DeleteLoadingIcons(i);

            fastenerObj[i].SetActive(false);

            i++;

            
        }

        /*
        yield return null;

        Camera.main.GetComponent<ToolCamera>().OverheadCameraView();

        yield return null;
        */

        //ToggleOverviewCounters(true);

        yield return new WaitForSeconds(1f);

        if (totalCorrect == patternLength)
        {
            Debug.Log("All correct!");

            Camera.main.GetComponent<ToolCamera>().SetResultPanel(true);

            StaticData.isToolDone = true;

            if (DataPersistenceManager.Instance != null)
            {
                DataPersistenceManager.Instance.SaveGame();
                Debug.Log("Tool station completion saved to StaticData.");
            }
        }
        else
        {
            Camera.main.GetComponent<ToolCamera>().SetResultPanel(false);
            Debug.Log("Something is wrong!");

            yield return new WaitForSeconds(3f);

            foreach (GameObject partTile in tiledParts) {
                partTile.GetComponent<PartTile>().GetFastenerPosition().GetComponentInChildren<Fastener>().SetFixedSprite();
                partTile.GetComponent<PartTile>().SetIsWrong(false);
                partTile.GetComponent<PartTile>().SetFastenerPosition(-0.7f);
            }
            Camera.main.GetComponent<ToolCamera>().ToggleNoteCanvas();
            //Camera.main.GetComponent<ToolCamera>().ToggleCounterCanvas();
            Camera.main.GetComponent<ToolCamera>().ToggleCanvas();
            Camera.main.GetComponent<ToolCamera>().ToggleCheckingCanvas();

            StaticData.toolWrong += 1;
            Debug.Log("Added one penalty to tool score");

            ToggleGapHolder(true);
        }

        Camera.main.GetComponent<ToolCamera>().ClearLoadingIcons();
    }

    //Changes to the next fastener
    private IEnumerator TriggerFastenerChange(Button button, int prevInt)
    {
        Vector3 newCameraPos = new Vector3(fastenerObj[currentInt].transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        button.interactable = false;

        fastenerList[0].transform.parent.gameObject.SetActive(false);
        if (numberToDisplay[currentInt + prevInt] <= max_HitCount)
        {
            fastenerObj[currentInt + prevInt].SetActive(false);
        }
        else
        {
            textCounter.gameObject.SetActive(false);
        }

        yield return null;

        yield return StartCoroutine(Camera.main.GetComponent<ToolCamera>().SubmitCameraMovement(newCameraPos, speed));

        //fastenerObj[currentInt].SetActive(true);
        CheckCounterToDisplay();
        CheckIfEditable();

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

    private void ToggleGapHolder(bool isShowing)
    {
        if (isShowing)
        {
            for (int i = 0; i < patternLength-1; i++)
            {
                gapHolder[i].GetComponent<GapHolder>().SetGapVal(gapToDisplay[i], originalGaps[i]);

                gapHolder[i].SetActive(true);
            }
        }
        else
        {
            foreach (GameObject gap in gapHolder)
            {
                gap.SetActive(false);
            }
        }
    }

    public void ToggleCollider()
    {
        Camera.main.GetComponent<ToolCamera>().ToggleCanvas();
        foreach (GameObject part in tiledParts)
        {
            part.GetComponent<BoxCollider2D>().enabled = !part.GetComponent<BoxCollider2D>().enabled;
        }
    }

    public void AddTenEvent()
    {
        numberToDisplay[currentInt] = numberToDisplay[currentInt]+10;
        if (numberToDisplay[currentInt] <= max_HitCount)
        {
            fastenerObj[currentInt].SetActive(true);
            textCounter.gameObject.SetActive(false);
            //hitCountManager.IncreaseChildCount(fastenerObj[currentInt], fastenerList[0].GetHitIcon());
        }
        else
        {
            fastenerObj[currentInt].SetActive(false);
            textCounter.gameObject.SetActive(true);
        }

        UpdateGapCounters();

        StartCoroutine(AddMoreCoroutine());
    }

    private IEnumerator AddMoreCoroutine()
    {
        if (fastenerObj[currentInt].transform.childCount >0)
        {
            foreach (Transform child in fastenerObj[currentInt].transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        yield return null;

        if (numberToDisplay[currentInt] <= max_HitCount)
        {
            hitCountManager.PresetCounter(numberToDisplay[currentInt], fastenerObj[currentInt], fastenerList[fastenerValues[currentInt] - 1].GetHitIcon());
        }
        else
        {
            SetZoomedInTextCounter(currentInt);
        }

        yield return StartCoroutine(currentTool.GetComponent<Tool>().TriggerToolAnimation(tiledParts[currentInt].GetComponent<PartTile>()));
    }
}
