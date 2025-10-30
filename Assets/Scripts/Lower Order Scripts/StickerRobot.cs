using UnityEngine;

public class StickerRobot : MonoBehaviour
{
    [SerializeField]
    private Sprite wrong_sprite;

    [SerializeField]
    private Sprite right_sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetWrongSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = wrong_sprite;
    }

    public void SetRightSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = right_sprite;
    }

    public void DefaultSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = null;
    }
}
