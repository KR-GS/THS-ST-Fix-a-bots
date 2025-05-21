using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class LoWireMinigame : MonoBehaviour
{
    [SerializeField]
    private Wire originalWire;

    [SerializeField]
    private Sprite instantiatedObjSprtie;

    private List<GameObject> createdWireChild = new List<GameObject>();

    private GameObject wireParent;

    private List<float> midPoints = new List<float>();

    private GameObject color;

    private bool isDragging;

    private int wireNoTotal;

    private GameObject wireCopy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDragging = false;

        wireNoTotal = 0;

        wireParent = new GameObject();

        createdWireChild.Add(Instantiate(originalWire.transform.gameObject));

        createdWireChild[0].name = "0";

        createdWireChild[0].transform.SetParent(wireParent.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                HandleClickEvent(Input.GetTouch(0).position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    Destroy(color);
                    isDragging = false;
                }
            }
            else
            {
                if (isDragging)
                {
                    Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    color.transform.position = new Vector2(touchPos.x, touchPos.y);
                }
            }
        }
    }

    private void HandleClickEvent(Vector2 position)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if (rayHit.transform.gameObject.TryGetComponent(out WireColor selectedColor))
            {
                Debug.Log(selectedColor.transform.name);
                color = Instantiate(selectedColor.transform.gameObject);
                isDragging = true;
            }
        }
    }

    public void ChangeWireValue(float value)
    {
        if (createdWireChild.Count > 0)
        {
            foreach(GameObject child in createdWireChild)
            {
                Destroy(child);
            }
        }

        createdWireChild.Clear();

        Debug.Log(createdWireChild.Count);

        int intValue = (int) value;

        float newLen = originalWire.GetComponent<SpriteRenderer>().bounds.size.x / intValue;

        midPoints = new List<float>(originalWire.GetDivisionPoints(intValue));

        Debug.Log("Number: " + midPoints.Count);

        for (int i = 0; i < intValue; i++)
        {
            createdWireChild.Add(Instantiate(originalWire.transform.gameObject));
            createdWireChild[i].transform.localScale = new Vector2(newLen, 1f);

            createdWireChild[i].name = i.ToString();

            createdWireChild[i].transform.SetParent(wireParent.transform);

            if (i%2 == 0)
            {
                createdWireChild[i].GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                createdWireChild[i].GetComponent<SpriteRenderer>().color = Color.grey;
            }

            createdWireChild[i].transform.position = new Vector2(midPoints[i], createdWireChild[i].transform.position.y);
        }

        Debug.Log("Current Value");
    }

    public void CheckColorNumber()
    {
        int redTotal = 0;
        int blueTotal = 0;
        int yellowTotal = 0;
        foreach(GameObject childClr in createdWireChild)
        {
            if(childClr.GetComponent<SpriteRenderer>().color == Color.red)
            {
                redTotal++;
            }else if(childClr.GetComponent<SpriteRenderer>().color == Color.blue)
            {
                blueTotal++;
            }
            else
            {
                yellowTotal++;
            }
        }

        wireNoTotal = redTotal + (blueTotal * 5) + (yellowTotal * 10);

        Debug.Log(wireNoTotal);
    }
}
