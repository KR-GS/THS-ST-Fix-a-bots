using UnityEngine;

public class AddOne : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer hit_prefab;

    [SerializeField]
    private Sprite[] sprite_variant;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetSpriteVariant(int variant)
    {
        Debug.Log("Changing +1 sprite");
        hit_prefab.sprite = sprite_variant[variant];
    }
}
