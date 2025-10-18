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

    public bool isInventoryOpened = false;

    private void Start()
    {
        
    }
    
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

    public void AddItemInInventory(GameObject currentItem)
    {
        if (currentItem.GetComponent<ItemController>() != null)
        {
            InventorySlots[0].UiSlot.GetComponent<RawImage>().texture = currentItem.GetComponent<ItemController>().ItemIcon.texture;

            InventorySlots[0].ItemPrefab = currentItem.GetComponent<ItemController>().ItemPrefab;

            int tempQuantity = Int32.Parse(InventorySlots[0].QuantityText.text);
            InventorySlots[0].QuantityText.text = (tempQuantity + currentItem.GetComponent<ItemController>().Quantity).ToString();
        }
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
