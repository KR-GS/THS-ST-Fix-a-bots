using UnityEngine;

public class PartTile : MonoBehaviour
{
    [SerializeField]
    private Transform fastenerHolder;

    public Vector2 GetSpriteSize()
    {
        return transform.lossyScale;
    }

    public Transform GetFastenerPosition()
    {
        return fastenerHolder;
    }
}
