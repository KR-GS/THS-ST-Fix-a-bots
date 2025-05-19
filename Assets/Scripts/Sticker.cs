using UnityEngine;

public class Sticker : MonoBehaviour
{
    [SerializeField]
    private string stickerType;

    [SerializeField]
    private int stickerNum;

    private Vector2 stickerPosition;

    private bool isDuplicate;

    private bool onPart;

    void Start()
    {
        stickerPosition = transform.position;
        isDuplicate = false;
        onPart = false;
    }

    public string GetStickerType()
    {
        return stickerType;
    }

    public int GetStickerNum()
    {
        return stickerNum;
    }

    public bool IsADuplicate()
    {
        return isDuplicate;
    }

    public bool IsOnPart()
    {
        return onPart;
    }

    public void ToggleIsOnPart()
    {
        onPart = !onPart;
        
    }

    public void ToggleIsADuplicate()
    {
        isDuplicate = !isDuplicate;
    }
}
