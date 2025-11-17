using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private TextMeshProUGUI InteractionText;

    [SerializeField] private HammerController _hammerController;

    private InventoryController _inventoryController;

    private Ray _ray;

    private RaycastHit _rayHit;

    [SerializeField] private float RayDistance;

    private float _alpha = 0f;
    private float _fadeTextDuration = 0.3f;
    public bool _isVisible = false;

    private void Start()
    {
        _inventoryController = GetComponent<InventoryController>();
    }

    private void Update()
    {
        _ray.origin = PlayerCamera.transform.position;
        _ray.direction = PlayerCamera.transform.forward;
        
        if (Physics.Raycast(_ray.origin, _ray.direction, out _rayHit, RayDistance))
        {
            if (_rayHit.collider.GetComponent<ItemController>() != null)
            {
                TakeItem();
            }
            else if (_rayHit.collider.GetComponent<FurnaceController>() != null && !_hammerController.isBuilding)
            {
                UseFurnace();
            }
            else if (_rayHit.collider.GetComponent<WaterCollectionController>() != null && !_hammerController.isBuilding)
            {
                UseWaterCollection();
            }
            else if (_rayHit.collider.GetComponent<CircularSawController>() != null && !_hammerController.isBuilding)
            {
                UseCircularSaw();
            }
            else
            {
                _isVisible = false;
            }
        }
        else
        {
            _isVisible = false;
        }

        ShowOrHideInteractionInfoText();

        //Debug.DrawRay(_ray.origin, _ray.direction * RayDistance, Color.black);
    }

    private void TakeItem()
    {
        _isVisible = true;

        ItemController currentItem = _rayHit.collider.GetComponent<ItemController>();

        InfoText.text = currentItem.ItemData.ItemName;
        InteractionText.text = currentItem.ItemData.InteractInfo;

        bool TryPickUp = false;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentItem.ItemData.Type == ItemType.Resource)
            {
                TryPickUp = _inventoryController.AddItem(currentItem.ItemData, currentItem.Quantity);
            }
            else if (currentItem.ItemData.Type == ItemType.Instrument || currentItem.ItemData.Type == ItemType.Gun)
            {
                TryPickUp = _inventoryController.AddItemInHotKeys(currentItem.ItemData, currentItem.Quantity);
            }
            else
            {
                Debug.Log("Can't take this item with strange type");
            }

            if (TryPickUp)
            {
                Destroy(currentItem.gameObject);
            }
        }
    }

    private void UseFurnace()
    {
        _isVisible = true;

        FurnaceController currentFurnace = _rayHit.collider.GetComponent<FurnaceController>();

        InfoText.text = currentFurnace.ObjectName;
        InteractionText.text = currentFurnace.Info;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            _inventoryController.ToggleFromFurnace(currentFurnace);
        }
    }

    private void UseWaterCollection()
    {
        _isVisible = true;

        WaterCollectionController currentWaterCollection = _rayHit.collider.GetComponent<WaterCollectionController>();

        InfoText.text = currentWaterCollection.ObjectName;
        InteractionText.text = currentWaterCollection.Info;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentWaterCollection.DrinkWater(GetComponent<PlayerConditionController>(), 10);
        }
    }

    private void UseCircularSaw()
    {
        _isVisible = true;

        CircularSawController currentCircularSaw = _rayHit.collider.GetComponent<CircularSawController>();

        InfoText.text = currentCircularSaw.ObjectName;
        InteractionText.text = currentCircularSaw.Info;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            _inventoryController.ToggleFromCirculatSaw(currentCircularSaw);
        }
    }

    private void ShowOrHideInteractionInfoText()
    {
        if (_isVisible && _alpha < 1f)
        {
            _alpha += Time.deltaTime / _fadeTextDuration;
        }
        else if (!_isVisible && _alpha > 0f)
        {
            _alpha -= Time.deltaTime / _fadeTextDuration;
        }

        Color ColorInfo = InfoText.color;
        Color ColorInteraction = InteractionText.color;

        _alpha = Mathf.Clamp01(_alpha);

        InfoText.color = new Color(InfoText.color.r, InfoText.color.g, InfoText.color.b, _alpha);
        InteractionText.color = new Color(InteractionText.color.r, InteractionText.color.g, InteractionText.color.b, _alpha);
    }
}
