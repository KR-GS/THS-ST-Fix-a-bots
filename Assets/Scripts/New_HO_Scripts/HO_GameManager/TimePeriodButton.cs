using UnityEngine;
using UnityEngine.UI;

public class TimePeriodButton : MonoBehaviour
{
    public int ButtonNumber;
    public bool isSelected = false;
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
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }

    public bool GetSelected()
    {
        return isSelected;
    }
}
