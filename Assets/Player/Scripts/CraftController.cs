using System.Collections.Generic;
using TMPro;
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

        public TextMeshProUGUI WoodQuantityText;
        public TextMeshProUGUI StoneQuantityText;
        public TextMeshProUGUI IronQuantityText;
        public TextMeshProUGUI SulfurQuantityText;

        public int Woods;
        public int Stones;
        public int Irons;
        public int Sulfur;

        public int Quantity;

        public bool NeedWorkBench;
    }

    [SerializeField] private ItemBluePrint[] ItemsBluePrints;

    public bool isWorkBenchNear = false;

    private void Awake()
    {
        _inventoryController = GetComponent<InventoryController>();

        for (int i = 0; i < ItemsBluePrints.Length; i++)
        {
            int index = i;

            if (ItemsBluePrints[i].WoodQuantityText != null)
            {
                ItemsBluePrints[i].WoodQuantityText.text = ItemsBluePrints[i].Woods.ToString();
            }
            if (ItemsBluePrints[i].StoneQuantityText != null)
            {
                ItemsBluePrints[i].StoneQuantityText.text = ItemsBluePrints[i].Stones.ToString();
            }
            if (ItemsBluePrints[i].IronQuantityText != null)
            {
                ItemsBluePrints[i].IronQuantityText.text = ItemsBluePrints[i].Irons.ToString();
            }
            if (ItemsBluePrints[i].SulfurQuantityText != null)
            {
                ItemsBluePrints[i].SulfurQuantityText.text = ItemsBluePrints[i].Sulfur.ToString();
            }

            ItemsBluePrints[i].CraftItemButton.onClick.AddListener(() => CraftItem(index));
        }
    }

    private void CraftItem(int index)
    {
        int remainingWoods = ItemsBluePrints[index].Woods;
        int remainingStones = ItemsBluePrints[index].Stones;
        int remainingIrons = ItemsBluePrints[index].Irons;
        int remainingSulfur = ItemsBluePrints[index].Sulfur;

        int availableWoods = 0;
        int availableStones = 0;
        int availableIrons = 0;
        int availableSulfur = 0;

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
            else if (_inventoryController.Slots[i].Item.ItemName == "Iron")
            {
                availableIrons += _inventoryController.Slots[i].Quantity;
            }
            else if (_inventoryController.Slots[i].Item.ItemName == "Sulfur")
            {
                availableSulfur += _inventoryController.Slots[i].Quantity;
            }
        }

        if (availableWoods < remainingWoods || availableStones < remainingStones || availableIrons < remainingIrons)
        {
            Debug.Log("Not enough resources");
            Notifications.Instance.CreateNotification("Not enough resources", Color.red);

            Debug.Log($"Need Woods: {Mathf.Max(0, remainingWoods - availableWoods)}, " +
                $"Need Stones: {Mathf.Max(0, remainingStones - availableStones)}, " +
                $"Need Irons: {Mathf.Max(0, remainingIrons - availableIrons)}, " +
                $"Need Irons: {Mathf.Max(0, remainingSulfur - availableSulfur)}");

            return;
        }

        if (_inventoryController.IsHotKeysSlotsFull() && _inventoryController.IsSlotsFull())
        {
            Debug.Log("Hot keys and Slots are full");
            Notifications.Instance.CreateNotification("Hot keys and Slots are full", Color.red);

            return;
        }

        if (ItemsBluePrints[index].NeedWorkBench && !isWorkBenchNear)
        {
            Notifications.Instance.CreateNotification("Wrok Bench not near", Color.red);
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

            if (ItemsBluePrints[index].Irons > 0 && remainingIrons != 0)
            {
                if (_inventoryController.Slots[i].Item != null)
                {
                    if (_inventoryController.Slots[i].Item.ItemName == "Iron")
                    {
                        remainingIrons = RemoveResourceFromSlot(i, "Iron", remainingIrons, itemsIndexesToDelete);
                    }
                }
            }

            if (ItemsBluePrints[index].Irons > 0 && remainingSulfur != 0)
            {
                if (_inventoryController.Slots[i].Item != null)
                {
                    if (_inventoryController.Slots[i].Item.ItemName == "Sulfur")
                    {
                        remainingSulfur = RemoveResourceFromSlot(i, "Sulfur", remainingSulfur, itemsIndexesToDelete);
                    }
                }
            }
        }

        if (remainingWoods == 0 && remainingStones == 0 && remainingIrons == 0 && remainingSulfur == 0)
        {
            _inventoryController.ClearSlots(itemsIndexesToDelete);

            if (ItemsBluePrints[index].Item.Type == ItemType.Resource)
            {
                _inventoryController.AddItem(ItemsBluePrints[index].Item, ItemsBluePrints[index].Quantity);
            }
            else
            {
                _inventoryController.AddItemInHotKeys(ItemsBluePrints[index].Item, ItemsBluePrints[index].Quantity);
            }

            SoundsController.Instance.PlayInventory(1, transform.position);

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
