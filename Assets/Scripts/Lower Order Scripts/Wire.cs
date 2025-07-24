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

    private bool isOnSlot = false;

    private Transform newWirePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

    public void SetComplete()
    {
        isCompleteWire = true;
    }

    public bool GetSlotStatus()
    {
        return isOnSlot;
    }

    public void SetSlotStatus()
    {
        isOnSlot = true;
    }

    public void SetNewWirePos(Transform newPos)
    {
        newWirePos = newPos;
    }

    public Transform GetNewNearbyPos()
    {
        return newWirePos;
    }

    public List<float> GetDivisionPoints(int numDiv)
    {
        if (divisionPoints.Count > 0)
        {
            divisionPoints.Clear();
        }

        float wireLen = wireSprite.bounds.size.x / numDiv;
        float currentLen = wireStartPoint;

        float prevPoint;

        float midPoint;

        for(int i = 0; i<numDiv; i++)
        {
            if(currentLen < wireEndPoint)
            {
                prevPoint = currentLen;
                currentLen += wireLen;

                midPoint = (prevPoint + currentLen) / 2;

                divisionPoints.Add(midPoint);
            }
        }

        return divisionPoints;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WireColor clrChange))
        {
            Debug.Log(clrChange.GetBtnColor());
            GetComponent<SpriteRenderer>().color = clrChange.GetBtnColor();
        }
    }
}
