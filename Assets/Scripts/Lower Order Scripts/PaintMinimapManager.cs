using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PaintMinimapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject minimapSelectionObj;

    [SerializeField]
    private GameObject minimapOrig;

    [SerializeField]
    private int miniMapLength;

    private int currentMinimap = 0;

    private RenderTexture[] miniMapRT;

    private GameObject[] minimapArr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        miniMapRT = new RenderTexture[miniMapLength];
        minimapArr = new GameObject[miniMapLength];
        minimapArr[0] = minimapOrig;
        minimapSelectionObj.SetActive(false);
        for (int i=1; i< miniMapLength; i++)
        {
            minimapArr[i] = Instantiate(minimapOrig, minimapOrig.transform.parent);
            
        }

        for(int i=0; i< miniMapLength; i++)
        {
            miniMapRT[i] = new RenderTexture(250, 250, 16);
            minimapArr[i].GetComponent<RawImage>().texture = miniMapRT[i];
            
        }

        StartCoroutine(WaitToProcess());
    }

    void Start()
    {
        for (int i = 0; i < miniMapLength; i++)
        {
            minimapArr[i].GetComponent<Minimap>().SetValue(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WaitToProcess()
    {
        yield return new WaitForEndOfFrame();
        minimapSelectionObj.transform.position = minimapArr[currentMinimap].transform.position;
        minimapSelectionObj.SetActive(true);
    }

    public void ChangeSelectedLeftSide()
    {
        if (currentMinimap > 0)
        {
            currentMinimap--;
            minimapSelectionObj.transform.position = minimapArr[currentMinimap].transform.position;
        }
    }

    public void ChangeSelectedRightSide()
    {
        if (currentMinimap < miniMapLength-1)
        {
            currentMinimap++;
            minimapSelectionObj.transform.position = minimapArr[currentMinimap].transform.position;
        }
    }

    public void ChangeSelectedSide(int val)
    {
        Debug.Log("testing");
        currentMinimap = val;
        Debug.Log(val);
        minimapSelectionObj.transform.position = minimapArr[currentMinimap].transform.position;
    }

    public RenderTexture[] GetGeneratedRT() {
        return miniMapRT;
    }
}
