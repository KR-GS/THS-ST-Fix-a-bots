using UnityEngine;
using UnityEditor;

public class LoPaintMinigame : MonoBehaviour
{
    private bool dragging = false;
    private GameObject draggableObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!dragging)
                {
                    HandleClickEvent(Input.GetTouch(0).position);
                }
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (dragging)
                {
                    if (draggableObject.GetComponent<Sticker>().IsOnPart())
                    {
                        dragging = false;
                        draggableObject = null;
                    }
                    else
                    {
                        Destroy(draggableObject);
                        dragging = false;
                    }
                }
            }
            else
            {
                if (dragging)
                {
                    Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    draggableObject.transform.position = new Vector2(touchPos.x, touchPos.y);
                }
            }
        }
    }

    private void HandleClickEvent(Vector2 position)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
        if (rayHit.collider != null)
        {
            if (rayHit.transform.gameObject.TryGetComponent(out Sticker sticker))
            {
                if (sticker.IsADuplicate())
                {
                    draggableObject = sticker.gameObject;
                }
                else
                {
                    draggableObject = GameObjectUtility.DuplicateGameObject(sticker.gameObject);
                }
                dragging = true;
                Debug.Log(draggableObject.name);
            }
        }
    }
}
