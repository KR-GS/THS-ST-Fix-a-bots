using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSnapper : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private float snapSpeed = 10f;

    private bool isSnapping = false;
    private float targetPosition;
    private int itemCount;
    private float itemWidth;

    void Start()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        if (content == null)
            content = scrollRect.content;
    }

    void Update()
    {
        if (isSnapping)
        {
            // Smoothly lerp to target position
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPosition,
                Time.deltaTime * snapSpeed
            );

            // Stop snapping when close enough
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = targetPosition;
                isSnapping = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isSnapping = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SnapToNearestItem();
    }

    private void SnapToNearestItem()
    {
        itemCount = content.childCount;
        if (itemCount == 0) return;

        // Calculate item width (assuming uniform spacing)
        HorizontalLayoutGroup layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
        float spacing = layoutGroup != null ? layoutGroup.spacing : 0;

        // Get first item width
        if (content.childCount > 0)
        {
            RectTransform firstItem = content.GetChild(0) as RectTransform;
            itemWidth = firstItem.rect.width + spacing;
        }

        // Calculate which item we're closest to
        float contentWidth = content.rect.width;
        float viewportWidth = scrollRect.viewport.rect.width;
        float scrollableWidth = contentWidth - viewportWidth;

        if (scrollableWidth <= 0) return;

        float currentScroll = scrollRect.horizontalNormalizedPosition;
        float currentPosition = currentScroll * scrollableWidth;

        // Find nearest item
        int nearestItem = Mathf.RoundToInt(currentPosition / itemWidth);
        nearestItem = Mathf.Clamp(nearestItem, 0, itemCount - 1);

        // Calculate target normalized position
        float targetPos = (nearestItem * itemWidth) / scrollableWidth;
        targetPosition = Mathf.Clamp01(targetPos);

        isSnapping = true;
    }
}
