using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

public class RobotPaintPart : MonoBehaviour
{
    [SerializeField]
    private int sequenceArray;

    [SerializeField]
    private GameObject testObject;

    [SerializeField]
    private GameObject defaultObj;

    private List<int> sideVal = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*
    void Awake()
    {
        testObject = new GameObject();
        testObject.name = "Added Sticker Holder";
        defaultObj = new GameObject();
        defaultObj.name = "Default Sitckers";

        testObject.transform.parent = transform;
        defaultObj.transform.parent = transform;
    }
    */

    public void AddSticker(Sticker newSticker)
    {
        newSticker.transform.SetParent(testObject.transform);
    }

    public void RemoveSticker()
    {
        Debug.Log(testObject.transform.childCount);
    }

    public void AssignSideNumber(int value)
    {
        sequenceArray = value;
    }

    public void SetStickersOnSide(Sticker[] stickerToAdd, List<int> packUsed)
    {
        //Sticker stickers = stickersToAdd.GetPackContents();
        float boxLength_L = Base_LeftVal();
        float boxLength_R = Base_RightVal();
        float boxLength_U = Base_UpVal();
        float boxLength_D = Base_DownVal();
        for (int j = 0; j< sideVal.Count; j++)
        {
            for (int i = 0; i < sideVal[j]; i++)
            {
                Debug.Log(packUsed[j]);
                GameObject sticker = Instantiate(stickerToAdd[packUsed[j]].gameObject);
                sticker.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                Vector3 newPos = new Vector3(Random.Range(boxLength_L, boxLength_R), Random.Range(boxLength_D, boxLength_U), sticker.transform.position.z);
                sticker.transform.position = newPos;
                sticker.transform.SetParent(defaultObj.transform);
                sticker.GetComponent<Sticker>().ToggleIsADuplicate();
                sticker.GetComponent<Sticker>().ToggleIsDefault();
                sticker.GetComponent<Sticker>().SetDefaultPos(newPos);
                sticker.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Sticker sticker))
        {
            if (!sticker.IsADefault())
            {
                AddSticker(sticker);
                sticker.transform.GetComponent<SpriteRenderer>().sortingOrder = 2;
                sticker.SetPart(sequenceArray);
            }
            sticker.ToggleIsOnPart();
            Debug.Log("Sticker stuck");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Sticker sticker))
        {
            if (!sticker.IsADefault())
            {
                RemoveSticker();
                Debug.Log("Sticker stuck");
            }
            else
            {
                Debug.Log("Cannot remove");
            }
            sticker.ToggleIsOnPart();
        }
    }

    public void ClearStickersOnSide()
    {
        for(int i=0; i < testObject.transform.childCount; i++)
        {
            Destroy(testObject.transform.GetChild(i).gameObject);
        }
    }

    public void SetSideValue(int value)
    {
        sideVal.Add(value); 
    }

    public float Base_LeftVal()
    {
        return transform.position.x - (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2) + 0.5f;
    }

    public float Base_RightVal()
    {
        return transform.position.x + (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2) - 0.5f;
    }

    public float Base_UpVal()
    {
        return transform.position.y + (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2) - 0.5f;
    }

    public float Base_DownVal()
    {
        return transform.position.y - (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2) + 0.5f;
    }

    public Transform GetDefaultHolder()
    {
        return defaultObj.transform;
    }

    public int GetCurrentStickerSideCount(int stickerType)
    {
        int total_Count = 0;

        foreach (Transform child in defaultObj.transform)
        {
            if (child.GetComponent<Sticker>().GetStickerNum() == stickerType)
            {
                total_Count++;
            }
        }

        foreach (Transform child in testObject.transform)
        {
            if (child.GetComponent<Sticker>().GetStickerNum() == stickerType)
            {
                total_Count++;
            }
        }

        return total_Count;
    }

    public bool GetStickeyTypeCount(List<int> stickerType)
    {
        int typeCount = 0;

        foreach (int type in stickerType)
        {
            foreach (Transform child in testObject.transform)
            {
                if (child.GetComponent<Sticker>().GetStickerNum() == type)
                {
                    typeCount++;
                }
            }
        }

        if(testObject.transform.childCount != typeCount)
        {
            return false;
        }

        return true;
    }
}