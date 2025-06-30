using UnityEngine;
using UnityEngine.UI;

public class TimePeriodButton : MonoBehaviour
{
    private Vector3 originalPosition;
    private RectTransform rectTransform;
    private float moveAmount = 10f; 
    private bool isInitialized = false;
    public int ButtonNumber;
    public bool isSelected = false;

    public bool isPreSelected = false;

    public bool wasSelected = false;
    public Sprite pressedSprite;
    public Button Button;

    private Image buttonImage;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        /*rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
        Debug.Log(originalPosition);*/
    }

    private void Start()
    {
        
    }

    public void SetHighlighted(bool selected)
    {
        /*if (!isInitialized)
        {
            originalPosition = transform.localPosition;
            isInitialized = true;
        }*/

        buttonImage.color = selected ? Color.yellow : Color.white;
        /*rectTransform.localPosition = selected
            ? originalPosition + new Vector3(0, moveAmount , 0)
            : originalPosition;*/
    }

    public void SetRed()
    {
        buttonImage.color = Color.red;
    }

    public void SetGreen()
    {
        buttonImage.color = Color.green;
        Button.GetComponent<Image>().sprite = pressedSprite;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }

    public bool GetSelected()
    {
        return isSelected;
    }

    public void SetPreSelected(bool preselected)
    {
        isPreSelected = preselected;
    }

    public bool GetPreSelected()
    {
        return isPreSelected;
    }

    public void SetWasSelected(bool wasselected)
    {
        wasSelected = wasselected;
    }

    public bool GetWasSelected()
    {
        return wasSelected;
    }
}
