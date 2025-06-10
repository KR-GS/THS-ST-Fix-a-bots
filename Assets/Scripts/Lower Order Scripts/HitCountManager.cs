using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HitCountManager : MonoBehaviour
{
    /*
    [SerializeField]
    private GameObject hitSprite;
    */

    [SerializeField]
    private int hitCount;

    [SerializeField]
    private float gapValue;

    [SerializeField]
    private int colValue;

    private GameObject newObj;

    public void IncreaseChildCount(GameObject hitCounterObject, GameObject hitSprite)
    {
        newObj = Instantiate(hitSprite, hitCounterObject.transform);
        newObj.transform.position = hitCounterObject.transform.position;
        TapIconLayout(hitCounterObject, hitSprite);
    }

    public void PresetCounter(int value, GameObject presetObject, GameObject hitSprite)
    {
        for(int i = 0; i<value; i++)
        {
            newObj = Instantiate(hitSprite, presetObject.transform);
            newObj.transform.position = presetObject.transform.localPosition;
        }

        TapIconLayout(presetObject, hitSprite);
    }

    public void TapIconLayout(GameObject parentObjectObject, GameObject hitSprite)
    {
        int childCounter = 0;

        int rowCount;

        int objChildCount = parentObjectObject.transform.childCount;

        //Debug.Log("Childcount for counter: " + objChildCount);

        float furthestPoint_left;
        float furthestPoint_up;
        float prefabRight = hitSprite.GetComponent<SpriteRenderer>().bounds.size.x + gapValue;
        float prefabDown = hitSprite.GetComponent<SpriteRenderer>().bounds.size.y + gapValue;

        float distance_LR;
        float distance_UD;
        float distance_lastRow = 0;
        float furthestPoint;

        rowCount = objChildCount / colValue;

        if (objChildCount % colValue != 0)
        {
            rowCount++;
        }

        if (objChildCount <= colValue)
        {
            distance_LR = prefabRight * (objChildCount - 1);
        }
        else
        {
            distance_LR = prefabRight * (colValue - 1);
        }

        if(objChildCount%colValue > 0)
        {
            distance_lastRow = prefabRight * ((objChildCount%colValue) - 1);
        }

        distance_UD = prefabDown * (rowCount - 1);

        //Debug.Log("Distance via multiplication: " + distance_LR);

        furthestPoint_left = -(distance_LR / 2) + newObj.transform.position.x;

        furthestPoint_up = (distance_UD / 2) + newObj.transform.position.y;

        for (int j = 0; j < rowCount; j++)
        {
            if (j==rowCount-1 && rowCount >1)
            {
                if(objChildCount % colValue > 0)
                {
                    furthestPoint_left = -(distance_lastRow / 2) + newObj.transform.position.x;
                }
            }

            for (int i = 0; i < colValue; i++)
            {
                if (childCounter <= objChildCount - 1)
                {
                    if (i > 0)
                    {
                        parentObjectObject.transform.GetChild(childCounter).position = new Vector2(prefabRight + parentObjectObject.transform.GetChild(childCounter - 1).position.x, furthestPoint_up);
                        furthestPoint = parentObjectObject.transform.GetChild(childCounter).position.x;
                    }
                    else
                    {
                        parentObjectObject.transform.GetChild(childCounter).position = new Vector2(furthestPoint_left, furthestPoint_up);
                    }
                    //Debug.Log("Element " + j + " " + i + ": " + parentObjectObject.transform.GetChild(i).position.x);
                    childCounter++;
                }
                else
                {
                    break;
                }
            }

            furthestPoint_up -= prefabDown;
        }
    }
}
