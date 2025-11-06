using System.Collections;
using UnityEngine;

public class PalmaController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _palmPosition;

    private Animator _animator;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    public int health;

    public ItemData Item;

    public float _recreateDelay;

    private bool isDead = false;

    private void Start()
    {
        _originalPosition = _palmPosition.transform.position;
        _originalRotation = _palmPosition.transform.rotation;

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (health <= 0 && !isDead)
        {
            isDead = true;

            _rigidbody.isKinematic = false;
            _animator.enabled = false;

            StartCoroutine(RecreateDelay());
        }
    }

    public void FarmWood(int Quantity)
    {
        if (health > 0)
        {
            _animator.SetTrigger("Hitted");
            health -= Quantity;
            _inventoryController.AddItem(Item, Quantity);
        }  
    }

    private void RecreatePalm()
    {
        isDead = false;

        _palmPosition.gameObject.SetActive(true);

        health = 100;

        _palmPosition.transform.position = _originalPosition;
        _palmPosition.transform.rotation = _originalRotation;

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;

        _animator.enabled = true;
    }

    private IEnumerator RecreateDelay()
    {
        yield return new WaitForSeconds(6f);

        _palmPosition.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(_recreateDelay);

        RecreatePalm();
    }
}
