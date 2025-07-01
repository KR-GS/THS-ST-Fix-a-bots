using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    [SerializeField]
    private Sticker[] stickerPacks;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Button[] moveBtn;

    private GameObject[] partSides;

    private int currentSide = 0;

    private int currentStickerPack = 0;

    private int[] numberPattern;

    private int packUsed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        partSides = new GameObject[numOfSides];

        partSides[0] = FindFirstObjectByType<RobotPaintPart>().transform.parent.gameObject;
    }
    void Start()
    {
        float posY;
        float posX;
        RenderTexture[] minimapRT = FindFirstObjectByType<PaintMinimapManager>().GetGeneratedRT();
        //Transform roboPart = FindFirstObjectByType<RobotPaintPart>().transform.parent;

        numberPattern = patternGameManager.ReturnPatternArray(numOfSides).ToArray();

        Debug.Log("Number of current sticker packs: " + stickerPacks.Length);
        Debug.Log("sticker pack name: " + stickerPacks[0].name);

        partSides[0].GetComponentInChildren<Camera>().targetTexture = minimapRT[0];

        posY = partSides[0].transform.position.y;

        posX = partSides[0].transform.position.x;

        for (int i = 1; i< numOfSides; i++)
        {
            partSides[i] = Instantiate(partSides[0].gameObject);

            partSides[i].transform.position = new Vector3(posX, posY - (i * 25), partSides[i].transform.position.z);

            partSides[i].name = partSides[0].name+ " " + i;

            partSides[i].GetComponentInChildren<Camera>().targetTexture = minimapRT[i];
        }

        int packToUse = Random.Range(0, stickerPacks.Length-1);

        packUsed = stickerPacks[packToUse].GetStickerNum();

        for (int i = 0; i<numOfSides; i++)
        {
            partSides[i].GetComponentInChildren<RobotPaintPart>().SetSideValue(numberPattern[i]);
            if (i < numOfSides-1)
            {
                partSides[i].GetComponentInChildren<RobotPaintPart>().SetStickersOnSide(stickerPacks[packToUse]);
            }
        }

        Debug.Log(partSides[0].name);
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
        Debug.Log("Hello from checking");
        int correctAnsNo = 0;
        int correntTypeNo = 0;
        for (int i = 0; i < numOfSides; i++) 
        {
            if (partSides[i].GetComponentInChildren<RobotPaintPart>().GetCurrentStickerSideCount() == numberPattern[i])
            {
                correctAnsNo++;
            }

            if (partSides[i].GetComponentInChildren<RobotPaintPart>().GetStickeyTypeCount(packUsed) == numberPattern[i])
            {
                correntTypeNo++;
            }
        }


        if (correctAnsNo == numOfSides)
        {
            Debug.Log("All Numbers Correct!");
        }
        else
        {
            Debug.Log("Some number's wrong");
        }

        if (correntTypeNo == numOfSides)
        {
            Debug.Log("All Types Correct!");
        }
        else
        {
            Debug.Log("Some type's wrong");
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

    public void ChangeStickerUp()
    {
        
        if (currentStickerPack > 0)
        {
            //Vector3 newPos = stickerPacks[currentStickerPack].transform.position;
            //currentStickerPack--;
            //stickerPacks[currentStickerPack+1].transform.position = stickerPacks[currentStickerPack].transform.position;
            //stickerPacks[currentStickerPack].transform.position = newPos;
            StartCoroutine(TriggerPackChange(1));
        }
    }

    public void ChangeStickerDown()
    {
        if (currentStickerPack < stickerPacks.Length-1)
        {
            Debug.Log("Hello World");
            //stickerPacks[currentStickerPack - 1].transform.position = stickerPacks[currentStickerPack].transform.position;
            //stickerPacks[currentStickerPack].transform.position = newPos;
            StartCoroutine(TriggerPackChange(-1));
        }
    }

    private IEnumerator TriggerPackChange(int val)
    {
        Debug.Log("Coroutine Triggered");

        foreach(Button button in moveBtn)
        {
            button.interactable = false;
        }

        Vector3 newPos = stickerPacks[currentStickerPack].transform.parent.position;
        
        if(val == 1)
        {
            currentStickerPack--;
        }
        else
        {
            currentStickerPack++;
        }

        while (Vector3.Distance(stickerPacks[currentStickerPack + val].transform.parent.position, stickerPacks[currentStickerPack].transform.parent.position) > 0.001f)
        {
            stickerPacks[currentStickerPack + val].transform.parent.position = Vector3.MoveTowards(stickerPacks[currentStickerPack + val].transform.parent.position, stickerPacks[currentStickerPack].transform.parent.position, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        while (Vector3.Distance(stickerPacks[currentStickerPack].transform.parent.position, newPos) > 0.001f)
        {
            stickerPacks[currentStickerPack].transform.parent.position = Vector3.MoveTowards(stickerPacks[currentStickerPack].transform.parent.position, newPos, speed * Time.deltaTime);
            yield return null;
        }

        foreach (Button button in moveBtn)
        {
            button.interactable = true;
        }
    }
}
