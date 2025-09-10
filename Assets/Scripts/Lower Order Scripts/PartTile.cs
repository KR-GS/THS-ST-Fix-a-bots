using UnityEngine;

public class PartTile : MonoBehaviour
{
    [SerializeField]
    private Transform fastenerHolder;

    private bool isRight = false;

    public Vector2 GetSpriteSize()
    {
        return GetComponentInChildren<SpriteRenderer>().size;
    }

    public Transform GetFastenerPosition()
    {
        return fastenerHolder;
    }

    public void SetFastenerPosition(float newPos)
    {
        fastenerHolder.GetChild(0).localPosition = new Vector3(0, newPos, 0);
    }

    public void SetIsWrong(bool value)
    {
        isRight = value;
    }

    public bool GetIsRight()
    {
        return isRight;
    }

    public void SetAttemptSprite(Sprite resulting_sprite)
    {
        fastenerHolder.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = resulting_sprite;
    }
}
