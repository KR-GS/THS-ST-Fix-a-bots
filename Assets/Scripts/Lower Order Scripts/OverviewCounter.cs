using TMPro;
using UnityEngine;

public class OverviewCounter : MonoBehaviour
{
    [SerializeField]
    private GameObject counterObj;

    public void SetCounterVal(int count, GameObject fastenerType)
    {
        if(count == 0)
        {
            counterObj.GetComponentInChildren<TextMeshProUGUI>().text = "?";
            counterObj.GetComponentInChildren<SpriteRenderer>().sprite = null;
        }
        else
        {
            counterObj.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
            counterObj.GetComponentInChildren<SpriteRenderer>().sprite = fastenerType.GetComponent<Hit>().GetSpriteVariant();
        }
    }
}
