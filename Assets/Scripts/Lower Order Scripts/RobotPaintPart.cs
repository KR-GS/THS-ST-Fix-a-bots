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

    private GameObject[] testObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSide = 0;

        testObject = new GameObject[4];

        for(int i = 0; i<4; i++)
        {
            testObject[i] = new GameObject("Side "+ (i+1).ToString());

            if(i != currentSide)
            {
                testObject[i].SetActive(false);
            }
        }
    }

    public void AddSticker(Sticker newSticker)
    {
        newSticker.transform.SetParent(testObject[currentSide].transform);
    }

    public void RemoveSticker()
    {
        Debug.Log(testObject[currentSide].transform.childCount);
    }

    public void RotateToRight()
    {
        if (currentSide < 3)
        {
            Debug.Log("Sticker Count: " + stickerCount[currentSide]);
            testObject[currentSide].SetActive(false);

            currentSide++;

            testObject[currentSide].SetActive(true);

            Debug.Log(currentSide);
        }
    }

    public void RotateToLeft()
    {
        if (currentSide > 0)
        {
            testObject[currentSide].SetActive(false);

            currentSide--;

            testObject[currentSide].SetActive(true);

            Debug.Log(currentSide);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Sticker sticker))
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
        return testObject[currentSide].transform.childCount;
    }
}
