using UnityEngine;
using System.Collections.Generic;

public class Wire : MonoBehaviour
{
    private SpriteRenderer wireSprite;

    private float wireStartPoint;

    private float wireEndPoint;

    private List<float> divisionPoints = new List<float>();

    private int wireNumberTotal;

    private Color origColor;

    [SerializeField]
    private bool isCompleteWire = false;

    [SerializeField]
    private bool isOnSlot = false;

    private bool canBeMoved = false;

    private Transform newWirePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        wireSprite = GetComponent<SpriteRenderer>();
        Debug.Log(wireSprite.bounds.size.x);

        wireStartPoint = transform.position.x - ((wireSprite.bounds.size.x) / 2);
        Debug.Log(wireStartPoint);

        wireEndPoint = transform.position.x + ((wireSprite.bounds.size.x) / 2);
        Debug.Log(wireEndPoint);

        origColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetStartPoint()
    {
        return wireStartPoint;
    }

    public float GetEndPoint()
    {
        return wireEndPoint;
    }

    public float GetWireHeight()
    {
        return wireSprite.bounds.size.y;
    }

    public int GetWireNumber()
    {
        return wireNumberTotal; 
    }

    public void SetWireNumber(int number)
    {
        wireNumberTotal = number;
    }

    public bool GetComplete()
    {
         return isCompleteWire;
    }

    public bool GetMovableStatus()
    {
        return canBeMoved;
    }

    public void SetMovableStatus()
    {
        canBeMoved = !canBeMoved;
    }

    public void SetComplete()
    {
        isCompleteWire = true;
    }

    public bool CheckOnSlot()
    {
        return isOnSlot;
    }

    public void ToggleOnSlot(bool value)
    {
        isOnSlot = value;
    }

    public bool GetSlotStatus()
    {
        return newWirePos.GetComponent<WireSlot>().CheckSlotStatus();
    }

    public void SetSlotStatus()
    {
        newWirePos.gameObject.GetComponent<WireSlot>().ToggleSlotStatus();
    }

    public void SetNewWirePos(Transform newPos)
    {
        if(newPos != null)
        {
            newWirePos = newPos;
        }
        else
        {
            isOnSlot = false;
        }
    }

    public Transform GetNewNearbyPos()
    {
        return newWirePos;
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = origColor;
    }

    public List<float> GetDivisionPoints(int numDiv)
    {
        
        if (divisionPoints.Count > 0)
        {
            divisionPoints.Clear();
        }

        Debug.Log(wireSprite);
        float wireLen = wireSprite.bounds.size.x / numDiv;
        Debug.Log("Hello2");
        float currentLen = wireStartPoint;
        Debug.Log("Hello3");
        float prevPoint;
        Debug.Log("Hello4");
        float midPoint;
        Debug.Log("Hello5");
        for (int i = 0; i<numDiv; i++)
        {
            Debug.Log("Hello!");
            if (currentLen < wireEndPoint)
            {
                prevPoint = currentLen;
                currentLen += wireLen;

                midPoint = (prevPoint + currentLen) / 2;

                divisionPoints.Add(midPoint);
            }
        }

        return divisionPoints;
    }
}
