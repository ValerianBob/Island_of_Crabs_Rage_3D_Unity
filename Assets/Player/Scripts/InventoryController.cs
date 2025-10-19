using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject InventoryObject;

    [SerializeField] private GameObject DropPoint;

    [System.Serializable]

    public struct HotKeyInventorySlot
    {
        public GameObject UiSlot;
        public ItemPrefab ItemPrefab;
        public GameObject Item;
    }

    [System.Serializable]
    public struct InventorySlot
    {
        public string ItemInfo;
        public GameObject UiSlot;
        public ItemPrefab ItemPrefab;
        public TextMeshProUGUI QuantityText;
    }

    public HotKeyInventorySlot[] HotKeysSlots;

    public InventorySlot[] InventorySlots;

    public Button[] DropButtons;

    public int MaxQuantityInItem;
    public int MaxQuantityInInstrument;

    public bool isInventoryOpened = false;

    private void Start()
    {
        for (int i = 0; i < DropButtons.Length; i++)
        {
            int index = i;
            DropButtons[i].onClick.AddListener(() => DropItem(index));
        }
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            HotKeysSlots[0].Item.SetActive(true);
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

        ToggleInventory();
    }

    public void AddItemInInventory(GameObject item, Sprite ItemIcon, ItemPrefab itemPrefab, int Quantity, GameObject InstrumentOrGunOnPlayerObject,
        string itemInfo)
    {
        if (item.CompareTag("Item"))
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
                    InventorySlots[i].ItemInfo = itemInfo;

                    Destroy(item);

                    break;
                }
                else if (InventorySlots[i].ItemPrefab == null)
                {
                    InventorySlots[i].UiSlot.GetComponent<RawImage>().texture = ItemIcon.texture;

                    InventorySlots[i].ItemPrefab = itemPrefab;

                    int tempQuantity = Int32.Parse(InventorySlots[i].QuantityText.text);
                    InventorySlots[i].QuantityText.text = (tempQuantity + Quantity).ToString();

                    InventorySlots[i].ItemInfo = itemInfo;

                    Destroy(item);

                    break;
                }
            }
        }
        else if (item.CompareTag("Instrument"))
        {
            for (int i = 0; i < HotKeysSlots.Length; i++)
            {
                if (HotKeysSlots[i].ItemPrefab == null)
                {
                    HotKeysSlots[i].UiSlot.GetComponent<RawImage>().texture = ItemIcon.texture;

                    HotKeysSlots[i].ItemPrefab = itemPrefab;

                    HotKeysSlots[i].Item = InstrumentOrGunOnPlayerObject;

                    Destroy(item);

                    break;
                }
            }
        }
    }

    private void DropItem(int index)
    {
        if (InventorySlots[index].ItemPrefab != null)
        {
            GameObject dropped = Instantiate(InventorySlots[index].ItemPrefab.Prefab, DropPoint.transform.position, Quaternion.identity);

            dropped.GetComponent<ItemController>().Quantity = Int32.Parse(InventorySlots[index].QuantityText.text);

            InventorySlots[index].UiSlot.GetComponent<RawImage>().texture = null;

            InventorySlots[index].ItemPrefab = null;

            InventorySlots[index].QuantityText.text = "0";
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
