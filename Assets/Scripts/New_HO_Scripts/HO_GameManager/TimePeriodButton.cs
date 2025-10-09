using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimePeriodButton : MonoBehaviour
{
    private Vector3 originalPosition;
    private RectTransform rectTransform;
    private TextMeshPro buttontext;
    private float moveAmount = 15f;
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
        //this.GetComponentInChildren<Text>().text = ButtonNumber.ToString();
        /*rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
        Debug.Log(originalPosition);*/
    }

    void Update()
    {
        rectTransform = GetComponent<RectTransform>();
        if (!isInitialized)
        {
            originalPosition = rectTransform.anchoredPosition;
            if (originalPosition != new Vector3(0, 0, 0))
            {
                isInitialized = true;
                Debug.Log("Btn " + ButtonNumber + " " + originalPosition);
            }

        }
    }

    private void Start()
    {

    }

    public void SetHighlighted(bool selected)
    {
        buttonImage.color = selected ? Color.yellow : Color.white;
        /*if (isInitialized)
        {
            rectTransform.anchoredPosition = selected
                ? originalPosition + new Vector3(0, moveAmount, 0)
                : originalPosition;
        }*/
    }

    public void SetHeight(bool selected)
    {
        rectTransform.anchoredPosition = selected ? originalPosition + new Vector3(0, moveAmount, 0) : originalPosition;
    }

    public void SetGray()
    {
        buttonImage.color = Color.gray;
    }

    public void SetRed()
    {
        buttonImage.color = Color.red;
    }

    public void SetBlue()
    {
        buttonImage.color = new Color32(100, 164, 237, 255);
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
    
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }
}
