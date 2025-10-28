using System;
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
    public InventorySlot[] HotKeysSlots;

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

    public bool AddItemInHotKeys(ItemData item, int Quantity)
    {
        bool isHotKeysFull = false;

        if (item == null)
        {
            Debug.Log("Item is empty");

            return false;
        }

        for (int i = 0; i < HotKeysSlots.Length; i++)
        {
            if (HotKeysSlots[i].Item == null)
            {
                HotKeysSlots[i].ItemIcon.texture = item.Icon.texture;
                HotKeysSlots[i].Item = item;
                HotKeysSlots[i].Quantity = Quantity;

                Debug.Log("Added in empty HotKeySlot");

                return true;
            }
            else
            {
                isHotKeysFull = true;
            }
        }

        if (isHotKeysFull)
        {
            Debug.Log("Not enough space in HotKeySlots inventory. Trying to add in Main Inventory :");
            
            return AddItem(item, Quantity); 
        }
        
        return false;
    }
    
    public bool DropItem(int index, bool isHotKeySlot)
    {
        if (!isHotKeySlot)
        {
            if (index >= 0 && index <= Slots.Length)
            {
                if (Slots[index].Item != null)
                {
                    GameObject ItemObjectToDrop = Instantiate(Slots[index].Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                    ItemObjectToDrop.GetComponent<ItemController>().Quantity = Slots[index].Quantity;

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
        }
        else
        {
            if (index >= 0 && index <= HotKeysSlots.Length)
            {
                if (HotKeysSlots[index].Item != null)
                {
                    GameObject ItemObjectToDrop = Instantiate(HotKeysSlots[index].Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                    ItemObjectToDrop.GetComponent<ItemController>().Quantity = HotKeysSlots[index].Quantity;

                    HotKeysSlots[index].ItemIcon.texture = null;
                    HotKeysSlots[index].Item = null;
                    HotKeysSlots[index].Quantity = 0;

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
        }

        return false;
    }

    public void SwapItem(int fromIndex, int toIndex, int TypeOfSwap)
    {
        switch (TypeOfSwap)
        {
            case 0:
                Debug.Log("Main item swap with Main Item");

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

                break;
            case 1:
                Debug.Log("HotKey item swap with HotKey Item");

                //Swap Images :
                Texture tempRawImage1 = HotKeysSlots[fromIndex].ItemIcon.texture;
                HotKeysSlots[fromIndex].ItemIcon.texture = HotKeysSlots[toIndex].ItemIcon.texture;
                HotKeysSlots[toIndex].ItemIcon.texture = tempRawImage1;

                //ItemData :
                ItemData tempItem1 = HotKeysSlots[fromIndex].Item;
                HotKeysSlots[fromIndex].Item = HotKeysSlots[toIndex].Item;
                HotKeysSlots[toIndex].Item = tempItem1;

                //Quantity :
                int tempQuantity1 = HotKeysSlots[fromIndex].Quantity;
                HotKeysSlots[fromIndex].Quantity = HotKeysSlots[toIndex].Quantity;
                HotKeysSlots[toIndex].Quantity = tempQuantity1;

                break;

            case 2:
                Debug.Log("HotKey item swap with Main Item");

                if (Slots[toIndex].Item == null || Slots[toIndex].Item.Type != ItemType.Resource)
                {
                    //Swap Images :
                    Texture tempRawImage2 = HotKeysSlots[fromIndex].ItemIcon.texture;
                    HotKeysSlots[fromIndex].ItemIcon.texture = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = HotKeysSlots[fromIndex].Item;
                    HotKeysSlots[fromIndex].Item = Slots[toIndex].Item;
                    Slots[toIndex].Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = HotKeysSlots[fromIndex].Quantity;
                    HotKeysSlots[fromIndex].Quantity = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = tempQuantity2;

                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                }
                else
                {
                    Debug.Log("Can't put Resource in HotKey");
                }
                
                break;

            case 3:
                Debug.Log("Main item swap with HotKey Item");

                if (Slots[fromIndex].Item.Type != ItemType.Resource)
                {
                    //Swap Images :
                    Texture tempRawImage2 = HotKeysSlots[toIndex].ItemIcon.texture;
                    HotKeysSlots[toIndex].ItemIcon.texture = Slots[fromIndex].ItemIcon.texture;
                    Slots[fromIndex].ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = HotKeysSlots[toIndex].Item;
                    HotKeysSlots[toIndex].Item = Slots[fromIndex].Item;
                    Slots[fromIndex].Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = HotKeysSlots[toIndex].Quantity;
                    HotKeysSlots[toIndex].Quantity = Slots[fromIndex].Quantity;
                    Slots[fromIndex].Quantity = tempQuantity2;
                }
                else
                {
                    Debug.Log("Can't put Resource in HotKey");
                }

                break;

            default:
                Debug.LogWarning("Unknown item type!");
                break;
        }
    }

    public void ClearSlots(List<int> slotsIndexes)
    {
        if (slotsIndexes.Count != 0)
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                for (int j = 0; j < slotsIndexes.Count; j++)
                {
                    if (i == slotsIndexes[j])
                    {
                        Slots[i].ItemIcon.texture = null;
                        Slots[i].Item = null;
                        Slots[i].Quantity = 0;
                        Slots[i].QuantityText.text = "0";
                    }
                }
            }
        }
    }

    public bool IsSlotsFull()
    {
        foreach (var slot in Slots)
        {
            if (slot.Item == null)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsHotKeysSlotsFull()
    {
        foreach (var slot in HotKeysSlots)
        {
            if (slot.Item == null)
            {
                return false;
            }
        }
        return true;
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
