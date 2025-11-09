using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class WireSlot : MonoBehaviour
{
    [SerializeField]
    private int slotValue;

    [SerializeField]
    private bool isOccupied;

    [SerializeField]
    private TextMeshProUGUI wireVal_Text;

    [SerializeField]
    private SpriteRenderer[] slot_Holders;

    private GameObject wireToAdd;


    void Start()
    {
        wireVal_Text.SetText("{0:00}", slotValue);

        isOccupied = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Wire wire))
        {
            if (!isOccupied)
            {
                wire.SetNewWirePos(transform);
                slotValue = wire.GetWireNumber();
                wireToAdd = wire.gameObject;
                SetWireText();
                wire.ToggleOnSlot(true);
                isOccupied = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Wire wire))
        {
            if (isOccupied && wire.gameObject == wireToAdd)
            {
                wire.SetNewWirePos(null);
                wireToAdd = null;
                slotValue = 0;
                SetWireText();
                isOccupied = false;
                wire.ToggleOnSlot(false);
            }
        }
    }

    public int GetWireSlotVal()
    {
        return slotValue;
    }

    public void SetWireText()
    {
        wireVal_Text.SetText("{0:00}", slotValue);
    }
    
    public bool CheckSlotStatus()
    {
        return isOccupied;
    }

    public void ToggleSlotStatus()
    {
        isOccupied = true;
    }

    public Wire GetWireInSlot()
    {
        return wireToAdd.GetComponent<Wire>();
    }
}
