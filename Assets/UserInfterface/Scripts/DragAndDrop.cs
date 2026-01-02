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
    public bool isFurnaceWoodSlot = false;
    public bool isFurnaceOreSlot = false;
    public bool isCircularSawWoodSlot = false;
    public bool isShipFixerBenchSlot = false;

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
                if (!draggedItem.isHotKeySlot && !isHotKeySlot 
                    && !isFurnaceWoodSlot
                    && !isFurnaceOreSlot
                    && !isCircularSawWoodSlot
                    && !isShipFixerBenchSlot
                    && !draggedItem.isFurnaceWoodSlot
                    && !draggedItem.isFurnaceOreSlot
                    && !draggedItem.isCircularSawWoodSlot
                    && !draggedItem.isShipFixerBenchSlot)
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
                else if (!draggedItem.isHotKeySlot && isFurnaceWoodSlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 4);
                }
                else if (draggedItem.isFurnaceWoodSlot && !isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 5);
                }
                else if (!draggedItem.isHotKeySlot && isFurnaceOreSlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 6);
                }
                else if (draggedItem.isFurnaceOreSlot && !isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 7);
                }
                else if (!draggedItem.isHotKeySlot && isCircularSawWoodSlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 8);
                }
                else if (draggedItem.isCircularSawWoodSlot && !isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 9);
                }
                else if (!draggedItem.isHotKeySlot && isShipFixerBenchSlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 10);
                }
                else if (draggedItem.isShipFixerBenchSlot && !draggedItem.isHotKeySlot)
                {
                    inventoryController.SwapItem(draggedItem.SlotIndex, SlotIndex, 11);
                }

                SoundsController.Instance.PlayInventory(0, inventoryController.transform.position);
            }
        }
    }

    private void TryToDropItem(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag.GetComponent<DragAndDrop>();

        if (IsOverPanel(eventData))
        {
        }
        else
        {
            inventoryController.DropItem(SlotIndex, draggedItem.isHotKeySlot, draggedItem.isFurnaceWoodSlot, draggedItem.isFurnaceOreSlot,
                draggedItem.isCircularSawWoodSlot, draggedItem.isShipFixerBenchSlot);

            SoundsController.Instance.PlayInventory(0, inventoryController.transform.position);
        }
    }
}