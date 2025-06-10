using UnityEngine;
using UnityEngine.UI;

public class TimePeriodButton : MonoBehaviour
{
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
        
    }

    public void SetHighlighted(bool selected)
    {
        buttonImage.color = selected ? Color.yellow : Color.white;
    }

    public void SetGreen()
    {
        buttonImage.color = Color.green;
        Image changeImg = Button.GetComponent<Image>();
        changeImg.sprite = pressedSprite;
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
