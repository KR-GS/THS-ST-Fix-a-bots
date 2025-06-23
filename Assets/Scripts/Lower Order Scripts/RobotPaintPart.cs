using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class RobotPaintPart : MonoBehaviour
{
    [SerializeField]
    private int sequenceArray;

    [SerializeField]
    private GameObject testObject;

    [SerializeField]
    private GameObject defaultObj;

    private int sideVal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        testObject = new GameObject();
        testObject.name = "Added Sticker Holder";
        defaultObj = new GameObject();
        defaultObj.name = "Default Sitckers";

        testObject.transform.parent = transform;
        defaultObj.transform.parent = transform;
    }

    public void AddSticker(Sticker newSticker)
    {
        newSticker.transform.SetParent(testObject.transform);
    }

    public void RemoveSticker()
    {
        Debug.Log(testObject.transform.childCount);
    }

    public void SetStickersOnSide(StickerPack stickersToAdd)
    {
        Sticker[] stickers = stickersToAdd.GetPackContents();
        float boxLength_L = transform.position.x - (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2);
        float boxLength_R = transform.position.x + (transform.GetComponent<SpriteRenderer>().bounds.size.x / 2);
        float boxLength_U = transform.position.y + (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        float boxLength_D = transform.position.y - (transform.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        for (int i = 0; i < sideVal; i++)
        {
            
            GameObject sticker = Instantiate(stickers[Random.Range(0, stickers.Length)].gameObject);
            Vector3 newPos = new Vector3(Random.Range(boxLength_L, boxLength_R), Random.Range(boxLength_D, boxLength_U), sticker.transform.position.z);
            sticker.transform.position = newPos;
            sticker.transform.SetParent(defaultObj.transform);
            sticker.GetComponent<Sticker>().ToggleIsADuplicate();
            sticker.GetComponent<Sticker>().ToggleIsDefault();
            sticker.GetComponent<Sticker>().SetDefaultPos(newPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Sticker sticker))
        {
            if (!sticker.IsADefault())
            {
                AddSticker(sticker);
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

    public int GetCurrentStickerSideCount()
    {
        return defaultObj.transform.childCount + testObject.transform.childCount;
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
        sideVal = value; 
    }
}