using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField]
    private Sprite hint_close;

    [SerializeField]
    private Sprite hint_open;

    [SerializeField]
    private Image hint_image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ChangeSpriteOpen()
    {
        hint_image.sprite = hint_open;
    }

    public void ChangeSpriteClose()
    {
        hint_image.sprite = hint_close;
    }
}
