using UnityEngine;

public class Fastener : MonoBehaviour
{
    [SerializeField]
    private Sprite brokenSprite;

    [SerializeField]
    private Sprite fixedSprite;

    [SerializeField]
    private SpriteRenderer mainsprite;


    public void SetBrokenSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = brokenSprite;
    }

    public void SetFixedSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = fixedSprite;
    }
}
