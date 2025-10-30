using TMPro;
using UnityEngine;

public class GapHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private TextMeshProUGUI gapObj;

    public void SetGapVal(int gap_val, int right_val)
    {
        if (gap_val > 0)
        {
            gapObj.text = "+" + gap_val.ToString();
        } else if (gap_val < 0)
        {
            gapObj.text = gap_val.ToString();
        }
        else
        {
            gapObj.text = "+0";
        }
            

        if (gap_val != right_val || gap_val == 0)
        {
            gapObj.color = Color.yellow;
        }
        else
        {
            gapObj.color = Color.white;
        }
    }
}
