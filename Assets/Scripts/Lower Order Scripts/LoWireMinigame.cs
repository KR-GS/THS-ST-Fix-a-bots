using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

public class LoWireMinigame : MonoBehaviour
{
    [SerializeField]
    private GameObject generator;

    [SerializeField]
    private GameObject robot_part;

    private GameObject WireSlots;

    private bool isDragging;

    private int wireNoTotal;

    private GameObject wireToAdd;

    private Transform wireGeneratedPlace;

    private bool isOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wireNoTotal = 0;

        isDragging = false;
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
                    isDragging = false;

                    if (wireToAdd.GetComponent<Wire>().GetSlotStatus())
                    {
                        wireToAdd.transform.position = wireToAdd.GetComponent<Wire>().GetNewNearbyPos().position;
                    }
                }
            }
            else
            {
                if (isDragging)
                {
                    Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    wireToAdd.transform.position = new Vector2(touchPos.x, touchPos.y);
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
                if (wire.GetComplete())
                {
                    Debug.Log(wire.transform.name);
                    wireToAdd = wire.transform.gameObject;
                    isDragging = true;
                    wireGeneratedPlace = wire.transform;
                }
                    
            }
        }
    }

    public List<GameObject> ChangeWireValue(float value, Wire wireToChange, GameObject parent)
    {
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

            generatedChildren[i].transform.position = new Vector2(generatePoints[i], parent.transform.position.y);
        }

        Debug.Log("Current Value");

        return generatedChildren;
    }

    public void ToggleGenerator()
    {
        isOpen = !isOpen;
        generator.SetActive(isOpen);

        robot_part.SetActive(!isOpen);
    }
}
