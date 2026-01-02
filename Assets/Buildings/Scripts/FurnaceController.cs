using UnityEngine;

public class FurnaceController : MonoBehaviour
{
    [SerializeField] private GameObject IronPrefab;

    [SerializeField] private GameObject FireLight;

    [SerializeField] private GameObject DropPoint;

    [SerializeField] private Texture EmptyIcon;

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
    private float _currentTime = 10f;

    private bool _isLightWork = false;
    public bool isFurnaceSelected = false;

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
            }
            _isLightWork = true;
        }
        else
        {
            _isLightWork = false;
            FireLight.SetActive(_isLightWork);

            _currentTime = 10f;
        }
    }

    private void DropIron()
    {
        if (_currentTime <= 0)
        {
            int IronToSpawn = 0;

            if (WoodSlot.Quantity >= 10 && OreSlot.Quantity >= 10)
            {
                WoodSlot.Quantity -= 10;
                OreSlot.Quantity -= 10;

                IronToSpawn = 10;
            }
            else if (WoodSlot.Quantity >= 10 && OreSlot.Quantity < 10)
            {
                IronToSpawn = OreSlot.Quantity;

                WoodSlot.Quantity -= 10;
                OreSlot.Quantity = 0;
            }
            else if (WoodSlot.Quantity < 10 && OreSlot.Quantity >= 10)
            {
                IronToSpawn = WoodSlot.Quantity;
                
                WoodSlot.Quantity = 0;
                OreSlot.Quantity -= 10;
            }
            else
            {
                IronToSpawn = 5;

                WoodSlot.Quantity = 0;
                OreSlot.Quantity = 0;
            }

            if (isFurnaceSelected)
            {
                UIManager.Instance.WoodFurnace.QuantityText.text = WoodSlot.Quantity.ToString();
                UIManager.Instance.OreFurnace.QuantityText.text = OreSlot.Quantity.ToString();
            }

            if (WoodSlot.Quantity == 0)
            {
                if(isFurnaceSelected)
                {
                    UIManager.Instance.WoodFurnace.ItemIcon.texture = EmptyIcon;
                }

                WoodSlot.Item = null;
            }
            if (OreSlot.Quantity == 0)
            {
                if (isFurnaceSelected)
                {
                    UIManager.Instance.OreFurnace.ItemIcon.texture = EmptyIcon;
                }

                OreSlot.Item = null;
            }

            GameObject IronToDrop = Instantiate(IronPrefab, DropPoint.transform.position, Quaternion.identity);

            IronToDrop.GetComponent<ItemController>().Quantity = IronToSpawn;

            IronToSpawn = 0;

            _currentTime = 10;

            SoundsController.Instance.PlayBuilds(2, DropPoint.transform.position);
        }
    }
}
