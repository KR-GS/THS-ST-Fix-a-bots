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

    public void SetFastenerPosition(float newPos)
    {
        fastenerHolder.GetChild(0).localPosition = new Vector3(0, newPos, 0);
    }
}
