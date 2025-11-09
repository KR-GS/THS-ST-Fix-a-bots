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

    public VfxSegment GetOppositeSide(int current_Side)
    {
        int side;

        if (current_Side == 0)
        {
            side = 1;
        }
        else
        {
            side = 0;
        }

        return segments[side];
    }

    public Vector2 GetPosition(int i)
    {
        return segments[i].transform.position;
    }

    public VfxSegment GetSegment(int i)
    {
        return segments[i];
    }
}
