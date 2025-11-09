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
    public struct FurnaceSlotUI
    {
        public RawImage ItemIcon;
        public TextMeshProUGUI QuantityText;
    }

    //Furnace Properties :
    public FurnaceSlotUI WoodFurnace;
    public FurnaceSlotUI OreFurnace;
}
