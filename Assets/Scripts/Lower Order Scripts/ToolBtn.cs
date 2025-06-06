using UnityEngine;

public class ToolBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject toolPrefab;

    [SerializeField]
    private int toolValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject GetToolSprite()
    {
        return toolPrefab;
    }

    public int GetToolType()
    {
        return toolValue;
    }
}
