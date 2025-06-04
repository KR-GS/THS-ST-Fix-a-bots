using UnityEngine;

public class PartTile : MonoBehaviour
{
    private SpriteRenderer partSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //partSprite = GetComponent<SpriteRenderer>();
    }

    public Vector2 GetSpriteSize()
    {
        return transform.lossyScale;
    }
}
