using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public TMP_Text labelText;
    public Slider slider;

    public void Init(string label, float defaultValue = 1f)
    {
        labelText.text = label;
        slider.value = defaultValue;
    }

    public float GetValue() => slider.value;
}