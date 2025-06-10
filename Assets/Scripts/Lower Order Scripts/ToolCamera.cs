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

    private Vector3 originalPosition;
    private float originalSize;

    public void OverheadCameraView()
    {
        originalPosition = transform.position;
        originalSize = GetComponent<Camera>().orthographicSize;

        transform.position = new Vector3(tilingManager.TileMidPoint(), (tilingManager.TileMidPoint() / 4), transform.position.z);
        GetComponent<Camera>().orthographicSize = Mathf.Ceil(tilingManager.TileMidPoint());
        zoomInCanvas.gameObject.SetActive(false);
        toolCanvas.gameObject.SetActive(false);
        overViewCanvas.gameObject.SetActive(true);
    }

    public void FocusedCameraView(float partPosition)
    {
        transform.position = new Vector3(partPosition, originalPosition.y, originalPosition.z);
        GetComponent<Camera>().orthographicSize = originalSize;

        zoomInCanvas.gameObject.SetActive(true);
        toolCanvas.gameObject.SetActive(true);
        overViewCanvas.gameObject.SetActive(false);
    }

    public void CameraTrigger(Vector3 firstFastenerPosition, float speed)
    {
        SubmitCameraMovement(firstFastenerPosition, speed*4);
        SubmitCameraZoom();
    }

    public async void SubmitCameraMovement(Vector3 firstFastenerPosition, float speed)
    {
        while (Vector3.Distance(transform.position, firstFastenerPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, firstFastenerPosition, speed * Time.deltaTime);
            await Task.Yield();
        }
    }

    public async void SubmitCameraZoom()
    {
        while (GetComponent<Camera>().orthographicSize > originalSize)
        {
            GetComponent<Camera>().orthographicSize--;
            await Task.Yield();
        }
    }
}
