using UnityEngine;

public class PalmaController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _palmPosition;

    private Animator _animator;

    private Transform _originalPosition;

    public int health;

    public ItemData Item;

    public bool repeat = false;

    private void Start()
    {
        _originalPosition = _palmPosition;

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            _rigidbody.isKinematic = false;
            _animator.enabled = false;
        }

        if (repeat)
        {
            health = 100;
            _palmPosition = _originalPosition;
            _rigidbody.isKinematic = true;
            //repeat = false;
        }
    }

    public void FarmWood(int Quantity)
    {
        _animator.SetTrigger("Hitted");
        health -= Quantity;
        _inventoryController.AddItem(Item, Quantity);
    }
}
