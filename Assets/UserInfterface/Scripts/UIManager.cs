using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    [System.Serializable]
    public struct BuildSlotUI
    {
        public RawImage ItemIcon;
        public TextMeshProUGUI QuantityText;
    }

    [Header("Furnac Properties :")]
    public BuildSlotUI WoodFurnace;
    public BuildSlotUI OreFurnace;
    public TextMeshProUGUI MeltTimeText;

    [Header("CircularSaw Properties :")]
    public BuildSlotUI WoodCiruclarSaw;
    public TextMeshProUGUI CutTimeText;
    public Button CutButton;

    [Header("ShipFixerBench Properties :")]
    public BuildSlotUI RecourcesShipFixer;
    public Button FixButton;
}
