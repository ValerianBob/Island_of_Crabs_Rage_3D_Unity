using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private TextMeshProUGUI InteractionText;

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
                InteractWithItem();
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

        Debug.DrawRay(_ray.origin, _ray.direction * RayDistance, Color.black);
    }

    private void InteractWithItem()
    {
        _isVisible = true;

        InfoText.text = _rayHit.collider.GetComponent<ItemController>().ItemInfo;
        InteractionText.text = _rayHit.collider.GetComponent<ItemController>().InteractionInfo;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameObject currentItem = _rayHit.collider.gameObject;
            GameObject instrumentObject = _rayHit.collider.GetComponent<ItemController>().InstrumentOrGunOnPlayerObject;

            Sprite currentItemSprite = _rayHit.collider.gameObject.GetComponent<ItemController>().ItemIcon;
            ItemPrefab currentItemPrefab = _rayHit.collider.gameObject.GetComponent<ItemController>().ItemPrefab;
            int currentItemQuantity = _rayHit.collider.gameObject.GetComponent<ItemController>().Quantity;

            string currentItemName = _rayHit.collider.gameObject.GetComponent<ItemController>().ItemInfo;

            _inventoryController.AddItemInInventory(currentItem, currentItemSprite, currentItemPrefab, currentItemQuantity, instrumentObject, currentItemName);
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
