using UnityEngine;

public class PalmaController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;

    public int health;

    public ItemData Item;

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void FarmWood(int Quantity)
    {
        health -= Quantity;
        _inventoryController.AddItem(Item, Quantity);
    }
}
