using UnityEngine;
using UnityEngine.EventSystems;

public class MarkerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private TimePeriodButton previousButton;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        previousButton = originalParent.GetComponent<TimePeriodButton>();

        transform.SetParent(canvas.transform); // Bring to front
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        SpawnMarker spawner = FindFirstObjectByType<SpawnMarker>();
        canvasGroup.blocksRaycasts = true;

        GameObject dropTarget = eventData.pointerEnter;
        TimePeriodButton newButton = dropTarget != null
            ? dropTarget.GetComponentInParent<TimePeriodButton>()
            : null;

        // Logic for dragging marker from origin to button
        if (previousButton == null && newButton != null && newButton.IsPreEnabled())
        {
            transform.SetParent(newButton.transform);
            rectTransform.anchoredPosition = Vector2.zero;
            newButton.SelectButton();
            spawner.SpawnNewMarker();

        }
        // Logic for dragging marker from button to button
        else if (previousButton != null && newButton != null && newButton.IsPreEnabled())
        {
            previousButton.DeselectButton();
            transform.SetParent(newButton.transform);
            rectTransform.anchoredPosition = Vector2.zero;
            newButton.SelectButton();
        }
        // Logic for dragging marker from button to null
        else if (previousButton != null && newButton == null)
        {
            previousButton.DeselectButton();
            Destroy(gameObject);
        }
        // Logic for dragging marker from origin to null
        else
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }

    }
}
