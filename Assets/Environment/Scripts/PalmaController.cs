using UnityEngine;

public class PalmaController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;

    public int health;

    public ItemData Item;

    public int Quantity;

    public int QuantityToGive;

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void FarmWood()
    {
        health -= QuantityToGive;
        _inventoryController.AddItem(Item, QuantityToGive);
    }
}
