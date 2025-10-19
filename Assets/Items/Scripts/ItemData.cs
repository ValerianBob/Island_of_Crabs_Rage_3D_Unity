using UnityEngine;

public enum ItemType
{
    Resource,
    Instrument,
    Gun
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public GameObject ObjectPrefab;
    public string ItemName;
    public string InteractInfo;
    public ItemType Type;
    public Sprite Icon;
    public bool isStackble;
    public int MaxQuantity;
}
