using UnityEngine;

public class Hit : MonoBehaviour
{
    [SerializeField]
    private Sprite[] hit_variants;

    [SerializeField]
    private SpriteRenderer hit_sprite;

    void SetVariantSprite(int value)
    {
        hit_sprite.sprite = hit_variants[value];
    }
}
