using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CraftController : MonoBehaviour
{
    private InventoryController _inventoryController;

    [SerializeField] private Texture EmptyIcon;

    [System.Serializable]
    public struct ItemBluePrint
    {
        public Button CraftItemButton;
        public ItemData Item;

        public int Woods;
        public int Stones;

        public int Quantity;
    }

    [SerializeField] private ItemBluePrint[] ItemsBluePrints;

    private void Awake()
    {
        _inventoryController = GetComponent<InventoryController>();

        for (int i = 0; i < ItemsBluePrints.Length; i++)
        {
            int index = i;
            ItemsBluePrints[i].CraftItemButton.onClick.AddListener(() => CraftItem(index));
        }
    }

    private void CraftItem(int index)
    {
        int remainingWoods = ItemsBluePrints[index].Woods;
        int remainingStones = ItemsBluePrints[index].Stones;

        int availableWoods = 0;
        int availableStones = 0;

        for (int i = 0; i < _inventoryController.Slots.Length; i++)
        {
            if (_inventoryController.Slots[i].Item == null)
            {
                continue;
            }

            if (_inventoryController.Slots[i].Item.ItemName == "Wood")
            {
                availableWoods += _inventoryController.Slots[i].Quantity;
            }
            else if (_inventoryController.Slots[i].Item.ItemName == "Stone")
            {
                availableStones += _inventoryController.Slots[i].Quantity;
            }
        }

        if (availableWoods < remainingWoods || availableStones < remainingStones)
        {
            Debug.Log("Not enough resources");
            Notifications.Instance.CreateNotification("Not enough resources", Color.red);

            Debug.Log($"Need Woods: {Mathf.Max(0, remainingWoods - availableWoods)}, " +
                $"Need Stones: {Mathf.Max(0, remainingStones - availableStones)}");

            return;
        }

        if (_inventoryController.IsHotKeysSlotsFull() && _inventoryController.IsSlotsFull())
        {
            Debug.Log("Hot keys and Slots are full");
            Notifications.Instance.CreateNotification("Hot keys and Slots are full", Color.red);

            return;
        }

        List<int> itemsIndexesToDelete = new List<int>();

        for (int i = 0; i < _inventoryController.Slots.Length; i++)
        {
            if (ItemsBluePrints[index].Woods > 0 && remainingWoods != 0)
            {
                if (_inventoryController.Slots[i].Item != null)
                {
                    if (_inventoryController.Slots[i].Item.ItemName == "Wood")
                    {
                        remainingWoods = RemoveResourceFromSlot(i, "Wood", remainingWoods, itemsIndexesToDelete);
                    }
                }
            }

            if (ItemsBluePrints[index].Stones > 0 && remainingStones != 0)
            {
                if (_inventoryController.Slots[i].Item != null)
                {
                    if (_inventoryController.Slots[i].Item.ItemName == "Stone")
                    {
                        remainingStones = RemoveResourceFromSlot(i, "Stone", remainingStones, itemsIndexesToDelete);
                    }
                }
            }
        }

        if (remainingWoods == 0 && remainingStones == 0)
        {
            _inventoryController.ClearSlots(itemsIndexesToDelete);

            _inventoryController.AddItemInHotKeys(ItemsBluePrints[index].Item, ItemsBluePrints[index].Quantity);
            Debug.Log($"Item :{ItemsBluePrints[index].Item} crafted");
            //Notifications.Instance.CreateNotification($"Item :{ItemsBluePrints[index].Item} crafted", Color.green);
        }
    }

    private int RemoveResourceFromSlot(int i, string resourceName, int remainingAmount, List<int> itemsIndexesToDelete)
    {
        if (remainingAmount > _inventoryController.Slots[i].Quantity)
        {
            itemsIndexesToDelete.Add(i);
            remainingAmount -= _inventoryController.Slots[i].Quantity;
        }
        else
        {
            _inventoryController.Slots[i].Quantity -= remainingAmount;
            _inventoryController.Slots[i].QuantityText.text = _inventoryController.Slots[i].Quantity.ToString();

            remainingAmount = 0;

            if (_inventoryController.Slots[i].Quantity == 0)
            {
                _inventoryController.Slots[i].Item = null;
                _inventoryController.Slots[i].ItemIcon.texture = EmptyIcon;
            }
        }

        return remainingAmount;
    }
}
