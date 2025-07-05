using UnityEngine;

public class ToolEvent : MonoBehaviour
{
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
        if (fastenerHeightValue != -1f)
        {
            currentFastener.SetFastenerPosition(fastenerHeightValue);
        }
        else
        {
            if (currentFastener.GetIsRight())
            {
                currentFastener.SetFastenerPosition(fastenerHeightValue);
            }
            else
            {
                currentFastener.SetFastenerPosition(0);
                currentFastener.GetFastenerPosition().GetComponentInChildren<Fastener>().SetBrokenSprite();
            }
        }

        Debug.Log("Hello World");
    }


}
