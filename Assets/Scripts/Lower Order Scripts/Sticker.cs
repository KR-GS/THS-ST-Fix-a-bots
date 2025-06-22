using UnityEngine;

public class Sticker : MonoBehaviour
{ 
    [SerializeField]
    private int stickerNum;

    [SerializeField]
    private bool isDuplicate = false;

    [SerializeField]
    private bool onPart = false;

    [SerializeField]
    private bool isDefault = false;

    private Vector3 defaultPos;

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

    public bool IsADefault()
    {
        return isDefault;
    }

    public void ToggleIsOnPart()
    {
        onPart = !onPart;
        Debug.Log(onPart);   
    }

    public void ToggleIsADuplicate()
    {
        isDuplicate = !isDuplicate;
    }

    public void ToggleIsDefault()
    {
        isDefault = !isDefault;
    }

    public void SetDefaultPos(Vector3 pos)
    {
        defaultPos = pos;
    }

    public Vector3 GetDefaultPos()
    {
        return defaultPos;
    }
}
