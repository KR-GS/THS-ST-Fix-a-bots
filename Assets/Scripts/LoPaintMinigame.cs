using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;

public class LoPaintMinigame : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stickerTextCounter;

    private bool dragging = false;

    private GameObject draggableObject;

    private List<Sticker> draggedObjects = new List<Sticker>();

    private RobotPaintPart roboPart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roboPart = FindFirstObjectByType<RobotPaintPart>();
        Debug.Log(roboPart.name);
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
                        draggedObjects.Remove(draggableObject.GetComponent<Sticker>());
                        Destroy(draggableObject);
                        dragging = false;
                    }
                    stickerTextCounter.text = roboPart.GetCurrentStickerSideCount().ToString();
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
                if (sticker.IsOnPart())
                {
                    Debug.Log("This is on the robot");
                    draggableObject = sticker.gameObject;
                }
                else
                {
                    draggableObject = Instantiate(sticker.transform.gameObject);
                    draggableObject.GetComponent<Sticker>().ToggleIsADuplicate();
                    Debug.Log(draggableObject.GetComponent<Sticker>().IsADuplicate());
                    draggedObjects.Add(sticker);
                }
                dragging = true;
                Debug.Log(draggableObject.name);
            }
        }
    }

    public void ClearStickers()
    {
        
    }

    public void TurnToRight()
    {
        roboPart.RotateToRight();
        stickerTextCounter.text = roboPart.GetCurrentStickerSideCount().ToString();
    }

    public void TurnToLeft()
    {
        roboPart.RotateToLeft();
        stickerTextCounter.text = roboPart.GetCurrentStickerSideCount().ToString();
    }
}
