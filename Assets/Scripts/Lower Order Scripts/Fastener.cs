using UnityEngine;

public class Fastener : MonoBehaviour
{
    [SerializeField]
    private Sprite brokenSprite;

    [SerializeField]
    private Sprite fixedSprite;

    [SerializeField]
    private SpriteRenderer mainsprite;

    [SerializeField]
    private bool isMissing;


    public void SetBrokenSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = brokenSprite;
    }

    public void SetFixedSprite()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = fixedSprite;
    }

    public bool CheckIsMissing()
    {
        return isMissing;
    }
}
