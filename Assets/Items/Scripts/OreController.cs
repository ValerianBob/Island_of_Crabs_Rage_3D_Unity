using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class OreController : MonoBehaviour
{
    public InventoryController _inventoryController;

    public int Capacity;

    public ItemData Item;

    public float _recreateDelay;

    private bool isDead = false;

    private void Update()
    {
        if (Capacity <= 0 && !isDead)
        {
            isDead = true;

            StartCoroutine(RecreateDelay());
        }
    }

    public void FarmOre(int Quantity)
    {
        if (Capacity > 0)
        {
            Capacity -= Quantity;
            _inventoryController.AddItem(Item, Quantity);
        }
    }

    private void RecreateOre()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        Capacity = 100;

        isDead = false;
    }

    private IEnumerator RecreateDelay()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(_recreateDelay);

        RecreateOre();
    }
}
