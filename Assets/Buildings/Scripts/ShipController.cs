using TMPro;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI WoodBoardAmountText;
    [SerializeField] private TextMeshProUGUI StoneAmountText;
    [SerializeField] private TextMeshProUGUI IronAmountText;

    public int CurrentWoodBoardAmount = 0;
    public int CurrentStoneAmount = 0;
    public int CurrentIronAmount = 0;

    public int WoodBoardNeed = 10000;
    public int StoneNeed = 10000;
    public int IronNeed = 5000;

    private void Start()
    {
        WoodBoardAmountText.text = $"{CurrentWoodBoardAmount}/{WoodBoardNeed}";
        StoneAmountText.text = $"{CurrentStoneAmount}/{StoneNeed}";
        IronAmountText.text = $"{CurrentIronAmount}/{IronNeed}";
    }

    public void UpdateShipConditionUI()
    {
        WoodBoardAmountText.text = $"{CurrentWoodBoardAmount}/{WoodBoardNeed}";
        StoneAmountText.text = $"{CurrentStoneAmount}/{StoneNeed}";
        IronAmountText.text = $"{CurrentIronAmount}/{IronNeed}";
    }
}
