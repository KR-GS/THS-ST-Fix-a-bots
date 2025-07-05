using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ToolCamera : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private ToolTilingManager tilingManager;

    [Header("Canvases")]
    [SerializeField]
    private Canvas zoomInCanvas;

    [SerializeField]
    private Canvas toolCanvas;

    [SerializeField]
    private Canvas overViewCanvas;

    [SerializeField]
    private Canvas doneCanvas;

    private Vector3 originalPosition;
    private float originalSize;

    public void OverheadCameraView()
    {
        originalPosition = transform.position;
        originalSize = GetComponent<Camera>().orthographicSize;

        transform.position = new Vector3(tilingManager.TileMidPoint(), (tilingManager.TileMidPoint() / 4), transform.position.z);
        GetComponent<Camera>().orthographicSize = 17;
        zoomInCanvas.enabled = false;
        toolCanvas.enabled = false;
        overViewCanvas.enabled = true;
    }

    public void FocusedCameraView(float partPosition)
    {
        transform.position = new Vector3(partPosition, originalPosition.y, originalPosition.z);
        GetComponent<Camera>().orthographicSize = originalSize;

        zoomInCanvas.enabled = true;
        toolCanvas.enabled = true;
        overViewCanvas.enabled = false;
    }

    public void CameraTrigger(Vector3 firstFastenerPosition, float speed)
    {
        StartCoroutine(SubmitCameraMovement(firstFastenerPosition, speed*4));
        StartCoroutine(SubmitCameraZoom());
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

    public void TriggerDoneCanvas()
    {
        overViewCanvas.enabled = false;
        doneCanvas.enabled = true;
        zoomInCanvas.enabled = false;
        toolCanvas.enabled = false;
    }
}
