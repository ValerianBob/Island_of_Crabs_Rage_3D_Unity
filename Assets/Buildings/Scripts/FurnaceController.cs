using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FurnaceController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;

    [SerializeField] private GameObject IronPrefab;

    [SerializeField] private GameObject FireLight;

    [SerializeField] private GameObject DropPoint;

    [System.Serializable]
    public struct FurnaceSlot
    {
        public ItemData Item;
        public int Quantity;
    }

    public FurnaceSlot WoodSlot;
    public FurnaceSlot OreSlot;

    public string ObjectName;
    public string Info;

    private float _meltSpeed = 1.0f;
    private float _nextTime = 0f;
    private float _currentTime = 5f;

    private bool _isLightWork = false;
    public bool isFurnaceSelected = false;

    private void Start()
    {
        _inventoryController = GameObject.Find("Player").GetComponent<InventoryController>();
    }

    private void Update()
    {
        if (isFurnaceSelected)
        {
            UIManager.Instance.MeltTimeText.text = _currentTime.ToString();
        }

        if (FireLight.activeSelf != _isLightWork)
        {
            FireLight.SetActive(_isLightWork);
        }

        DropIron();

        CountTime();
    }

    private void CountTime()
    {
        if (WoodSlot.Quantity > 0 && OreSlot.Quantity > 0)
        {
            if (Time.time >= _nextTime)
            {
                _nextTime = Time.time + _meltSpeed;

                _currentTime -= 1f;
                Debug.Log(_currentTime);
            }
            _isLightWork = true;
        }
        else
        {
            _isLightWork = false;
            FireLight.SetActive(_isLightWork);

            _currentTime = 5f;
        }
    }

    private void DropIron()
    {
        if (_currentTime <= 0)
        {
            WoodSlot.Quantity -= 1;
            OreSlot.Quantity -= 1;

            GameObject IronToDrop = Instantiate(IronPrefab, DropPoint.transform.position, Quaternion.identity);

            IronToDrop.GetComponent<ItemController>().Quantity = 1;

            _currentTime = 5;
        }
    }
}
