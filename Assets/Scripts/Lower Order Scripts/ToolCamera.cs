using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ToolCamera : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private ToolTilingManager tilingManager;

    [Header("UI")]
    [SerializeField]
    private Canvas zoomInCanvas;

    [SerializeField]
    private Canvas toolCanvas;

    [SerializeField]
    private Canvas overViewCanvas;

    [SerializeField]
    private Canvas notesCanvas;

    [SerializeField]
    private Canvas counterCanvas;

    [SerializeField]
    private Canvas checkingCanvas;

    [Header("Sprites")]
    [SerializeField]
    private Sprite correct_sprite;

    [SerializeField]
    private Sprite wrong_sprite;

    [SerializeField]
    private GameObject loading_Prefab;

    private Vector3 originalPosition;
    private float originalSize;
    private List<GameObject> loading_icons = new List<GameObject>();

    public void OverheadCameraView()
    {
        originalPosition = transform.position;
        originalSize = GetComponent<Camera>().orthographicSize;

        transform.position = new Vector3(tilingManager.TileMidPoint(), (tilingManager.TileMidPoint() / 4), transform.position.z);
        GetComponent<Camera>().orthographicSize = 17;
        zoomInCanvas.enabled = false;
        toolCanvas.enabled = false;
        overViewCanvas.enabled = true;
        notesCanvas.enabled = true;
        counterCanvas.enabled = false;
        checkingCanvas.enabled = false;
    }

    public void FocusedCameraView(float partPosition)
    {
        transform.position = new Vector3(partPosition, originalPosition.y, originalPosition.z);
        GetComponent<Camera>().orthographicSize = originalSize;

        zoomInCanvas.enabled = true;
        toolCanvas.enabled = true;
        counterCanvas.enabled = true;
        overViewCanvas.enabled = false;
        notesCanvas.enabled = false;
    }

    public void CameraTrigger(Vector3 firstFastenerPosition, float speed)
    {
        checkingCanvas.enabled = true;
        checkingCanvas.transform.Find("Result Image").gameObject.SetActive(false);
        checkingCanvas.transform.Find("Button").gameObject.SetActive(false);
        //StartCoroutine(SubmitCameraMovement(firstFastenerPosition, speed*4));
        //StartCoroutine(SubmitCameraZoom());
    }

    public IEnumerator SubmitCameraMovement(Vector3 firstFastenerPosition, float speed)
    {
        while (Vector3.Distance(transform.position, firstFastenerPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, firstFastenerPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator SubmitCameraZoom()
    {
        while (GetComponent<Camera>().orthographicSize > originalSize)
        {
            GetComponent<Camera>().orthographicSize--;
            yield return null;
        }
    }

    public void SetResultPanel(bool result)
    {
        if (result)
        {
            checkingCanvas.transform.Find("Result Image").GetComponent<Image>().sprite = correct_sprite;
            checkingCanvas.transform.Find("Button").gameObject.SetActive(true);
        }
        else
        {
            checkingCanvas.transform.Find("Result Image").GetComponent<Image>().sprite = wrong_sprite;
        }

        checkingCanvas.transform.Find("Result Image").gameObject.SetActive(true);


    }

    public void TriggerDoneCanvas()
    {
        notesCanvas.enabled = false;
        overViewCanvas.enabled = false;
        zoomInCanvas.enabled = false;
        toolCanvas.enabled = false;
    }

    public void ToggleCanvas()
    {
        overViewCanvas.enabled = !overViewCanvas.enabled;
    }

    public void ToggleNoteCanvas()
    {
        notesCanvas.enabled = !notesCanvas.enabled;
    }

    public void ToggleCounterCanvas()
    {
        counterCanvas.enabled = !counterCanvas.enabled;
    }

    public void ToggleCheckingCanvas()
    {
        checkingCanvas.enabled = !checkingCanvas.enabled;
    }

    public void CreateLoadingIcons(Vector3 spawn_pos)
    {
        loading_icons.Add(Instantiate(loading_Prefab, checkingCanvas.transform));

        int latest_index = loading_icons.Count - 1;

        loading_icons[latest_index].transform.position = spawn_pos;
    }

    public void DeleteLoadingIcons(int icon_to_delete)
    {
        Destroy(loading_icons[icon_to_delete]);
    }

    public void ClearLoadingIcons()
    {
        loading_icons.Clear();
    }
}
