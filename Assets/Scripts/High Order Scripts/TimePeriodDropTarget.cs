using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimePeriodButton : MonoBehaviour, IDropHandler
{
    private Image buttonImage;
    private bool isSelected = false;
    private bool isPreEnabled = false; 
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!GetComponent<Button>().interactable)
            return;

        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<MarkerDrag>())
        {
            SelectButton();
        }
    }

     public void SetPreEnabled(bool enabled)
    {
        isPreEnabled = enabled;
        isSelected = enabled;
        buttonImage.color = enabled ? Color.green : Color.white;

        GetComponent<Button>().interactable = !enabled;
    }

    public bool IsPreEnabled()
    {
        return GetComponent<Button>().interactable;
    }
    public void DeselectButton()
    {
        isSelected = false;
        buttonImage.color = Color.white;
    }

    public void SelectButton()
    {
        if (GetComponent<Button>().interactable)
        {
            isSelected = true;
            buttonImage.color = Color.yellow;
        }
        
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}