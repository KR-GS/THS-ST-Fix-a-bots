using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class LoWireMinigame : MonoBehaviour
{
    [SerializeField]
    private GameObject generator;

    [SerializeField]
    private GameObject robot_part;

    [SerializeField]
    private Transform wireGeneratedPlace;

    [SerializeField]
    private PatternGameManager patternManager;

    [SerializeField]
    private DifficultyManager difficultyManager;

    [SerializeField]
    private Canvas ValueUI;

    [SerializeField]
    private Wire origWire;

    [SerializeField]
    private WireSlot[] wireSlots;

    [SerializeField]
    private Canvas OverallUI;

    [SerializeField]
    private Canvas GeneratorUI;

    [SerializeField]
    private Canvas RobotUI;

    [SerializeField]
    private Transform sparks_vfx;

    [SerializeField]
    private Canvas ResultUI;

    [SerializeField]
    private Transform[] copper_Ends;

    [SerializeField]
    private NotesManager notesManager;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private Button hint_btn;

    private bool tutorialOpen = false;

    /*
    [SerializeField]
    private WireGenerator wireGenerator;
    */

    private int[] num_patterns;

    private bool isDragging;

    private GameObject wireToAdd;

    [SerializeField]
    private GameObject pliers;

    private bool isOpen = false;

    private bool isDeleting = false;

    private int item_dragged = 0;

    //private Vector2 origPos_Pliers;

    private bool onWireToCut;

    private List<int> wireToEdit = new List<int>();

    private List<Transform> vfxList = new List<Transform>();

    [SerializeField]
    private Vector3 origPos_Robot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WireGenerator wireGenerator = FindAnyObjectByType<WireGenerator>();

        Color btn_Red = wireGenerator.GetRed();

        Color btn_Blue = wireGenerator.GetBlue();

        Color btn_Yellow = wireGenerator.GetYellow();

        ResultUI.enabled = false;

        GeneratorUI.enabled = false;

        Vector2 size = new Vector2(origWire.transform.lossyScale.x, origWire.transform.lossyScale.y);
        //num_patterns = patternManager.ReturnPatternArray(6).ToArray();
        num_patterns = StaticData.wirePattern.ToArray();
        Debug.Log("Your pattern array is: " + num_patterns);

        isDragging = false;

        int valueToChange = StaticData.valuestoChange;

        for (int i = 0; i < 6; i++)
        {
            GameObject newWire = new GameObject("Default Wire " + i);
            newWire.transform.position = origWire.transform.position;
            List<GameObject> wireSegments = new List<GameObject>();
            Debug.Log("Current Int: " + num_patterns[i]);
            int red_segments = num_patterns[i];
            int total_val = red_segments;

            if (StaticData.wireDifficulty == 0)
            {
                if (i == 5)
                {
                    red_segments = 0;
                    total_val = 0;
                }
            }
            else if (StaticData.wireDifficulty == 1 || StaticData.wireDifficulty == 2)
            {
                if (valueToChange == i)
                {
                    Debug.Log("Changing Value of Index " + valueToChange);
                    int diff = Random.Range(2, 3);
                    red_segments = red_segments - diff;
                    total_val = red_segments;
                    Debug.Log("New Value: " + red_segments);
                }
            }

            if (red_segments > 0)
            {
                int yellow_segments = red_segments / 10;
                red_segments = red_segments - (yellow_segments * 10);
                int blue_segments = red_segments / 5;
                red_segments = red_segments - (blue_segments * 5);
                int totalSegments = yellow_segments + blue_segments + red_segments;

                Debug.Log("total segments for " + num_patterns[i] + ": " + totalSegments);

                wireSegments = ChangeWireValue(totalSegments, origWire, newWire);

                int currentSegment = 0;

                if (yellow_segments > 0)
                {
                    for (int j = 0; j < yellow_segments; j++)
                    {
                        wireSegments[currentSegment].GetComponent<SpriteRenderer>().color = btn_Yellow;
                        currentSegment++;
                    }
                }

                if (blue_segments > 0)
                {
                    for (int j = 0; j < blue_segments; j++)
                    {
                        wireSegments[currentSegment].GetComponent<SpriteRenderer>().color = btn_Blue;
                        currentSegment++;
                    }
                }

                if (red_segments > 0)
                {
                    for (int j = 0; j < red_segments; j++)
                    {
                        wireSegments[currentSegment].GetComponent<SpriteRenderer>().color = btn_Red;
                        currentSegment++;
                    }
                }

                for (int j = 0; j < newWire.transform.childCount; j++)
                {
                    newWire.transform.GetChild(j).GetComponent<BoxCollider2D>().enabled = false;
                    Destroy(newWire.transform.GetChild(j).GetComponent<Wire>());
                }

                newWire.AddComponent<BoxCollider2D>();
                newWire.AddComponent<SpriteRenderer>();
                newWire.AddComponent<Wire>();
                newWire.AddComponent<Rigidbody2D>();

                newWire.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                newWire.GetComponent<Wire>().SetWireNumber(total_val);
                newWire.GetComponent<Wire>().SetComplete();

                newWire.transform.SetParent(null);
                newWire.GetComponent<BoxCollider2D>().size = size;
                newWire.GetComponent<BoxCollider2D>().isTrigger = true;

                Transform copper_L = Instantiate(copper_Ends[0]);
                Transform copper_R = Instantiate(copper_Ends[1]);

                copper_L.transform.SetParent(newWire.transform);
                copper_R.transform.SetParent(newWire.transform);

                copper_L.transform.localPosition = new Vector3(copper_L.transform.localPosition.x, -0.07f, copper_L.transform.localPosition.z);
                copper_R.transform.localPosition = new Vector3(copper_R.transform.localPosition.x, -0.015f, copper_R.transform.localPosition.z);

                newWire.transform.position = wireSlots[i].transform.position;
                newWire.transform.SetParent(wireSlots[i].transform.parent);

                if (total_val != num_patterns[i])
                {
                    GameObject extraVFX = new GameObject();

                    extraVFX = Instantiate(sparks_vfx.gameObject, robot_part.transform);

                    extraVFX.transform.position = wireSlots[i].transform.position;

                    extraVFX.GetComponent<VFXManager>().SetSlotNumber(i);

                    foreach (Transform child in extraVFX.transform)
                    {
                        child.GetComponent<VfxSegment>().ToggleVFXAnimOn();
                    }

                    vfxList.Add(extraVFX.transform);

                    wireToEdit.Add(i);
                }
            }
            else
            {
                GameObject extraVFX = new GameObject();

                extraVFX = Instantiate(sparks_vfx.gameObject, robot_part.transform);

                extraVFX.transform.position = wireSlots[i].transform.position;

                extraVFX.GetComponent<VFXManager>().SetSlotNumber(i);

                foreach(Transform child in extraVFX.transform)
                {
                    child.GetComponent<VfxSegment>().ToggleVFXAnimOff();
                }

                vfxList.Add(extraVFX.transform);

                wireToEdit.Add(i);
            }
        }

        //generator.SetActive(isOpen);

        //robot_part.SetActive(!isOpen);

        origPos_Robot = robot_part.transform.position;

        sparks_vfx.gameObject.SetActive(false);

        switch (StaticData.wireDifficulty)
        {
            case 0:
                ValueUI.enabled = true;
                hint_btn.gameObject.SetActive(false);
                break;
            case 1:
                ValueUI.enabled = false;
                hint_btn.gameObject.SetActive(true);
                break;
            case 2:
                ValueUI.enabled = false;
                hint_btn.gameObject.SetActive(false);
                break;
            default:
                hint_btn.gameObject.SetActive(false);
                break;
        }
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
                    Debug.Log("Hello World");
                    HandleClickEvent(Input.GetTouch(0).position);
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    //Debug.Log(wireToAdd.GetComponent<Wire>().GetNewNearbyPos());
                    if (item_dragged == 1)
                    {
                        if (wireToAdd.GetComponent<Wire>().CheckOnSlot())
                        {

                            wireToAdd.transform.SetParent(wireToAdd.GetComponent<Wire>().GetNewNearbyPos().parent);

                            wireToAdd.transform.position = wireToAdd.GetComponent<Wire>().GetNewNearbyPos().position;

                            //wireToAdd.GetComponent<Wire>().SetSlotStatus();
                        }
                        else
                        {
                            wireToAdd.GetComponent<Wire>().SetNewWirePos(wireGeneratedPlace);

                            wireToAdd.transform.position = wireGeneratedPlace.position;

                            wireToAdd.transform.SetParent(wireGeneratedPlace);
                        }

                        wireToAdd = null;
                    }

                    item_dragged = 0;

                    isDragging = false;
                }
            }
            else
            {
                if (isDragging)
                {
                    if(item_dragged == 1)
                    {
                        Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        wireToAdd.transform.position = new Vector2(touchPos.x, touchPos.y);
                    }
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

    private void HandleClickEvent(Vector2 position)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
        if (rayHit.collider != null)
        {
            Debug.Log(rayHit.transform.name);
            if (rayHit.transform.gameObject.TryGetComponent(out Wire wire))
            {
                if (!isDragging)
                {
                    if (wire.GetComplete() && wire.GetMovableStatus())
                    {
                        Debug.Log(wire.transform.name);
                        wireToAdd = wire.transform.gameObject;
                        isDragging = true;
                        //wireGeneratedPlace = wire.transform;
                        item_dragged = 1;
                    }
                }
            }
            else if (rayHit.transform.gameObject.TryGetComponent(out WirePliers plier))
            {
                StartCoroutine(StartWireCutting(plier));
            }
        }
    }

    public List<GameObject> ChangeWireValue(float value, Wire wireToChange, GameObject parent)
    {
        wireToChange.gameObject.SetActive(true);
        List<GameObject> generatedChildren = new List<GameObject>();
        List<float> generatePoints = new List<float>();

        Debug.Log(generatedChildren.Count);

        int intValue = (int)value;

        float newLen = wireToChange.GetComponent<SpriteRenderer>().bounds.size.x / intValue;

        generatePoints = new List<float>(wireToChange.GetDivisionPoints(intValue));

        Debug.Log("Number: " + generatePoints.Count);

        for (int i = 0; i < intValue; i++)
        {
            generatedChildren.Add(Instantiate(wireToChange.transform.gameObject));
            generatedChildren[i].transform.localScale = new Vector2(newLen, wireToChange.GetWireHeight());

            generatedChildren[i].name = i.ToString();

            generatedChildren[i].transform.SetParent(parent.transform);

            /*
            if (i % 2 == 0)
            {
                generatedChildren[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
            }
            else
            {
                generatedChildren[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.grey;
            }
            */

            //generatedChildren[i].GetComponent<SpriteRenderer>().sortingOrder = 4;

            generatedChildren[i].transform.position = new Vector2(generatePoints[i], parent.transform.position.y);
        }

        Debug.Log("Current Value");

        wireToChange.gameObject.SetActive(false);

        return generatedChildren;
    }

    public void OpenGenerator()
    {
        StartCoroutine(MoveToGenerator());
    }

    private IEnumerator MoveToGenerator()
    {
        GameObject tempObj = new GameObject();

        tempObj.transform.position = generator.transform.position;

        generator.transform.SetParent(tempObj.transform);

        robot_part.transform.SetParent(tempObj.transform);

        OverallUI.enabled = false;

        RobotUI.enabled = false;

        while (Vector2.Distance(tempObj.transform.position, origPos_Robot) > 0.01f)
        {
            tempObj.transform.position = Vector2.MoveTowards(tempObj.transform.position, origPos_Robot, speed * 3 * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        //OverallUI.enabled = true;

        GeneratorUI.enabled = true;

        generator.transform.SetParent(null);

        robot_part.transform.SetParent(null);
    }

    public void ReturnToRobot()
    {
        StartCoroutine(MoveToRobotPart());
    }

    private IEnumerator MoveToRobotPart()
    {
        GameObject tempObj = new GameObject();

        tempObj.transform.position = robot_part.transform.position;

        robot_part.transform.SetParent(tempObj.transform);

        generator.transform.SetParent(tempObj.transform);

        OverallUI.enabled = false;

        GeneratorUI.enabled = false;

        while (Vector2.Distance(tempObj.transform.position, origPos_Robot) > 0.01f)
        {
            tempObj.transform.position = Vector2.MoveTowards(tempObj.transform.position, origPos_Robot, speed * 3 * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); 

        OverallUI.enabled = true;

        RobotUI.enabled = true;

        generator.transform.SetParent(null);

        robot_part.transform.SetParent(null);
    }

    public void ToggleDelete()
    {
        isDeleting = !isDeleting;
    }

    private IEnumerator StartWireCutting(WirePliers wirePliers)
    {
        Transform currentSegment = wirePliers.GetSegment().transform.parent;
        int current_Side = wirePliers.GetSegment().GetSide();
        Vector3 current_Pos = wirePliers.GetSegment().transform.position;
        //yield return StartCoroutine(pliers.GetComponent<WirePliers>().TriggerPlierMovement(true, 5));

        wirePliers.DisableCutting();

        wirePliers.GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f);

        wirePliers.TriggerPlierMovement(current_Pos, current_Side);

        wirePliers.TriggerCuttingAnim();

        yield return new WaitForSeconds(1f);

        wirePliers.StopParticleEmission();

        VfxSegment next_Side = currentSegment.GetComponent<VFXManager>().GetOppositeSide(current_Side);

        //pliers.transform.position = next_Side.transform.position;

        yield return new WaitForSeconds(0.05f);

        wirePliers.SetSegment(next_Side);

        yield return new WaitForSeconds(0.5f);

        current_Pos = wirePliers.GetSegment().transform.position;

        current_Side = wirePliers.GetSegment().GetSide();

        wirePliers.TriggerPlierMovement(current_Pos, current_Side);

        wirePliers.TriggerCuttingAnim();

        yield return new WaitForSeconds(1f);

        wirePliers.StopParticleEmission();

        //yield return StartCoroutine(pliers.GetComponent<WirePliers>().TriggerPlierMovement(true, 5));


        /*
        int sidecount = 0;

        foreach(Transform child in currentSegment)
        {
            if (!child.GetComponent<BoxCollider2D>().enabled)
            {
                sidecount++;
            }
        }
        */

        yield return new WaitForSeconds(0.5f);

        Transform wireObj = wireSlots[currentSegment.GetComponent<VFXManager>().GetSlotNumber()].transform.parent.GetComponentInChildren<Wire>().transform;
        wireObj.transform.position = new Vector3(wireObj.position.x, wireObj.position.y, -0.1f);
        wireObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(1f);

        Destroy(wireObj.gameObject);

        //StartCoroutine(pliers.GetComponent<WirePliers>().TriggerPlierMovement(false, 5));

        //pliers.GetComponent<WirePliers>().TriggerPlierMovement(false, 5);

        //pliers.transform.position = new Vector2(origPos_Pliers.x, origPos_Pliers.y);

        wirePliers.RemoveCurrentVFX();

        bool currentStatus = wirePliers.CheckList();

        if (wirePliers.CheckList())
        {
            wirePliers.GetComponent<BoxCollider2D>().enabled = true;

            yield return new WaitForEndOfFrame();

            wirePliers.EnableCutting();
        }
        else
        {
            Destroy(wirePliers.gameObject);
        }
    }

    public void CheckWire()
    {
        GameObject pliers = Instantiate(this.pliers);
        int j = 0;
        StaticData.playerWirePattern = new List<int>();

        foreach (int i in wireToEdit)
        {
            if (wireSlots[i].CheckSlotStatus())
            {
                wireSlots[i].GetWireInSlot().DisableMovableStatus();
                if (wireSlots[i].GetWireSlotVal() == num_patterns[i])
                {
                    Debug.Log("Value is Correct!");

                    //StaticData.playerWirePattern.Add(wireSlots[i].GetWireSlotVal());

                    OverallUI.enabled = false;

                    ResultUI.enabled = true;



                    StaticData.isWireDone = true;

                    Debug.Log("Where is the resultUI screen???");

                    if (DataPersistenceManager.Instance != null)
                    {
                        DataPersistenceManager.Instance.SaveGame();
                        Debug.Log("Wire station completion saved to StaticData.");
                    }

                    notesManager.ToggleNotes();
                }
                else
                {
                    //Debug.Log(sparks_vfx.name);
                    foreach (Transform child in vfxList[j])
                    {
                        child.GetComponent<VfxSegment>().ToggleVFXAnimOn();
                    }

                    pliers.GetComponent<WirePliers>().GetWiresToCut(vfxList[j]);

                    StaticData.playerWirePattern.Add(wireSlots[i].GetWireSlotVal());

                    Debug.Log("Value is Wrong!");

                    StaticData.wireWrong += 1;
                    Debug.Log("Added one penalty to wire score");

                    j++;
                }
            }
        }

        if (j <= 0)
        {
            Destroy(pliers);
        }
    }

    public void ShowHints()
    {
        StartCoroutine(ToggleNumberView());
    }

    public IEnumerator ToggleNumberView()
    {
        hint_btn.GetComponent<Hint>().ChangeSpriteOpen();

        hint_btn.interactable = false;

        ValueUI.enabled = true;

        yield return new WaitForSeconds(5f);

        ValueUI.enabled = false;

        yield return new WaitForSeconds(10f);

        hint_btn.interactable = true;

        hint_btn.GetComponent<Hint>().ChangeSpriteClose();
    }
}
