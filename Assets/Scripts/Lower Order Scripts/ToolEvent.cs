using UnityEngine;

public class ToolEvent : MonoBehaviour
{
    [SerializeField]
    private Tool tool_Parent;

    private float fastenerHeightValue;
    private PartTile currentFastener;

    public void SetCurrentFastener(PartTile fastener)
    {
        currentFastener = fastener;
    }

    public void SetHeightToSet(float value)
    {
        fastenerHeightValue = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TriggerFastenerChange()
    {
        currentFastener.SetFastenerPosition(fastenerHeightValue);
        StartCoroutine(tool_Parent.TriggerToolChange(fastenerHeightValue));

        Debug.Log("Hello World");
    }


}
