using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;

    [System.Serializable]
    public struct FurnaceSlot
    {
        public ItemData Item;
        public int Quantity;
    }

    public FurnaceSlot WoodSlot;
    public FurnaceSlot OreSlot;

    public string ObjectName;
    public string Info;

    private void Start()
    {
        _inventoryController = GameObject.Find("Player").GetComponent<InventoryController>();
    }
}
