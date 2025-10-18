using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject InventoryObject;

    [System.Serializable]
    public struct InventorySlot
    {
        public GameObject UiSlot;
        public ItemPrefab ItemPrefab;
        public TextMeshProUGUI QuantityText;
    }

    public InventorySlot[] HotKeysSlots;

    public InventorySlot[] InventorySlots;

    public int MaxQuantityInItem;

    public bool isInventoryOpened = false;
    
    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("Selected 1 item");
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("Selected 2 item");
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("Selected 3 item");
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("Selected 4 item");
        }
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            Debug.Log("Selected 5 item");
        }

        DropItem();

        ToggleInventory();
    }

    public void AddItemInInventory(GameObject item, Sprite ItemIcon, ItemPrefab itemPrefab, int Quantity)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i].ItemPrefab == itemPrefab)
            {
                int tempQuantity = Int32.Parse(InventorySlots[i].QuantityText.text);

                if (tempQuantity + Quantity > MaxQuantityInItem)
                {
                    continue;
                }

                InventorySlots[i].ItemPrefab = itemPrefab;
                InventorySlots[i].QuantityText.text = (tempQuantity + Quantity).ToString();

                Destroy(item);

                break;
            }
            else if (InventorySlots[i].ItemPrefab == null)
            {
                InventorySlots[i].UiSlot.GetComponent<RawImage>().texture = ItemIcon.texture;

                InventorySlots[i].ItemPrefab = itemPrefab;

                int tempQuantity = Int32.Parse(InventorySlots[i].QuantityText.text);
                InventorySlots[i].QuantityText.text = (tempQuantity + Quantity).ToString();

                Destroy(item);

                break;
            }
        }

        Debug.Log("Inventory are full");
    }

    private void DropItem()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            GameObject dropped = Instantiate(InventorySlots[0].ItemPrefab.Prefab, transform.position, Quaternion.identity);

            dropped.GetComponent<ItemController>().Quantity = Int32.Parse(InventorySlots[0].QuantityText.text);
        }
    }

    private void ToggleInventory()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isInventoryOpened = !isInventoryOpened;

            InventoryObject.SetActive(isInventoryOpened);

            CursorVisabilityController.Instance.SetCursorVisability(isInventoryOpened);
        }
    }
}
