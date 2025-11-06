using UnityEngine;

public class Hit : MonoBehaviour
{
    [SerializeField]
    private Sprite[] hit_variants;

    [SerializeField]
    private SpriteRenderer hit_sprite;

    private int variant_value;

    public Sprite GetSpriteVariant()
    {
        return hit_variants[variant_value];
    }

    public void SetVariant(int value)
    {
        variant_value = value;

        hit_sprite.sprite = hit_variants[variant_value];
    }
}
