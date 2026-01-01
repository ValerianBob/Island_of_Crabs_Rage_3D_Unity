using UnityEngine;

public class ShipFixerBenchController : MonoBehaviour
{
    [SerializeField] private ShipController ShipController;

    [SerializeField] private Texture EmptyIcon;

    public string ObjectName;
    public string Info;

    [System.Serializable]
    public struct ShipFixerSlot
    {
        public ItemData Item;
        public int Quantity;
    }

    public ShipFixerSlot ResourceSlot;

    private void Start()
    {
        ShipController = GameObject.Find("ScriptsObject").GetComponent<ShipController>();
    }

    public void FixShip()
    {
        if (ResourceSlot.Item != null)
        {
            if (ResourceSlot.Item.ItemName == "Wood Board")
            {
                ShipController.CurrentWoodBoardAmount += ResourceSlot.Quantity;
            }
            else if (ResourceSlot.Item.ItemName == "Stone")
            {
                ShipController.CurrentStoneAmount += ResourceSlot.Quantity;
            }
            else if (ResourceSlot.Item.ItemName == "Iron")
            {
                ShipController.CurrentIronAmount += ResourceSlot.Quantity;
            }

            UIManager.Instance.RecourcesShipFixer.ItemIcon.texture = EmptyIcon;
            ResourceSlot.Item = null;
            ResourceSlot.Quantity = 0;
            UIManager.Instance.RecourcesShipFixer.QuantityText.text = "0";

            ShipController.UpdateShipConditionUI();
        }
        else
        {
        }
    }
}
