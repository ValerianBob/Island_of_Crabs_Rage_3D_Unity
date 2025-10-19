using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;

    [SerializeField] RectTransform Panel;

    private Vector2 startPosioion;
    private Vector2 pointerOffset;

    public int SlotIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        startPosioion = rectTransform.anchoredPosition;
    }

    public bool IsOverPanel(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            Panel,
            eventData.position,
            eventData.pressEventCamera
        );
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsOverPanel(eventData))
        {
            Debug.Log("On Panel!");
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );
        pointerOffset = rectTransform.anchoredPosition - pointerOffset;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsOverPanel(eventData))
        {
            Debug.Log("On Panel!");
        }

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        ))
        {
            rectTransform.anchoredPosition = localPoint + pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Left mouse");

        if (IsOverPanel(eventData))
        {
            Debug.Log("Was on Panel when left!");
        }
        else
        {
            Debug.Log("wasn't on Panel when left mouse");
        }

        rectTransform.anchoredPosition = startPosioion;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
