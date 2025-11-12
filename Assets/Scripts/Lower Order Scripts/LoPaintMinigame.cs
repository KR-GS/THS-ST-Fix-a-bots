using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class LoPaintMinigame : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private TextMeshProUGUI stickerTextCounter;

    
    [SerializeField]
    private PatternGameManager patternGameManager;

    [SerializeField]
    private PaintMinimapManager minimapManager;


    private bool dragging = false;

    private GameObject draggableObject;

    [SerializeField]
    private int numOfSides;

    [SerializeField]
    private Sticker[] stickerPacks;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Button[] moveBtn;

    [SerializeField]
    private Button[] moveSidesBtn;

    [SerializeField]
    private Canvas checkUI;

    [SerializeField]
    private Canvas overviewUI;

    [SerializeField]
    private Canvas notesUI;

    [SerializeField]
    private Canvas tutorialUI;

    [SerializeField]
    private TutorialManager tutorialManager;

    [SerializeField]
    private Sprite correct_sprite;

    [SerializeField]
    private Sprite wrong_sprite;

    [SerializeField]
    private Sprite correct_face;

    [SerializeField]
    private Sprite wrong_face;

    [SerializeField]
    private GameObject loading_obj;

    [SerializeField]
    private GameObject robotPart;

    /*
    [SerializeField]
    private DifficultyManager difficulty;
    */

    private GameObject[] partSides;

    private int currentSide = 0;

    private int currentStickerPack = 0;

    private List<int[]> numberPattern = new List<int[]>();

    private List<int[]> number2Pattern = new List< int[]>();

    private List<int> packUsed = new List<int>();

    private List<GameObject> loading_list = new List<GameObject>();

    private Vector2 pos_anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        DataPersistenceManager.Instance.LoadGame();
        Debug.Log("Awake called in LoPaintMinigame");
        partSides = new GameObject[numOfSides];
        Debug.Log("Initialized partSides array with size: " + numOfSides);
    }
    void Start()
    {
        checkUI.enabled = false;
        float posY;
        float posX;
        RenderTexture[] minimapRT = FindFirstObjectByType<PaintMinimapManager>().GetGeneratedRT();

        List<int> packToUse = new List<int>();

        Debug.Log("Current difficulty: " + StaticData.paintDifficulty);
        
        if (StaticData.paintPattern != null && StaticData.paintPattern.Count > 0 && StaticData.paint2Pattern != null && StaticData.paint2Pattern.Count > 0)
        {
            Debug.Log("Loading paint patterns from StaticData");

            if (StaticData.paintDifficulty == 0)
            {
                Debug.Log("Hello World");
                numberPattern.Add(StaticData.paintPattern.ToArray());
                //numberPattern.Add(StaticData.paintPattern[0]);

                int randPack = StaticData.selectedStickerIndex;
                //int randPack = Random.Range(0, stickerPacks.Length);
                packToUse.Add(randPack);

                if (randPack >= 0 && randPack < stickerPacks.Length)
                {
                    packUsed.Add(stickerPacks[randPack].GetStickerNum());
                }
                else
                {
                    Debug.LogError($"Invalid sticker pack index: {randPack}");
                }
            }
            else if (StaticData.paintDifficulty == 1 || StaticData.paintDifficulty == 2)
            {
                Debug.Log("Current Number Pattern: " + numberPattern);
                Debug.Log("Current 2nd Number Pattern: " + number2Pattern);
                /*
                for (int i = 0; i < 2; i++)
                {
                    Debug.Log("Wello horld!");
                    numberPattern.Add(StaticData.paintPattern.ToArray()); // reuse pattern twice
                    //numberPattern.Add(StaticData.paintPattern[i]);
                }
                */

                numberPattern.Add(StaticData.paintPattern.ToArray());
                numberPattern.Add(StaticData.paint2Pattern.ToArray());
                
                Debug.Log("Number Patterns after adding from StaticData: " + numberPattern);

                int j = 0;
                while (j < 2)
                {
                    int rand = 0;

                    if (j == 0)
                    {
                        rand = StaticData.selectedStickerIndex;
                    }
                    else if (j == 1)
                    {
                        rand = StaticData.selectedStickerIndexTwo;
                    }
                    if (!packToUse.Contains(rand))
                    {
                        packToUse.Add(rand);
                        if (rand >= 0 && rand < stickerPacks.Length)
                        {
                            packUsed.Add(stickerPacks[rand].GetStickerNum());
                        }
                        else
                        {
                            Debug.LogError($"Invalid sticker pack index: {rand}");
                        }
                        j++;
                    }
                }

                Debug.Log("Assignment done");
            }
        }
        else
        {
            Debug.LogWarning("StaticData.paintPattern is empty. This shouldn't happen if GameLoopManager generated and stored it properly.");
        }

        Debug.Log("Number of current sticker packs: " + stickerPacks.Length);
        Debug.Log("sticker pack name: " + stickerPacks[0].name);

        partSides[0] = robotPart;

        partSides[0].GetComponentInChildren<Camera>().targetTexture = minimapRT[0];

        posY = partSides[0].transform.position.y;

        posX = partSides[0].transform.position.x;

        int base_distance = 50;

        pos_anim = new Vector2(posX - 25, posY);

        for (int i = 1; i< numOfSides; i++)
        {
            partSides[i] = Instantiate(partSides[0]);

            partSides[i].transform.position = new Vector3(posX - (i * base_distance), posY, partSides[i].transform.position.z);

            partSides[i].name = partSides[0].name + " " + i;

            partSides[i].GetComponentInChildren<Camera>().targetTexture = minimapRT[i];
        }

        for (int i = 0; i < numOfSides; i++)
        {
            for (int j = 0; j < numberPattern.Count; j++)
            {
                if (i < numberPattern[j].Length)
                {
                    partSides[i].GetComponentInChildren<RobotPaintPart>().SetSideValue(numberPattern[j][i]);
                }
                else
                {
                    Debug.LogWarning($"Pattern index out of range: numberPattern[{j}].Length = {numberPattern[j].Length}, but i = {i}");
                }
            }

            if (i < numOfSides - 1)
            {
                Debug.Log(packToUse.Count);
                partSides[i].GetComponentInChildren<RobotPaintPart>().SetStickersOnSide(stickerPacks, packToUse);
            }



            partSides[i].GetComponentInChildren<RobotPaintPart>().AssignSideNumber(i);
        }

        Debug.Log(partSides[0].name);

        if (StaticData.isFirstPaint)
        {
            StaticData.isFirstPaint = false;
            DataPersistenceManager.Instance.SaveGame();
            tutorialManager.OpenTutorial();
            OpenTutorial();
        }
        Debug.Log("Reached End of Start()");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (!dragging)
                    {
                        HandleClickEvent(Input.GetTouch(0).position);
                    }
                }
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (dragging)
                {
                    draggableObject.GetComponent<Sticker>().ResetColor();
                    if (draggableObject.GetComponent<Sticker>().IsOnPart())
                    {
                        dragging = false;
                        if (draggableObject.GetComponent<Sticker>().IsADefault())
                        {
                            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                            Debug.Log(currentSide);
                            if(touchPos.x > partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_RightVal() + 0.5f)
                            {
                                touchPos.x = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_RightVal();
                            }
                            else if(touchPos.x < partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_LeftVal() - 0.5f)
                            {
                                touchPos.x = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_LeftVal();
                            }

                            if (touchPos.y > partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_UpVal() +0.5f)
                            {
                                touchPos.y = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_UpVal();
                            }
                            else if (touchPos.y < partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_DownVal() - 0.5f)
                            {
                                touchPos.y = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_DownVal();
                            }

                            Vector3 newPos = new Vector3(touchPos.x, touchPos.y, 0);

                            draggableObject.GetComponent<Sticker>().SetDefaultPos(newPos);
                            draggableObject.transform.position = newPos;
                        }
                        else
                        {
                            if (draggableObject.GetComponent<Sticker>().GetPartOn() < 5)
                            {
                                StartCoroutine(TriggerStickerFall());
                                dragging = false;
                            }
                        }

                        draggableObject = null;
                    }
                    else
                    {
                        if (!draggableObject.GetComponent<Sticker>().IsADefault())
                        {
                            Destroy(draggableObject);
                            dragging = false;
                        }
                        else
                        {
                            draggableObject.transform.position = draggableObject.GetComponent<Sticker>().GetDefaultPos();
                            draggableObject = null; 
                            dragging = false;
                        }
                    }
                }
            }
            else
            {
                if (dragging)
                {
                    Vector2 cameraPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    Vector3 touchPos = new Vector3(cameraPoint.x, cameraPoint.y, -0.1f);
                    draggableObject.transform.position = touchPos;
                }
            }
        }

        //stickerTextCounter.text = roboPart.GetCurrentStickerSideCount().ToString();
    }

    private void HandleClickEvent(Vector2 position)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
        if (rayHit.collider != null)
        {
            Debug.Log("Interacting with: " + rayHit.transform.name);
            if (rayHit.transform.gameObject.TryGetComponent(out Sticker sticker))
            {
                if (sticker.IsOnPart())
                {
                    Debug.Log("This is on the robot");
                    draggableObject = sticker.gameObject;
                }
                else
                {
                    draggableObject = Instantiate(sticker.transform.gameObject);

                    draggableObject.layer = LayerMask.NameToLayer("Default");
                    draggableObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(position).x, Camera.main.ScreenToWorldPoint(position).y, -0.1f);
                    draggableObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                    draggableObject.GetComponent<Sticker>().ToggleIsADuplicate();
                    Debug.Log(draggableObject.GetComponent<Sticker>().IsADuplicate());
                    
                }

                draggableObject.GetComponent<Sticker>().SetIsHighlighted();
                dragging = true;
                Debug.Log(draggableObject.name);
            }
        }
    }

    public IEnumerator TriggerStickerFall()
    {
        GameObject obj_to_delete = draggableObject;
        draggableObject.GetComponent<BoxCollider2D>().enabled = false;
        draggableObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(2f);
        Debug.Log("Deleting obj");
        Destroy(obj_to_delete);
    }

    public void ClearStickers()
    {
        partSides[currentSide].GetComponentInChildren<RobotPaintPart>().ClearStickersOnSide();
    }

    public void CheckSideValues()
    {
        PaintMinimapManager mapSelect = FindAnyObjectByType<PaintMinimapManager>();
        
        for(int i = 0; i < 6; i++)
        {
            loading_list.Add(Instantiate(loading_obj, checkUI.transform));
            loading_list[i].transform.position = mapSelect.GetMinimapPosition(i);
        }

        checkUI.transform.Find("Result Image").gameObject.SetActive(false);
        checkUI.transform.Find("Button").gameObject.SetActive(false);
        checkUI.enabled = true;
        StartCoroutine(ValueCheckCoroutine());
    }

    public IEnumerator ValueCheckCoroutine()
    {
        int prevSide = currentSide;
        PaintMinimapManager mapSelect = FindAnyObjectByType<PaintMinimapManager>();
        overviewUI.enabled = false;
        notesUI.enabled = false;
        tutorialUI.enabled = false;
        Debug.Log("Hello from checking");
        int correctAnsNo = 0;
        int correntTypeNo = 0;
        bool countCorrect = false;
        bool typeCorrect = false;
        StaticData.playerPaintPattern = new List<int>();
        StaticData.playerPaint2Pattern = new List<int>();


        yield return new WaitForSeconds(1f);

        for (int i = 0; i < numOfSides; i++)
        {
            ChangeSide(i);
            mapSelect.ChangeSelectedSide(i);
            yield return new WaitForSeconds(0.5f);

            for (int j =0; j < numberPattern.Count; j++)
            {
                Debug.Log("Adding data in player paint pattern!");
                int playerCount = partSides[i].GetComponentInChildren<RobotPaintPart>().GetCurrentStickerSideCount(packUsed[j]);
                
                if(j == 0)
                {
                    StaticData.playerPaintPattern.Add(playerCount);
                }
                else if (j == 1)
                {
                    StaticData.playerPaint2Pattern.Add(playerCount);
                }

                if (partSides[i].GetComponentInChildren<RobotPaintPart>().GetCurrentStickerSideCount(packUsed[j]) == numberPattern[j][i])
                {
                    countCorrect = true;
                }
                else
                {
                    countCorrect = false;
                    Debug.Log("Count is wrong");
                    break;
                }
            }

            if (countCorrect)
            {
                correctAnsNo++;
            }

            if (partSides[i].GetComponentInChildren<RobotPaintPart>().GetStickeyTypeCount(packUsed))
            {
                correntTypeNo++;
                typeCorrect = true;
            }

            else
            {
                typeCorrect = false;
                Debug.Log("Some type is wrong");
            }

            if(typeCorrect && countCorrect)
            {
                loading_list[i].GetComponentInChildren<Image>().sprite = correct_face;
            }
            else
            {
                loading_list[i].GetComponentInChildren<Image>().sprite = wrong_face;
            }

            yield return new WaitForSeconds(0.25f);
        }

        for (int i = 0; i < 6; i++)
        {
            Destroy(loading_list[i]);
            yield return new WaitForSeconds(0.1f);
        }

        loading_list.Clear();

        checkUI.transform.Find("Result Image").gameObject.SetActive(true);

        if (correctAnsNo == numOfSides && correntTypeNo == numOfSides)
        {
            Debug.Log("All Numbers Correct!");
            Debug.Log("All Types Correct!");
            overviewUI.enabled = false;
            notesUI.enabled = false;
            StaticData.isPaintDone = true;
            Debug.Log("Paint station marked as done in StaticData.");

            /*
            if (DataPersistenceManager.Instance != null)
            {
                DataPersistenceManager.Instance.SaveGame();
                Debug.Log("Paint station completion saved to StaticData.");
            }
            */

            StaticData.pendingGameRecord = new GameData.GameRecord(
            StaticData.paintPattern,
            StaticData.playerPaintPattern,
            StaticData.paint2Pattern,
            new List<int>(StaticData.playerPaint2Pattern ?? new List<int>()),
            StaticData.timeSpent,
            StaticData.dayNo,
            StaticData.paintWrong, // Capture NOW
            1,
            StaticData.orderNumber,
            1
        );

            if (DataPersistenceManager.Instance != null)
            {
                DataPersistenceManager.Instance.SaveGame();
                Debug.Log("Paint station completion saved to StaticData.");
            }

            checkUI.transform.Find("Result Image").GetComponent<Image>().sprite = correct_sprite;
            checkUI.transform.Find("Button").gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Some number's wrong");
            Debug.Log("Some type's wrong");
            overviewUI.enabled = true;
            StaticData.paintWrong += 1;
            Debug.Log("Added one penalty to paint score");
            checkUI.transform.Find("Result Image").GetComponent<Image>().sprite = wrong_sprite;
            checkUI.transform.Find("Button").gameObject.SetActive(false);

            StaticData.pendingGameRecord = new GameData.GameRecord(
            StaticData.paintPattern,
            StaticData.playerPaintPattern,
            StaticData.paint2Pattern,
            new List<int>(StaticData.playerPaint2Pattern ?? new List<int>()),
            StaticData.timeSpent,
            StaticData.dayNo,
            StaticData.paintWrong, // Capture NOW
            1,
            StaticData.orderNumber,
            1
        );

            if (DataPersistenceManager.Instance != null)
            {
                DataPersistenceManager.Instance.SaveGame();
                Debug.Log("Paint station completion saved to StaticData.");
            }

            yield return new WaitForSeconds(2f);

            notesUI.enabled = true;
            checkUI.enabled = false;
        }

        yield return null;

        mapSelect.ChangeSelectedSide(prevSide);
        ChangeSide(prevSide);
        
    }

    public void ChangeSideRight()
    {
        if (currentSide < 5)
        {
            StartCoroutine(RightChange());
        }
    }

    public void ChangeSideLeft()
    {
        if (currentSide > 0)
        {
            StartCoroutine(LeftChange());
        }
    }

    private IEnumerator RightChange()
    {
        int prevSide = currentSide;

        Vector2 origPos1 = partSides[currentSide+1].transform.position;

        Vector2 origPos2 = partSides[prevSide].transform.position;

        Vector2 goalPos = new Vector2(-pos_anim.x, pos_anim.y);

        currentSide++;

        partSides[currentSide].transform.position = pos_anim;

        partSides[prevSide].GetComponentInChildren<BoxCollider2D>().enabled = false;

        partSides[currentSide].GetComponentInChildren<BoxCollider2D>().enabled = false;

        foreach (Button btn in moveSidesBtn)
        {
            btn.enabled = false;
        }

        minimapManager.DisableMinimapPressing();

        while (Vector2.Distance(partSides[prevSide].transform.position, goalPos) > 0.01f)
        {
            partSides[prevSide].transform.position = Vector2.MoveTowards(partSides[prevSide].transform.position, goalPos, speed * 3 * Time.deltaTime);
            yield return null;
        }

        while (Vector2.Distance(partSides[currentSide].transform.position, origPos2) > 0.01f)
        {
            partSides[currentSide].transform.position = Vector2.MoveTowards(partSides[currentSide].transform.position, origPos2, speed * 3 * Time.deltaTime);
            yield return null;
        }

        yield return null;

        minimapManager.ChangeSelectedSide(currentSide);

        yield return new WaitForSeconds(0.5f);

        partSides[prevSide].transform.position = origPos1;

        partSides[prevSide].GetComponentInChildren<BoxCollider2D>().enabled = true;

        partSides[currentSide].GetComponentInChildren<BoxCollider2D>().enabled = true;

        yield return new WaitForSeconds(0.5f);

        minimapManager.EnableMinimapPressing();

        foreach (Button btn in moveSidesBtn)
        {
            btn.enabled = true;
        }
    }

    private IEnumerator LeftChange()
    {
        int prevSide = currentSide;

        Vector2 origPos1 = partSides[currentSide - 1].transform.position;

        Vector2 origPos2 = partSides[prevSide].transform.position;

        Vector2 goalPos = new Vector2(-pos_anim.x, pos_anim.y);

        currentSide--;

        partSides[currentSide].transform.position = goalPos;

        partSides[prevSide].GetComponentInChildren<BoxCollider2D>().enabled = false;

        partSides[currentSide].GetComponentInChildren<BoxCollider2D>().enabled = false;

        foreach(Button btn in moveSidesBtn)
        {
            btn.enabled = false;
        }

        minimapManager.DisableMinimapPressing();

        while (Vector2.Distance(partSides[prevSide].transform.position, pos_anim) > 0.01f)
        {
            partSides[prevSide].transform.position = Vector2.MoveTowards(partSides[prevSide].transform.position, pos_anim, speed * 3 * Time.deltaTime);
            yield return null;
        }

        while (Vector2.Distance(partSides[currentSide].transform.position, origPos2) > 0.01f)
        {
            partSides[currentSide].transform.position = Vector2.MoveTowards(partSides[currentSide].transform.position, origPos2, speed * 3 * Time.deltaTime);
            yield return null;
        }

        yield return null;

        minimapManager.ChangeSelectedSide(currentSide);

        yield return new WaitForSeconds(1f);

        minimapManager.EnableMinimapPressing();

        partSides[prevSide].transform.position = origPos1;

        partSides[prevSide].GetComponentInChildren<BoxCollider2D>().enabled = true;

        partSides[currentSide].GetComponentInChildren<BoxCollider2D>().enabled = true;

        foreach (Button btn in moveSidesBtn)
        {
            btn.enabled = true;
        }
    }

    public void ChangeSide(int val)
    {
        Vector3 tempPos = partSides[currentSide].transform.position;
        int prevVal = currentSide;
        currentSide = val;

        partSides[prevVal].transform.position = partSides[currentSide].transform.position;
        partSides[currentSide].transform.position = tempPos;
        Transform defaultObj = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().GetDefaultHolder();

        for (int i = 0; i < defaultObj.childCount; i++) 
        {
            defaultObj.GetChild(i).GetComponent<Sticker>().SetDefaultPos(Vector3.zero);
        }
    }

    public void ChangeStickerUp()
    {
        
        if (currentStickerPack > 0)
        {
            //Vector3 newPos = stickerPacks[currentStickerPack].transform.position;
            //currentStickerPack--;
            //stickerPacks[currentStickerPack+1].transform.position = stickerPacks[currentStickerPack].transform.position;
            //stickerPacks[currentStickerPack].transform.position = newPos;
            StartCoroutine(TriggerPackChange(1));
        }
    }

    public void ChangeStickerDown()
    {
        if (currentStickerPack < stickerPacks.Length-1)
        {
            Debug.Log("Hello World");
            //stickerPacks[currentStickerPack - 1].transform.position = stickerPacks[currentStickerPack].transform.position;
            //stickerPacks[currentStickerPack].transform.position = newPos;
            StartCoroutine(TriggerPackChange(-1));
        }
    }

    private IEnumerator TriggerPackChange(int val)
    {
        Debug.Log("Coroutine Triggered");

        foreach(Button button in moveBtn)
        {
            button.interactable = false;
        }

        Vector3 newPos = stickerPacks[currentStickerPack].transform.parent.position;
        
        if(val == 1)
        {
            currentStickerPack--;
        }
        else
        {
            currentStickerPack++;
        }

        while (Vector3.Distance(stickerPacks[currentStickerPack + val].transform.parent.position, stickerPacks[currentStickerPack].transform.parent.position) > 0.001f)
        {
            stickerPacks[currentStickerPack + val].transform.parent.position = Vector3.MoveTowards(stickerPacks[currentStickerPack + val].transform.parent.position, stickerPacks[currentStickerPack].transform.parent.position, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        while (Vector3.Distance(stickerPacks[currentStickerPack].transform.parent.position, newPos) > 0.001f)
        {
            stickerPacks[currentStickerPack].transform.parent.position = Vector3.MoveTowards(stickerPacks[currentStickerPack].transform.parent.position, newPos, speed * Time.deltaTime);
            yield return null;
        }

        foreach (Button button in moveBtn)
        {
            button.interactable = true;
        }
    }

    public void OpenTutorial()
    {
        minimapManager.DisableMinimapPressing();

        overviewUI.enabled = false;
    }

    public void CloseTutorial()
    {
        minimapManager.EnableMinimapPressing();

        overviewUI.enabled = true;
    }

    public void SaveData(ref GameData data)
    {
        data.isFirstPaint = StaticData.isFirstPaint;
    }
    public void LoadData(GameData data)
    {
        StaticData.isFirstPaint = data.isFirstPaint;
    }
}
