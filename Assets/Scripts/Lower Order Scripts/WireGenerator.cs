using System.Collections.Generic;
using UnityEngine;

public class WireGenerator : MonoBehaviour
{
    [SerializeField]
    private Wire originalWire;

    [SerializeField]
    private Sprite instantiatedObjSprtie;

    [SerializeField]
    private Transform generateLocation;

    [SerializeField]
    private LoWireMinigame generalWireScript;

    [SerializeField]
    private GameObject select_icon;

    private List<GameObject> createdWireChild = new List<GameObject>();

    private GameObject wireParent;

    private bool isDragging;

    private Color color;

    private int wireNoTotal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDragging = false;

        wireParent = new GameObject("Wire Parent");

        wireParent.transform.SetParent(transform);

        createdWireChild.Add(Instantiate(originalWire.transform.gameObject, wireParent.transform));

        createdWireChild[0].name = "0";

        wireParent.transform.position = originalWire.transform.position;

        select_icon.SetActive(false);
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
            /*
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    color = null;
                    isDragging = false;
                    //Destroy(color);
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
            */
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
                color = selectedColor.GetBtnColor();
                select_icon.SetActive(true);
                select_icon.transform.position = selectedColor.transform.position;
                //isDragging = true;
            }
            else if (rayHit.transform.gameObject.TryGetComponent(out Wire wire))
            {
                if(color != null)
                {
                    wire.GetComponent<SpriteRenderer>().color = color;
                }
            }
        }
    }

    public void EditWireValue(float value)
    {
        foreach (GameObject child in createdWireChild)
        {
            Destroy(child);
        }
        createdWireChild = new List<GameObject>(generalWireScript.ChangeWireValue(value, originalWire, wireParent));
    }

    public void CheckColorNumber()
    {
        int redTotal = 0;
        int blueTotal = 0;
        int yellowTotal = 0;
        foreach (GameObject childClr in createdWireChild)
        {
            if (childClr.GetComponent<SpriteRenderer>().color == UnityEngine.Color.red)
            {
                redTotal++;
            }
            else if (childClr.GetComponent<SpriteRenderer>().color == UnityEngine.Color.blue)
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

        GenerateWire();
    }
    
    private void GenerateWire()
    {
        for(int i = 0; i<wireParent.transform.childCount; i++)
        {
            wireParent.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
            wireParent.transform.GetChild(i).GetComponent<Wire>().enabled = false;
        }

        wireParent.transform.position = generateLocation.position;

        wireParent.AddComponent<BoxCollider2D>();

        wireParent.AddComponent<SpriteRenderer>();

        wireParent.AddComponent<Wire>();

        wireParent.GetComponent<Wire>().SetWireNumber(wireNoTotal);

        wireParent.GetComponent<Wire>().SetComplete();

        generalWireScript.ToggleGenerator();

        wireParent.transform.SetParent(null);
    }
}
