using UnityEngine;
using UnityEngine.UI;


public class CraftController : MonoBehaviour
{
    private InventoryController _inventoryController;

    [System.Serializable]
    public struct ItemBluePrint
    {
        public Button CraftItemButton;
        public ItemData Item;

        public int Woods;
        public int Stones;
    }

    [SerializeField] private ItemBluePrint[] ItemsBluePrints;

    private void Awake()
    {
        _inventoryController = GetComponent<InventoryController>();

        for (int i = 0; i < ItemsBluePrints.Length; i++)
        {
            int index = i;
            ItemsBluePrints[i].CraftItemButton.onClick.AddListener(() => CraftItem(index));
        }
    }

    private void CraftItem(int index)
    {
        var item = ItemsBluePrints[index];
        Debug.Log($"You clicked button #{index}");
        //Debug.Log($"You clicked button #{index} for item {item.Item.name}");
    }
}
