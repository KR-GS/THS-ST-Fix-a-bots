using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

    private Color color = Color.white;

    private int wire_count = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wireParent = new GameObject("New Wire " + wire_count);

        wireParent.transform.position = originalWire.transform.position;

        wireParent.transform.SetParent(transform);

        createdWireChild = new List<GameObject>(generalWireScript.ChangeWireValue(1, originalWire, wireParent));

        //createdWireChild[0].transform.localPosition = new Vector3(0, 0, 0);

        //createdWireChild[0].name = "0";

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
                if(color != Color.white)
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

        int wireTotal = 0;
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
            else if (childClr.GetComponent<SpriteRenderer>().color == UnityEngine.Color.yellow)
            {
                yellowTotal++;
            }
        }

        int totalColored = redTotal + blueTotal + yellowTotal;

        if (totalColored == createdWireChild.Count)
        {
            wireTotal = redTotal + (blueTotal * 5) + (yellowTotal * 10);

            Debug.Log(wireTotal);

            GenerateWire(wireTotal);
        }
            
    }
    
    private void GenerateWire(int wireTotal)
    {
        originalWire.gameObject.SetActive(true);

        Vector3 originalPos = originalWire.transform.position;

        for (int i = 0; i<wireParent.transform.childCount; i++)
        {
            wireParent.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
            Destroy(wireParent.transform.GetChild(i).GetComponent<Wire>());
        }

        wireParent.AddComponent<BoxCollider2D>();

        wireParent.AddComponent<SpriteRenderer>();

        wireParent.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        wireParent.AddComponent<Wire>();

        wireParent.GetComponent<Wire>().SetWireNumber(wireTotal);

        wireParent.GetComponent<Wire>().SetComplete();

        wireParent.GetComponent<Wire>().SetMovableStatus();

        //wireParent.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        Vector2 size = new Vector2(0, wireParent.transform.GetChild(0).lossyScale.y);

        foreach(Transform child in wireParent.transform)
        {
            size += new Vector2(child.lossyScale.x, 0);
        }

        wireParent.GetComponent<BoxCollider2D>().size = size;

        //wireParent.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);

        wireParent.GetComponent<BoxCollider2D>().isTrigger = true;

        generalWireScript.ToggleGenerator();

        wireParent.transform.position = generateLocation.position;

        wireParent.transform.SetParent(generateLocation);

        wire_count++;

        wireParent = new GameObject("New Wire " + wire_count);

        wireParent.transform.position = originalPos;

        wireParent.transform.SetParent(transform);

        createdWireChild = new List<GameObject>(generalWireScript.ChangeWireValue(1, originalWire, wireParent));

        originalWire.gameObject.SetActive(false);

        color = Color.white;

        select_icon.SetActive(false);
    }

    public void ResetValue(Slider slider)
    {
        slider.value = slider.minValue;
    }
}
