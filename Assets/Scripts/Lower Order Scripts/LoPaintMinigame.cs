using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LoPaintMinigame : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stickerTextCounter;

    [SerializeField]
    private PatternGameManager patternGameManager;

    private bool dragging = false;

    private GameObject draggableObject;

    [SerializeField]
    private int numOfSides;

    private GameObject[] partSides;

    private int currentSide = 0;

    private int[] numberPattern;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        partSides = new GameObject[numOfSides];
    }
    void Start()
    {
        float posY;
        float posX;
        RenderTexture[] minimapRT = FindFirstObjectByType<PaintMinimapManager>().GetGeneratedRT();
        RobotPaintPart roboPart = FindFirstObjectByType<RobotPaintPart>();
        StickerPack[] stickerPacks = FindObjectsByType<StickerPack>(FindObjectsSortMode.None);

        numberPattern = patternGameManager.ReturnPatternArray(numOfSides).ToArray();

        Debug.Log("Number of current sticker packs: " + stickerPacks.Length);
        Debug.Log("sticker pack name: " + stickerPacks[0].name);

        roboPart.transform.parent.GetComponentInChildren<Camera>().targetTexture = minimapRT[0];

        posY = -roboPart.transform.parent.position.y * 15;

        posX = roboPart.transform.parent.position.x;

        partSides[0] = roboPart.transform.parent.gameObject;

        for (int i = 1; i< numOfSides; i++)
        {
            partSides[i] = Instantiate(roboPart.transform.parent.gameObject);

            partSides[i].transform.position = new Vector3(posX, posY - ((i - 1) * 20), partSides[i].transform.position.z);

            partSides[i].name = roboPart.transform.parent.name+ " " + i;

            partSides[i].GetComponentInChildren<Camera>().targetTexture = minimapRT[i];
        }

        for (int i = 0; i<numOfSides; i++)
        {
            partSides[i].GetComponentInChildren<RobotPaintPart>().SetSideValue(numberPattern[i]);
            if (i < numOfSides-1)
            {
                partSides[i].GetComponentInChildren<RobotPaintPart>().SetStickersOnSide(stickerPacks[0]);
            }
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
                        if (draggableObject.GetComponent<Sticker>().IsADefault())
                        {
                            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                            Debug.Log(currentSide);
                            if(touchPos.x > partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_RightVal() + 0.5f)
                            {
                                touchPos.x = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_RightVal();
                            }
                            else if(touchPos.x < partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_LeftVal() - 0.5f)
                            {
                                touchPos.x = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_LeftVal();
                            }

                            if (touchPos.y > partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_UpVal() +0.5f)
                            {
                                touchPos.y = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_UpVal();
                            }
                            else if (touchPos.y < partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_DownVal() - 0.5f)
                            {
                                touchPos.y = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().Base_DownVal();
                            }

                            Vector3 newPos = new Vector3(touchPos.x, touchPos.y, draggableObject.transform.position.z);

                            draggableObject.GetComponent<Sticker>().SetDefaultPos(newPos);
                            draggableObject.transform.position = newPos;
                        }

                        draggableObject = null;
                    }
                    else
                    {
                        if (!draggableObject.GetComponent<Sticker>().IsADefault())
                        {
                            Destroy(draggableObject);
                            dragging = false;
                        }
                        else
                        {
                            draggableObject.transform.position = draggableObject.GetComponent<Sticker>().GetDefaultPos();
                            draggableObject = null; 
                            dragging = false;
                        }
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

    public void CheckSideValues()
    {
        int correctAns = 0;
        for (int i = 0; i < numOfSides; i++) 
        {
            if (partSides[i].GetComponentInChildren<RobotPaintPart>().GetCurrentStickerSideCount() == numberPattern[i])
            {
                correctAns++;
            }
        }

        if (correctAns == numOfSides)
        {
            Debug.Log("All Correct!");
        }
    }
    public void ChangeSide(int val)
    {
        Vector3 tempPos = partSides[currentSide].transform.position;
        int prevVal = currentSide;
        currentSide = val;

        partSides[prevVal].transform.position = partSides[currentSide].transform.position;
        partSides[currentSide].transform.position = tempPos;
        Transform defaultObj = partSides[currentSide].GetComponentInChildren<RobotPaintPart>().GetDefaultHolder();

        for (int i = 0; i < defaultObj.childCount; i++) 
        {
            defaultObj.GetChild(i).GetComponent<Sticker>().SetDefaultPos(Vector3.zero);
        }
    }
}
