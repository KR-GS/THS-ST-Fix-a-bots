using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEditor.ShaderGraph.Internal;

public class LoPaintMinigame : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stickerTextCounter;

    private bool dragging = false;

    private GameObject draggableObject;

    [SerializeField]
    private List<Sticker> draggedObjects = new List<Sticker>();

    private RobotPaintPart roboPart;

    private LayerMask bodyMask;

    private GameObject[] partSides = new GameObject[4];

    private int currentSide = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float posY;
        float posX;
        RenderTexture[] minimapRT = FindFirstObjectByType<PaintMinimap>().GetGeneratedRT();
        roboPart = FindFirstObjectByType<RobotPaintPart>();

        roboPart.transform.parent.GetComponentInChildren<Camera>().targetTexture = minimapRT[0];

        posY = -roboPart.transform.parent.position.y * 15;

        posX = roboPart.transform.parent.position.x;

        partSides[0] = roboPart.transform.parent.gameObject;

        for (int i = 1; i<4; i++)
        {
            partSides[i] = Instantiate(roboPart.transform.parent.gameObject);

            partSides[i].transform.position = new Vector3(posX+ ((i-1) * 5), posY, partSides[i].transform.position.z);

            partSides[i].name = roboPart.transform.parent.name+ " " + i;

            partSides[i].GetComponentInChildren<Camera>().targetTexture = minimapRT[i];
        }

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

        //stickerTextCounter.text = roboPart.GetCurrentStickerSideCount().ToString();
    }

    private void HandleClickEvent(Vector2 position)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
        if (rayHit.collider != null)
        {
            Debug.Log("Interacting with: " + rayHit.transform.name);
            if (rayHit.transform.gameObject.TryGetComponent(out Sticker sticker))
            {
                Debug.Log("111Interacting with: " + rayHit.transform.name);
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
        partSides[currentSide].GetComponentInChildren<RobotPaintPart>().ClearStickersOnSide();
    }

    public void TurnToRight()
    {
        Vector3 tempPos;
        if (currentSide < 3)
        {
            tempPos = partSides[currentSide].transform.position;
            currentSide++;
            partSides[currentSide - 1].transform.position = partSides[currentSide].transform.position;
            partSides[currentSide].transform.position = tempPos;
        }
    }

    public void TurnToLeft()
    {
        Vector3 tempPos;
        if (currentSide > 0)
        {
            tempPos = partSides[currentSide].transform.position;
            currentSide--;
            partSides[currentSide + 1].transform.position = partSides[currentSide].transform.position;
            partSides[currentSide].transform.position = tempPos;
        }
    }
}
