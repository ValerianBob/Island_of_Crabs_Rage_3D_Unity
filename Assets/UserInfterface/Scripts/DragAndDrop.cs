using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private InventoryController inventoryController;
    
    private CanvasGroup _canvasGroup;

    private RectTransform rectTransform;

    [SerializeField] RectTransform[] Panels;

    private Vector2 startPosioion;
    private Vector2 pointerOffset;

    public int SlotIndex;

    public bool isHotKeySlot = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        startPosioion = rectTransform.anchoredPosition;

        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public bool IsOverPanel(PointerEventData eventData)
    {
        foreach (var panel in Panels)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                panel,
                eventData.position,
                eventData.pressEventCamera))
            {
                return true;
            }
        }

        return false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.5f;

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
        TryToDropItem(eventData);

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = startPosioion;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var draggedItem = eventData.pointerDrag.GetComponent<DragAndDrop>();

            if (draggedItem != null)
            {
                if (!draggedItem.isHotKeySlot && !isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 0);
                }
                else if (draggedItem.isHotKeySlot && isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 1);
                }
                else if (draggedItem.isHotKeySlot && !isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 2);
                }
                else if (!draggedItem.isHotKeySlot && isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 3);
                }

                Debug.Log($"Dropped item {draggedItem.SlotIndex} swapped with slot {SlotIndex}");
            }
        }
    }

    private void TryToDropItem(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag.GetComponent<DragAndDrop>();

        if (IsOverPanel(eventData))
        {
            Debug.Log("Was on Panel when left!");
        }
        else
        {
            Debug.Log("wasn't on Panel when left mouse");
            inventoryController.DropItem(SlotIndex, draggedItem.isHotKeySlot);
        }
    }
}