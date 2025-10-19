using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventoryController;

public class CraftController : MonoBehaviour
{
    private InventoryController inventoryController;

    [SerializeField] private Button CraftPickAxeButton;

    [SerializeField] private GameObject[] Instruments;

    //Pick Axe Craft :
    [Header("Pick Axe Craft")]
    public int PickAxeCraftCost;
    [SerializeField] private GameObject PickAxePref;
    [SerializeField] private GameObject PickAxeOnPlayer;
    [SerializeField] private TextMeshProUGUI WoodQuantityCost;
    [SerializeField] private TextMeshProUGUI StoneQuantityCost;

    private void Start()
    {
        inventoryController = GetComponent<InventoryController>();

        CraftPickAxeButton.onClick.AddListener(CraftInstrument);

        //Pick Axe :
        WoodQuantityCost.text = PickAxeCraftCost.ToString();
        StoneQuantityCost.text = PickAxeCraftCost.ToString();
        PickAxePref.GetComponent<ItemController>().InstrumentOrGunOnPlayerObject = PickAxeOnPlayer;
    }

    private void CraftInstrument()
    {
        int WoodQuantity = 0;
        int StoneQuantity = 0;

        for (int i = 0; i < inventoryController.InventorySlots.Length; i++)
        {
            if (inventoryController.InventorySlots[i].ItemInfo == "Stick")
            {
                WoodQuantity += Int32.Parse(inventoryController.InventorySlots[i].QuantityText.text);
            }
            else if (inventoryController.InventorySlots[i].ItemInfo == "Stone")
            {
                StoneQuantity += Int32.Parse(inventoryController.InventorySlots[i].QuantityText.text);
            }
        }

        if (WoodQuantity >= PickAxeCraftCost && StoneQuantity >= PickAxeCraftCost)
        {
            for (int i = 0; i < inventoryController.InventorySlots.Length; i++)
            {
                if (inventoryController.InventorySlots[i].ItemInfo == "Stick")
                {
                    WoodQuantity += Int32.Parse(inventoryController.InventorySlots[i].QuantityText.text);
                    if (WoodQuantity >= PickAxeCraftCost)
                    {
                        WoodQuantity -= PickAxeCraftCost;
                        inventoryController.InventorySlots[i].QuantityText.text = WoodQuantity.ToString();
                    }
                }
                else if (inventoryController.InventorySlots[i].ItemInfo == "Stone")
                {
                    StoneQuantity += Int32.Parse(inventoryController.InventorySlots[i].QuantityText.text);
                    if (StoneQuantity >= PickAxeCraftCost)
                    {
                        StoneQuantity -= PickAxeCraftCost;
                        inventoryController.InventorySlots[i].QuantityText.text = StoneQuantity.ToString();
                    }
                }
            }
            inventoryController.AddItemInInventory(PickAxePref, PickAxePref.GetComponent<ItemController>().ItemIcon,
            PickAxePref.GetComponent<ItemController>().ItemPrefab, PickAxePref.GetComponent<ItemController>().Quantity,
            PickAxeOnPlayer, PickAxePref.GetComponent<ItemController>().ItemInfo);
        }
        else
        {
            Debug.Log("Not Enough items quantity");
        }
    }
}
