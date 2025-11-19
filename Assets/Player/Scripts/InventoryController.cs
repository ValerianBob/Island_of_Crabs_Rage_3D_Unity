using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject Inventory;

    [SerializeField] private GameObject CraftUI;
    [SerializeField] private GameObject FurnaceUI;
    [SerializeField] private GameObject CircularSawUI;
    [SerializeField] private GameObject ShipFixerBenchUI;

    [SerializeField] private GameObject DropPoint;

    [SerializeField] private Texture EmptyIcon;

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

    [SerializeField] private GameObject[] PlayerInstrumentAndGunsPrefabs;

    private FurnaceController _currentFurnace;
    private CircularSawController _currentCircularSaw;
    private ShipFixerBenchController _currentShipFixerBench;

    public bool isMeleeItemInHand = true;

    private int _selectedItemInHotKeysIndex = -1;

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            _selectedItemInHotKeysIndex = 0;
            ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) 
        { 
            _selectedItemInHotKeysIndex = 1;
            ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) 
        {
            _selectedItemInHotKeysIndex = 2;
            ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame) 
        {
            _selectedItemInHotKeysIndex = 3;
            ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            _selectedItemInHotKeysIndex = 4;
            ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);
        }

        ToggleInventory();
    }

    private void ChangePlayerHotKeyItem(int index)
    {
        if (index < 0 || index > HotKeysSlots.Length)
        {
            return;
        }

        if (HotKeysSlots[index].Item != null)
        {
            if (HotKeysSlots[index].Item.Type == ItemType.Instrument)
            {
                isMeleeItemInHand = true;
            }
            else
            {
                isMeleeItemInHand = false;
            }

            for (int i = 0; i < PlayerInstrumentAndGunsPrefabs.Length; i++)
            {
                if (HotKeysSlots[index].Item.ObjectPrefab.name == PlayerInstrumentAndGunsPrefabs[i].name)
                {
                    PlayerInstrumentAndGunsPrefabs[i].SetActive(true);
                }
                else
                {
                    PlayerInstrumentAndGunsPrefabs[i].SetActive(false);
                }
            }
            Debug.Log($"Take {HotKeysSlots[index].Item.name} in slot {index}");
        }
        else
        {
            isMeleeItemInHand = true;

            _selectedItemInHotKeysIndex = -1;

            for (int i = 0; i < PlayerInstrumentAndGunsPrefabs.Length; i++)
            {
                PlayerInstrumentAndGunsPrefabs[i].SetActive(false);
            }

            Debug.Log("Swap on empty slot");
        }
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
            Notifications.Instance.CreateNotification("Not enough space to pick up item", Color.red);

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
                        Notifications.Instance.CreateNotification($"+ {Quantity} {slot.Item.ItemName}", Color.green);
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
                
                if (remaining <= 0)
                {
                    Notifications.Instance.CreateNotification($"+ {Quantity} {slot.Item.ItemName}", Color.green);
                    return true;
                }
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
                Notifications.Instance.CreateNotification($"+ {HotKeysSlots[i].Quantity} {HotKeysSlots[i].Item.ItemName}", Color.green);

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
    
    public bool DropItem(int index, bool isHotKeySlot, bool FurnaceWoodSlot, bool FurnaceOreSlot, bool CircularSawWoodSlot)
    {
        if (!isHotKeySlot && !FurnaceWoodSlot && !FurnaceOreSlot && !CircularSawWoodSlot)
        {
            if (index >= 0 && index <= Slots.Length)
            {
                if (Slots[index].Item != null)
                {
                    GameObject ItemObjectToDrop = Instantiate(Slots[index].Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                    ItemObjectToDrop.GetComponent<ItemController>().Quantity = Slots[index].Quantity;

                    Slots[index].ItemIcon.texture = EmptyIcon;
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
        else if (FurnaceWoodSlot && !FurnaceOreSlot && !isHotKeySlot)
        {
            if (_currentFurnace.WoodSlot.Item != null)
            {
                GameObject ItemObjectToDrop = Instantiate(_currentFurnace.WoodSlot.Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                ItemObjectToDrop.GetComponent<ItemController>().Quantity = _currentFurnace.WoodSlot.Quantity;

                UIManager.Instance.WoodFurnace.ItemIcon.texture = EmptyIcon;
                _currentFurnace.WoodSlot.Item = null;
                _currentFurnace.WoodSlot.Quantity = 0;
                UIManager.Instance.WoodFurnace.QuantityText.text = "0";

                Debug.Log("Wood dropped from furnace");

                return true;
            }

            return false;
        }
        else if (FurnaceOreSlot && !FurnaceWoodSlot && !isHotKeySlot)
        {
            if (_currentFurnace.OreSlot.Item != null)
            {
                GameObject ItemObjectToDrop = Instantiate(_currentFurnace.OreSlot.Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                ItemObjectToDrop.GetComponent<ItemController>().Quantity = _currentFurnace.OreSlot.Quantity;

                UIManager.Instance.OreFurnace.ItemIcon.texture = EmptyIcon;
                _currentFurnace.OreSlot.Item = null;
                _currentFurnace.OreSlot.Quantity = 0;
                UIManager.Instance.OreFurnace.QuantityText.text = "0";

                Debug.Log("Ore dropped from furnace");

                return true;
            }

            return false;
        }
        else if (!FurnaceOreSlot && !FurnaceWoodSlot && !isHotKeySlot && CircularSawWoodSlot)
        {
            if (_currentCircularSaw.WoodSlot.Item != null)
            {
                GameObject ItemObjectToDrop = Instantiate(_currentCircularSaw.WoodSlot.Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                ItemObjectToDrop.GetComponent<ItemController>().Quantity = _currentCircularSaw.WoodSlot.Quantity;

                UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = EmptyIcon;
                _currentCircularSaw.WoodSlot.Item = null;
                _currentCircularSaw.WoodSlot.Quantity = 0;
                UIManager.Instance.WoodCiruclarSaw.QuantityText.text = "0";

                Debug.Log("Wood dropped from CircularSaw");

                return true;
            }

            return false;
        }
        else
        {
            if (index >= 0 && index <= HotKeysSlots.Length)
            {
                if (HotKeysSlots[index].Item != null)
                {
                    GameObject ItemObjectToDrop = Instantiate(HotKeysSlots[index].Item.ObjectPrefab, DropPoint.transform.position, Quaternion.identity);

                    ItemObjectToDrop.GetComponent<ItemController>().Quantity = HotKeysSlots[index].Quantity;

                    HotKeysSlots[index].ItemIcon.texture = EmptyIcon;
                    HotKeysSlots[index].Item = null;
                    HotKeysSlots[index].Quantity = 0;

                    // Drop if it selected :
                    if (index == _selectedItemInHotKeysIndex)
                    {
                        isMeleeItemInHand = true;

                        _selectedItemInHotKeysIndex = -1;

                        for (int i = 0; i < PlayerInstrumentAndGunsPrefabs.Length; i++)
                        {
                            PlayerInstrumentAndGunsPrefabs[i].SetActive(false);
                        }

                    }

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

    /// <summary>
    /// Swap items in different inventory slots.
    /// </summary>
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

                //Swap item in hand :
                ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);

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

                    //Swap item in hand :
                    ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);
                }
                else
                {
                    Debug.Log("Can't put Resource in HotKey");
                    Notifications.Instance.CreateNotification("Can't put Resource in HotKey", Color.red);
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

                    //Swap item in hand :
                    ChangePlayerHotKeyItem(_selectedItemInHotKeysIndex);
                }
                else
                {
                    Debug.Log("Can't put Resource in HotKey");
                    Notifications.Instance.CreateNotification("Can't put Resource in HotKey", Color.red);
                }

                break;

            case 4:
                Debug.Log("Main item swap with Wood Furnace Item");

                if (Slots[fromIndex].Item.Type == ItemType.Resource && Slots[fromIndex].Item.ItemName == "Wood")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[fromIndex].ItemIcon.texture;
                    Slots[fromIndex].ItemIcon.texture = UIManager.Instance.WoodFurnace.ItemIcon.texture;
                    UIManager.Instance.WoodFurnace.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[fromIndex].Item;
                    Slots[fromIndex].Item = _currentFurnace.WoodSlot.Item;
                    _currentFurnace.WoodSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[fromIndex].Quantity;
                    Slots[fromIndex].Quantity = _currentFurnace.WoodSlot.Quantity;
                    _currentFurnace.WoodSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[fromIndex].QuantityText.text = Slots[fromIndex].Quantity.ToString();
                    UIManager.Instance.WoodFurnace.QuantityText.text = _currentFurnace.WoodSlot.Quantity.ToString();

                    ReloadFurnaceUI(_currentFurnace);
                }
                else
                {
                    Debug.Log("Can put only Wood in this slot");
                    Notifications.Instance.CreateNotification("Can put only Wood in this slot", Color.red);
                }

                break;

            case 5:
                Debug.Log("Wood Furnace Item swap with Main Item");

                if (Slots[toIndex].Item == null)
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.WoodFurnace.ItemIcon.texture;
                    UIManager.Instance.WoodFurnace.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentFurnace.WoodSlot.Item;
                    _currentFurnace.WoodSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentFurnace.WoodSlot.Quantity;
                    _currentFurnace.WoodSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.WoodFurnace.QuantityText.text = _currentFurnace.WoodSlot.Quantity.ToString();

                    ReloadFurnaceUI(_currentFurnace);
                }
                else if (Slots[toIndex].Item.Type == ItemType.Resource && Slots[toIndex].Item.ItemName == "Wood")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.WoodFurnace.ItemIcon.texture;
                    UIManager.Instance.WoodFurnace.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentFurnace.WoodSlot.Item;
                    _currentFurnace.WoodSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentFurnace.WoodSlot.Quantity;
                    _currentFurnace.WoodSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.WoodFurnace.QuantityText.text = _currentFurnace.WoodSlot.Quantity.ToString();

                    ReloadFurnaceUI(_currentFurnace);
                }
                else
                {
                    Debug.Log("Can put only Wood in this slot");
                    Notifications.Instance.CreateNotification("Can put only Wood in this slot", Color.red);
                }

                break;

            case 6:
                Debug.Log("Main Item spaw with Furnace Ore Slot");

                if (Slots[fromIndex].Item.Type == ItemType.Resource && Slots[fromIndex].Item.ItemName == "Iron Ore")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[fromIndex].ItemIcon.texture;
                    Slots[fromIndex].ItemIcon.texture = UIManager.Instance.OreFurnace.ItemIcon.texture;
                    UIManager.Instance.OreFurnace.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[fromIndex].Item;
                    Slots[fromIndex].Item = _currentFurnace.OreSlot.Item;
                    _currentFurnace.OreSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[fromIndex].Quantity;
                    Slots[fromIndex].Quantity = _currentFurnace.OreSlot.Quantity;
                    _currentFurnace.OreSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[fromIndex].QuantityText.text = Slots[fromIndex].Quantity.ToString();
                    UIManager.Instance.OreFurnace.QuantityText.text = _currentFurnace.OreSlot.Quantity.ToString();

                    ReloadFurnaceUI(_currentFurnace);
                }
                else
                {
                    Debug.Log("Can put only Iron Ore in this slot");
                    Notifications.Instance.CreateNotification("Can put only Iron Ore in this slot", Color.red);
                }

                break;

            case 7:
                Debug.Log("Ore Furnace Item swap with Main Item");

                if (Slots[toIndex].Item == null)
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.OreFurnace.ItemIcon.texture;
                    UIManager.Instance.OreFurnace.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentFurnace.OreSlot.Item;
                    _currentFurnace.OreSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentFurnace.OreSlot.Quantity;
                    _currentFurnace.OreSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.OreFurnace.QuantityText.text = _currentFurnace.OreSlot.Quantity.ToString();

                    ReloadFurnaceUI(_currentFurnace);
                }
                else if (Slots[toIndex].Item.Type == ItemType.Resource && Slots[toIndex].Item.ItemName == "Iron Ore")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.OreFurnace.ItemIcon.texture;
                    UIManager.Instance.OreFurnace.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentFurnace.OreSlot.Item;
                    _currentFurnace.OreSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentFurnace.OreSlot.Quantity;
                    _currentFurnace.OreSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.OreFurnace.QuantityText.text = _currentFurnace.OreSlot.Quantity.ToString();

                    ReloadFurnaceUI(_currentFurnace);
                }
                else
                {
                    Debug.Log("Can put only Iron Ore in this slot");
                    Notifications.Instance.CreateNotification("Can put only Iron Ore in this slot", Color.red);
                }

                break;

            case 8:
                Debug.Log("Main Item swap with CircularSaw Wood");

                if (Slots[fromIndex].Item.Type == ItemType.Resource && Slots[fromIndex].Item.ItemName == "Wood")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[fromIndex].ItemIcon.texture;
                    Slots[fromIndex].ItemIcon.texture = UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture;
                    UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[fromIndex].Item;
                    Slots[fromIndex].Item = _currentCircularSaw.WoodSlot.Item;
                    _currentCircularSaw.WoodSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[fromIndex].Quantity;
                    Slots[fromIndex].Quantity = _currentCircularSaw.WoodSlot.Quantity;
                    _currentCircularSaw.WoodSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[fromIndex].QuantityText.text = Slots[fromIndex].Quantity.ToString();
                    UIManager.Instance.WoodCiruclarSaw.QuantityText.text = _currentCircularSaw.WoodSlot.Quantity.ToString();

                    ReloadCircularSawUI(_currentCircularSaw);
                }
                else
                {
                    Debug.Log("Can put only Wood in this slot");
                    Notifications.Instance.CreateNotification("Can put only Wood in this slot", Color.red);
                }

                break;

            case 9:
                Debug.Log("CircularSaw Wood Item swap with Main Item");

                if (Slots[toIndex].Item == null)
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture;
                    UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentCircularSaw.WoodSlot.Item;
                    _currentCircularSaw.WoodSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentCircularSaw.WoodSlot.Quantity;
                    _currentCircularSaw.WoodSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.WoodCiruclarSaw.QuantityText.text = _currentCircularSaw.WoodSlot.Quantity.ToString();

                    ReloadCircularSawUI(_currentCircularSaw);
                }
                else if (Slots[toIndex].Item.Type == ItemType.Resource && Slots[toIndex].Item.ItemName == "Wood")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture;
                    UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentCircularSaw.WoodSlot.Item;
                    _currentCircularSaw.WoodSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentCircularSaw.WoodSlot.Quantity;
                    _currentCircularSaw.WoodSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.WoodCiruclarSaw.QuantityText.text = _currentCircularSaw.WoodSlot.Quantity.ToString();

                    ReloadCircularSawUI(_currentCircularSaw);
                }
                else
                {
                    Debug.Log("Can put only Wood in this slot");
                    Notifications.Instance.CreateNotification("Can put only Wood in this slot", Color.red);
                }

                break;

            case 10:
                Debug.Log("Main Item swap with ShipFixerBench Recource");

                if (Slots[fromIndex].Item.Type == ItemType.Resource && Slots[fromIndex].Item.ItemName == "Wood Board")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[fromIndex].ItemIcon.texture;
                    Slots[fromIndex].ItemIcon.texture = UIManager.Instance.RecourcesShipFixer.ItemIcon.texture;
                    UIManager.Instance.RecourcesShipFixer.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[fromIndex].Item;
                    Slots[fromIndex].Item = _currentShipFixerBench.ResourceSlot.Item;
                    _currentShipFixerBench.ResourceSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[fromIndex].Quantity;
                    Slots[fromIndex].Quantity = _currentShipFixerBench.ResourceSlot.Quantity;
                    _currentShipFixerBench.ResourceSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[fromIndex].QuantityText.text = Slots[fromIndex].Quantity.ToString();
                    UIManager.Instance.RecourcesShipFixer.QuantityText.text = _currentShipFixerBench.ResourceSlot.Quantity.ToString();

                    ReloadShipFixerUI(_currentShipFixerBench);
                }
                else
                {
                    Debug.Log("Can put only Wood Board in this slot");
                    Notifications.Instance.CreateNotification("Can put only Wood in this slot", Color.red);
                }

                break;


            case 11:
                Debug.Log("ShipFixerSlot Item swap with Main Item");

                if (Slots[toIndex].Item == null)
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.RecourcesShipFixer.ItemIcon.texture;
                    UIManager.Instance.RecourcesShipFixer.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentShipFixerBench.ResourceSlot.Item;
                    _currentShipFixerBench.ResourceSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentShipFixerBench.ResourceSlot.Quantity;
                    _currentShipFixerBench.ResourceSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.RecourcesShipFixer.QuantityText.text = _currentShipFixerBench.ResourceSlot.Quantity.ToString();

                    ReloadShipFixerUI(_currentShipFixerBench);
                }
                else if (Slots[toIndex].Item.Type == ItemType.Resource && Slots[toIndex].Item.ItemName == "Wood")
                {
                    //Swap Images :
                    Texture tempRawImage2 = Slots[toIndex].ItemIcon.texture;
                    Slots[toIndex].ItemIcon.texture = UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture;
                    UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = tempRawImage2;

                    //ItemData :
                    ItemData tempItem2 = Slots[toIndex].Item;
                    Slots[toIndex].Item = _currentCircularSaw.WoodSlot.Item;
                    _currentCircularSaw.WoodSlot.Item = tempItem2;

                    //Quantity :
                    int tempQuantity2 = Slots[toIndex].Quantity;
                    Slots[toIndex].Quantity = _currentCircularSaw.WoodSlot.Quantity;
                    _currentCircularSaw.WoodSlot.Quantity = tempQuantity2;

                    //Quantity Text :
                    Slots[toIndex].QuantityText.text = Slots[toIndex].Quantity.ToString();
                    UIManager.Instance.WoodCiruclarSaw.QuantityText.text = _currentCircularSaw.WoodSlot.Quantity.ToString();

                    ReloadCircularSawUI(_currentCircularSaw);
                }
                else
                {
                    Debug.Log("Can put only Wood in this slot");
                    Notifications.Instance.CreateNotification("Can put only Wood in this slot", Color.red);
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
                        Slots[i].ItemIcon.texture = EmptyIcon;
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
    private void ReloadFurnaceUI(FurnaceController currentFurnace)
    {
        if (currentFurnace.WoodSlot.Item != null)
        {
            Debug.Log("Reload Furnace Wood UI");

            UIManager.Instance.WoodFurnace.ItemIcon.texture = currentFurnace.WoodSlot.Item.Icon.texture;
            UIManager.Instance.WoodFurnace.QuantityText.text = currentFurnace.WoodSlot.Quantity.ToString();
        }
        else
        {
            Debug.Log("Reload Furnace Wood UI to zero");

            UIManager.Instance.WoodFurnace.ItemIcon.texture = EmptyIcon;
            UIManager.Instance.WoodFurnace.QuantityText.text = "0";
        }
        if (currentFurnace.OreSlot.Item != null)
        {
            Debug.Log("Reload Furnace Ore UI");

            UIManager.Instance.OreFurnace.ItemIcon.texture = currentFurnace.OreSlot.Item.Icon.texture;
            UIManager.Instance.OreFurnace.QuantityText.text = currentFurnace.OreSlot.Quantity.ToString();
        }
        else
        {
            Debug.Log("Reload Furnace Ore UI to zero");

            UIManager.Instance.OreFurnace.ItemIcon.texture = EmptyIcon;
            UIManager.Instance.OreFurnace.QuantityText.text = "0";
        }
    }

    private void ReloadCircularSawUI(CircularSawController currentCircularSaw)
    {
        if (currentCircularSaw.WoodSlot.Item != null)
        {
            Debug.Log("Reload CircularSaw Wood UI");

            UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = currentCircularSaw.WoodSlot.Item.Icon.texture;
            UIManager.Instance.WoodCiruclarSaw.QuantityText.text = currentCircularSaw.WoodSlot.Quantity.ToString();
        }
        else
        {
            Debug.Log("Reload CircularSaw Wood UI to zero");

            UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = EmptyIcon;
            UIManager.Instance.WoodCiruclarSaw.QuantityText.text = "0";
        }
    }

    private void ReloadShipFixerUI(ShipFixerBenchController currentShipFixer)
    {
        if (currentShipFixer.ResourceSlot.Item != null)
        {
            Debug.Log("Reload ShipFixer Resource UI");

            UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = currentShipFixer.ResourceSlot.Item.Icon.texture;
            UIManager.Instance.WoodCiruclarSaw.QuantityText.text = currentShipFixer.ResourceSlot.Quantity.ToString();
        }
        else
        {
            Debug.Log("Reload ShipFixer Resource UI to zero");

            UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = EmptyIcon;
            UIManager.Instance.WoodCiruclarSaw.QuantityText.text = "0";
        }
    }

    private void ToggleInventory()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isOpened = !isOpened;
            Inventory.SetActive(isOpened);
            CraftUI.SetActive(isOpened);
            FurnaceUI.SetActive(!isOpened);
            CircularSawUI.SetActive(!isOpened);
            ShipFixerBenchUI.SetActive(!isOpened);

            if (_currentFurnace != null)
            {
                _currentFurnace.isFurnaceSelected = false;
            }

            CursorVisabilityController.Instance.SetCursorVisability(isOpened);
        }
    }

    public void ToggleFromFurnace(FurnaceController currentFurnace)
    {
        isOpened = !isOpened;

        if (currentFurnace != null)
        {
            ReloadFurnaceUI(currentFurnace);

            currentFurnace.isFurnaceSelected = isOpened;

            _currentFurnace = currentFurnace;
        }

        Inventory.SetActive(isOpened);
        CraftUI.SetActive(!isOpened);
        FurnaceUI.SetActive(isOpened);
        CircularSawUI.SetActive(!isOpened);
        ShipFixerBenchUI.SetActive(!isOpened);

        CursorVisabilityController.Instance.SetCursorVisability(isOpened);
    }

    public void ToggleFromCirculatSaw(CircularSawController currentCircular)
    {
        isOpened = !isOpened;

        if (currentCircular != null)
        {
            ReloadCircularSawUI(currentCircular);

            currentCircular.isCirculatSawSelected = isOpened;

            UIManager.Instance.CutButton.onClick.AddListener(currentCircular.CutWood);

            _currentCircularSaw = currentCircular;
        }

        Inventory.SetActive(isOpened);
        CraftUI.SetActive(!isOpened);
        FurnaceUI.SetActive(!isOpened);
        CircularSawUI.SetActive(isOpened);
        ShipFixerBenchUI.SetActive(!isOpened);

        CursorVisabilityController.Instance.SetCursorVisability(isOpened);
    }

    public void ToggleFromShipFixer(ShipFixerBenchController currentShipFixer)
    {
        isOpened = !isOpened;

        if (currentShipFixer != null)
        {
            ReloadShipFixerUI(currentShipFixer);

            UIManager.Instance.FixButton.onClick.AddListener(currentShipFixer.FixShip);

            _currentShipFixerBench = currentShipFixer;
        }

        Inventory.SetActive(isOpened);
        CraftUI.SetActive(!isOpened);
        FurnaceUI.SetActive(!isOpened);
        CircularSawUI.SetActive(!isOpened);
        ShipFixerBenchUI.SetActive(isOpened);

        CursorVisabilityController.Instance.SetCursorVisability(isOpened);
    }
}
