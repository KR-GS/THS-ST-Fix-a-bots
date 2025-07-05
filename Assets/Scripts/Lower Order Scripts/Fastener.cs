using UnityEngine;

public class Fastener : MonoBehaviour
{
    [SerializeField]
    private Sprite brokenSprite;

    [SerializeField]
    private Sprite fixedSprite;


    public void SetBrokenSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = brokenSprite;
    }

    public void SetFixedSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = fixedSprite;
    }
}
