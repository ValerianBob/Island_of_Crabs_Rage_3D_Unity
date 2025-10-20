using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject Inventory;

    [SerializeField] private GameObject DropPoint;

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
    
    public bool DropItem(int index)
    {
        if (index >= 0 && index <= Slots.Length )
        {
            if (Slots[index].Item != null)
            {
                GameObject ItemObjectToDrop = Instantiate(Slots[index].Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                ItemObjectToDrop.GetComponent<ItemController>().Quantity= Slots[index].Quantity;

                Slots[index].ItemIcon.texture = null;
                Slots[index].Item = null;
                Slots[index].Quantity = 0;
                Slots[index].QuantityText.text = "0";

                return true;
            }
            else
            {
                Debug.Log("Item is empty");

                return false;
            }
        }
        else
        {
            Debug.Log("Invalid index");
        }

        return false;
    }

    public void SwapItems(int fromIndex, int toIndex)
    {
        //Swap Images :
        Texture tempRawImage = Slots[fromIndex].ItemIcon.texture;
        Slots[fromIndex].ItemIcon.texture = Slots[toIndex].ItemIcon.texture;
        Slots[toIndex].ItemIcon.texture = tempRawImage;

        //ItemData :
        ItemData tempItem = Slots[fromIndex].Item;
        Slots[fromIndex].Item = Slots[toIndex].Item;
        Slots[toIndex].Item = tempItem;

        //Quantity :
        int tempQuantity = Slots[fromIndex].Quantity;
        Slots[fromIndex].Quantity = Slots[toIndex].Quantity;
        Slots[toIndex].Quantity = tempQuantity;

        //Quantity Text :
        Slots[fromIndex].QuantityText.text = Slots[fromIndex].Quantity.ToString();
        Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
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
