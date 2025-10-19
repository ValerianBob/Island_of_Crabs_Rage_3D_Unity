using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject Inventory;

    public bool isOpened = false;

    [System.Serializable]
    public struct InventorySlot
    {
        public RawImage ItemIcon;
        public ItemData Item;
        public int Quantity;
        public TextMeshProUGUI QuantityText;
    }

    public InventorySlot[] Slots;

    private void Update()
    {
        ToggleInventory();
    }

    public bool CanAddItem(ItemData item, int Quantity)
    {
        if (item == null)
        {
            return false;
        }

        int remaining = Quantity;

        for (int i = 0; i < Slots.Length; i++)
        {
            InventorySlot slot = Slots[i];

            if (slot.Item == item)
            {
                int space = item.MaxQuantity - slot.Quantity;
                remaining -= space;
            }
            else if (slot.Item == null)
            {
                remaining -= item.MaxQuantity;
            }

            if (remaining <= 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool AddItem(ItemData item, int Quantity)
    {
        if (item == null)
        {
            return false;
        }

        if (!CanAddItem(item, Quantity))
        {
            Debug.Log("Not enough space to pick up item !");

            return false;
        }

        int remaining = Quantity;

        if (item.isStackble)
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].Item == item && Slots[i].Quantity < item.MaxQuantity)
                {
                    InventorySlot slot = Slots[i];

                    int space = item.MaxQuantity - slot.Quantity;
                    int toAdd = Mathf.Min(space, remaining);
                    slot.Quantity += toAdd;
                    remaining -= toAdd;

                    slot.QuantityText.text = slot.Quantity.ToString();

                    Slots[i].Quantity = slot.Quantity;

                    Debug.Log("Added in existing slot");

                    if (remaining <= 0)
                    {
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i <= Slots.Length; i++)
        {
            if (Slots[i].Item == null)
            {
                InventorySlot slot = Slots[i];
                slot.Item = item;
                slot.Quantity = Mathf.Min(remaining, item.MaxQuantity);
                remaining -= slot.Quantity;

                slot.ItemIcon.texture = item.Icon.texture;
                slot.QuantityText.text = slot.Quantity.ToString();

                Slots[i].Item = item;
                Slots[i].Quantity = slot.Quantity;

                Debug.Log("Added in empty slot");

                if (remaining <= 0) return true;
            }
        }

        return false;
    }

    private void ToggleInventory()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isOpened = !isOpened;
            Inventory.SetActive(isOpened);

            CursorVisabilityController.Instance.SetCursorVisability(isOpened);
        }
    }
}
