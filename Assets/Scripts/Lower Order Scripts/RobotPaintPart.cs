using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class RobotPaintPart : MonoBehaviour
{
    [SerializeField]
    private int[] sequenceArray = new int[4];

    private int[] stickerCount = new int[4];

    private Sticker[][] stickers = new Sticker[4][];

    private int currentSide;

    [SerializeField]
    private GameObject testObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSide = 0;

        testObject = new GameObject();

        testObject.transform.parent = transform;
    }

    public void AddSticker(Sticker newSticker)
    {
        newSticker.transform.SetParent(testObject.transform);
    }

    public void RemoveSticker()
    {
        Debug.Log(testObject.transform.childCount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Sticker sticker))
        {
            AddSticker(sticker);
            sticker.ToggleIsOnPart();
            Debug.Log("Sticker stuck");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Sticker sticker))
        {
            RemoveSticker();
            sticker.ToggleIsOnPart();
            Debug.Log("Sticker stuck");
        }
    }

    public int GetCurrentStickerSideCount()
    {
        return testObject.transform.childCount;
    }

    public void ClearStickersOnSide()
    {
        for(int i=0; i < testObject.transform.childCount; i++)
        {
            Destroy(testObject.transform.GetChild(i).gameObject);
        }
    }
}