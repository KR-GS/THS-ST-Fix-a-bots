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

    [SerializeField]
    private Color highlight;

    private Vector3 defaultPos;

    private int partOn;

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
        if(pos != Vector3.zero)
        {
            defaultPos = pos;
        }
        else
        {
            defaultPos = transform.position;
        }
            
    }

    public Vector3 GetDefaultPos()
    {
        return defaultPos;
    }

    public void SetPart(int val)
    {
        partOn = val;
    }

    public int GetPartOn()
    {
        return partOn;
    }

    public void SetIsHighlighted()
    {
        GetComponent<SpriteRenderer>().color = highlight;
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
