using TMPro;
using UnityEngine;

public class OverviewCounter : MonoBehaviour
{
    [SerializeField]
    private GameObject[] counterArr;

    public void SetCounterVal(int count)
    {
        int counterSlot = count % 5;

        if(counterSlot == 0 && count>0)
        {
            counterSlot = 5;
        }

        for(int i=0; i<5; i++)
        {
            if (i<counterSlot)
            {
                counterArr[i].GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                counterArr[i].GetComponent<SpriteRenderer>().color = Color.gray;
            }

            if (i == counterSlot-1)
            {
                counterArr[i].GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
            }
            else
            {
                counterArr[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}
