using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;

    [System.Serializable]
    public struct OreSlot
    {
        public Transform SpawnPosition;
        public GameObject Ore;
    }

    [SerializeField] private OreSlot[] OreSlots;

    private void Start()
    {
        for (int i = 0; i < OreSlots.Length; i++)
        {
            Instantiate(OreSlots[i].Ore, OreSlots[i].SpawnPosition.position, Quaternion.identity);
            OreSlots[i].Ore.GetComponent<OreController>()._inventoryController = inventoryController;
        }
    }


}


