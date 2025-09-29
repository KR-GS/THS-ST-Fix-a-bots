using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private VfxSegment[] segments;

    [SerializeField]
    private int assignedSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void SetSlotNumber(int value)
    {
        assignedSlot = value;
    }

    public int GetSlotNumber()
    {
        return assignedSlot;
    }
}
