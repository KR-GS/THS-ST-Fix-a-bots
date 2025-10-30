using System.Collections;
using UnityEngine;

public class PartTile : MonoBehaviour
{
    [SerializeField]
    private Transform fastenerHolder;

    [SerializeField]
    private Transform part_Sprite;

    [SerializeField]
    private float speed;

    private bool isRight = false;

    public Vector2 GetSpriteSize()
    {
        return part_Sprite.GetComponent<SpriteRenderer>().bounds.size;
    }

    public Transform GetFastenerPosition()
    {
        return fastenerHolder;
    }

    public void SetFastenerPosition(float newPos)
    {
        Vector3 position = new Vector3(0, newPos, 0);

        fastenerHolder.GetChild(0).localPosition = position;

        /*
        while (Vector3.Distance(fastenerHolder.GetChild(0).localPosition, position)>0.01)
        {
            Vector3.MoveTowards(fastenerHolder.GetChild(0).localPosition, position, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.05f);
        }
        */
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

    public Vector2 GetSpritePosition()
    {
        return part_Sprite.position;
    }
}
