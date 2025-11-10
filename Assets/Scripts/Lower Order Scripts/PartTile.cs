using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PartTile : MonoBehaviour
{
    [SerializeField]
    private Transform fastenerHolder;

    [SerializeField]
    private Transform part_Sprite;

    [SerializeField]
    private float speed;

    private bool isRight = false;

    private Vector3 default_size;

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

        default_size = fastenerHolder.GetChild(0).localScale;

        /*
        while (Vector3.Distance(fastenerHolder.GetChild(0).localPosition, position)>0.01)
        {
            Vector3.MoveTowards(fastenerHolder.GetChild(0).localPosition, position, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.05f);
        }
        */
    }

    public void SetDefaultSize(Vector3 size)
    {
        default_size = size;
    }


    public void SetFastenerSize(bool zoomed)
    {
        if (zoomed)
        {
            fastenerHolder.GetChild(0).localScale = default_size;
        }
        else
        {
            fastenerHolder.GetChild(0).localScale = new Vector3(default_size.x + 0.5f, default_size.y + 0.5f, default_size.z + 0.5f);
        }
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
