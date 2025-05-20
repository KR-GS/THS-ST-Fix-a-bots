using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiveValue : MonoBehaviour
{
    public TextMeshProUGUI myText;
    public void Start()
    {
        string newText = StaticData.valueToKeep;
        myText.text = newText;
    }
}
