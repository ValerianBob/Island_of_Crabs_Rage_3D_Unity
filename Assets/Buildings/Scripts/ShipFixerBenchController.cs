using UnityEngine;

public class ShipFixerBenchController : MonoBehaviour
{
    [SerializeField] private ShipController ShipController;

    public string ObjectName;
    public string Info;

    [System.Serializable]
    public struct ShipFixerSlot
    {
        public ItemData Item;
        public int Quantity;
    }

    public ShipFixerSlot ResourceSlot;

    public void FixShip()
    {

    }
}
