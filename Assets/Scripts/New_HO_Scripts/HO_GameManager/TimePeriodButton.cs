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

    public void SetSelected(bool selected)
    {
        buttonImage.color = selected ? Color.yellow : Color.white;
    }

    public void SetGreen()
    {
        isSelected = true;
        buttonImage.color = Color.green;
    }

    public bool getSelected()
    {
        return isSelected;
    }
}
