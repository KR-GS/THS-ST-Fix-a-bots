using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArrowSelector : MonoBehaviour
{
    public TMP_Text labelText;
    public TMP_Text valueText;
    public Button leftButton;
    public Button rightButton;

    [SerializeField] private List<string> options = new List<string>();
    private int currentIndex = 0;

    public void Init(string label, List<string> values, int startIndex = 0)
    {
        labelText.text = label;
        options = values;
        currentIndex = Mathf.Clamp(startIndex, 0, options.Count - 1);
        UpdateValueText();

        leftButton.onClick.AddListener(PrevOption);
        rightButton.onClick.AddListener(NextOption);
    }

    private void PrevOption()
    {
        currentIndex = (currentIndex - 1 + options.Count) % options.Count;
        UpdateValueText();
    }

    private void NextOption()
    {
        currentIndex = (currentIndex + 1) % options.Count;
        UpdateValueText();
    }

    private void UpdateValueText()
    {
        valueText.text = options[currentIndex];
    }

    public string GetCurrentValue()
    {
        return options[currentIndex];
    }
}