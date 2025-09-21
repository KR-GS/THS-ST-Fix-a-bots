using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;


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
    private Canvas ValueUI;

    [SerializeField]
    private Wire origWire;

    [SerializeField]
    private WireSlot[] wireSlots;

    [SerializeField]
    private Canvas OverallUI;

    [SerializeField]
    private Transform sparks_vfx;

    private int[] num_patterns;

    private bool isDragging;

    private GameObject wireToAdd;

    private GameObject pliers;

    private bool isOpen = false;

    private bool isDeleting = false;

    private int item_dragged = 0;

    private Vector2 origPos_Pliers;

    private Transform vfx_obj;

    private bool onWireToCut;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generator.SetActive(true);

        Vector2 size = new Vector2(origWire.transform.lossyScale.x, origWire.transform.lossyScale.y);
        num_patterns = patternManager.ReturnPatternArray(6).ToArray();

        isDragging = false;

        int valueToChange = Random.Range(2, 6);

        for (int i = 0; i < 6; i++)
        {
            GameObject newWire = new GameObject("Default Wire " + i);
            newWire.transform.position = origWire.transform.position;
            List<GameObject> wireSegments = new List<GameObject>();
            Debug.Log("Current Int: " + num_patterns[i]);
            int red_segments = num_patterns[i];
            int total_val = red_segments;

            if (valueToChange == i)
            {
                Debug.Log("Changing Value of Index " + valueToChange);
                int diff = Random.Range(2, 3);
                red_segments = red_segments - diff;
                total_val = red_segments;
                Debug.Log("New Value: " + red_segments);
            }

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
                    wireSegments[currentSegment].GetComponent<SpriteRenderer>().color = Color.yellow;
                    currentSegment++;
                }
            }

            if (blue_segments > 0)
            {
                for (int j = 0; j < blue_segments; j++)
                {
                    wireSegments[currentSegment].GetComponent<SpriteRenderer>().color = Color.blue;
                    currentSegment++;
                }
            }

            if (red_segments > 0)
            {
                for (int j = 0; j < red_segments; j++)
                {
                    wireSegments[currentSegment].GetComponent<SpriteRenderer>().color = Color.red;
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
            newWire.GetComponent<Wire>().SetWireNumber(total_val);
            newWire.GetComponent<Wire>().SetComplete();
            if (valueToChange == i)
            {
                newWire.GetComponent<Wire>().SetMovableStatus();
            }
            newWire.transform.SetParent(null);
            newWire.GetComponent<BoxCollider2D>().size = size;
            newWire.transform.position = wireSlots[i].transform.position;
            newWire.transform.SetParent(wireSlots[i].transform.parent);

        }

        sparks_vfx.position = wireSlots[valueToChange].transform.position;

        generator.SetActive(isOpen);

        robot_part.SetActive(!isOpen);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Debug.Log("Hello World");
                HandleClickEvent(Input.GetTouch(0).position);
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
                            wireToAdd.transform.SetParent(null);

                            wireToAdd.GetComponent<Wire>().SetNewWirePos(wireGeneratedPlace);

                            wireToAdd.transform.position = wireGeneratedPlace.position;
                        }

                        wireToAdd = null;
                    }
                    else if (item_dragged == 2)
                    {
                        Debug.Log(origPos_Pliers);
                        Debug.Log(pliers.transform.position);

                        if (pliers.GetComponent<WirePliers>().GetIsOnPart())
                        {
                            Debug.Log("Cutting part now");
                            StartCoroutine(StartWireCutting());
                        }
                        else
                        {
                            pliers.transform.position = new Vector2(origPos_Pliers.x, origPos_Pliers.y);
                        }
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
                    else if (item_dragged == 2)
                    {
                        Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        pliers.transform.position = new Vector2(touchPos.x, touchPos.y);
                    }
                }
            }
        }
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
                if (!isDragging)
                {
                    Debug.Log("Using the pliers");
                    pliers = plier.transform.gameObject;
                    isDragging = true;
                    item_dragged = 2;
                    origPos_Pliers = plier.transform.position;
                }
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

            if (i % 2 == 0)
            {
                generatedChildren[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
            }
            else
            {
                generatedChildren[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.grey;
            }

            //generatedChildren[i].GetComponent<SpriteRenderer>().sortingOrder = 4;

            generatedChildren[i].transform.position = new Vector2(generatePoints[i], parent.transform.position.y);
        }

        Debug.Log("Current Value");

        wireToChange.gameObject.SetActive(false);

        return generatedChildren;
    }

    public void ToggleGenerator()
    {
        isOpen = !isOpen;

        isDeleting = false;

        generator.SetActive(isOpen);

        robot_part.SetActive(!isOpen);

        ValueUI.enabled = !isOpen;

        OverallUI.enabled = !isOpen;
    }

    public void ToggleDelete()
    {
        isDeleting = !isDeleting;
    }

    private IEnumerator StartWireCutting()
    {
        yield return StartCoroutine(pliers.GetComponent<WirePliers>().TriggerPlierMovement(new Vector3(0,0,0), 1));

        pliers.GetComponent<WirePliers>().TriggerCuttingAnim();

        yield return new WaitForSeconds(1.5f);

        pliers.GetComponent<WirePliers>().StopParticleEmission();

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(pliers.GetComponent<WirePliers>().TriggerPlierMovement(origPos_Pliers, 5));

        //pliers.transform.position = new Vector2(origPos_Pliers.x, origPos_Pliers.y);

        pliers = null;
    }
}
