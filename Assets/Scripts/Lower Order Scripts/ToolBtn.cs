using UnityEngine;
using UnityEngine.UI;

public class ToolBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject toolPrefab;

    [SerializeField]
    private int toolValue;

    [SerializeField]
    private Sprite selectedSprite;

    [SerializeField]
    private Sprite unselectedSprite;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject GetToolSprite()
    {
        return toolPrefab;
    }

    public int GetToolType()
    {
        return toolValue;
    }

    public void Unselect()
    {    
        GetComponent<Image>().sprite = unselectedSprite;
    }

    public void Select()
    {
        GetComponent<Image>().sprite = selectedSprite;
    }
}
