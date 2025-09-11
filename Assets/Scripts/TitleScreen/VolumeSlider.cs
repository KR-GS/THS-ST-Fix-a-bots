using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public TMP_Text labelText;
    public Slider slider;

    public void Init(string label, float defaultValue = 0)
    {
        labelText.text = label;

        slider.minValue = -80f;
        slider.maxValue = 0f;

        slider.value = Mathf.Clamp(defaultValue, slider.minValue, slider.maxValue);
    }

    public float GetValue() => slider.value;
}