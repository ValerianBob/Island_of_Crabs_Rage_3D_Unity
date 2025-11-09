using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;

    [System.Serializable]
    public struct InventorySlot
    {
        public RawImage ItemIcon;
        public ItemData Item;
        public int Quantity;
        public TextMeshProUGUI QuantityText;
    }

    public InventorySlot WoodSlot;
    public InventorySlot OreSlot;

    public string ObjectName;
    public string Info;

    private void Start()
    {
        _inventoryController = GameObject.Find("Player").GetComponent<InventoryController>();

        WoodSlot.ItemIcon = UIManager.Instance.WoodFurnace.ItemIcon;
        WoodSlot.QuantityText = UIManager.Instance.WoodFurnace.QuantityText;

        OreSlot.ItemIcon = UIManager.Instance.OreFurnace.ItemIcon;
        OreSlot.QuantityText = UIManager.Instance.OreFurnace.QuantityText;
    }

    public void OpenFurnace(FurnaceController currentFurnace)
    {
        _inventoryController.ToggleFromFurnace(currentFurnace);
    }
}
