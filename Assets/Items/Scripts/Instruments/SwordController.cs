using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private ParticleSystem Blood;

    [SerializeField] private LayerMask LayerMaskWater;

    [SerializeField] private InventoryController _inventoryController;

    public int damage;

    private Ray _ray;

    private RaycastHit _hit;

    private Coroutine _hitCoroutine;

    private float _rayDistance = 6f;

    private float _attackRait = 0.6f;

    private bool _canAttack = true;

    private void Update()
    {
        _ray.origin = PlayerCamera.transform.position;
        _ray.direction = PlayerCamera.transform.forward;

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, _rayDistance, LayerMaskWater))
        {
            var enemy = _hit.collider.GetComponentInParent<EnemyController>();

            if (enemy != null && Mouse.current.leftButton.wasPressedThisFrame && 
                _canAttack && _hit.collider.gameObject.CompareTag("Enemy") && !_inventoryController.isOpened)
            {
                _hitCoroutine = StartCoroutine(Delay(_hit));
            }
        }
    }

    private void Hit(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        EnemyController enemyHealth = hit.collider.gameObject.GetComponent<EnemyController>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);

            ParticleSystem tempBlood = Instantiate(Blood, _hit.point, Blood.transform.rotation);
            tempBlood.Play();

            SoundsController.Instance.PlayInstruments(2, hit.point);
        }
    }

    private IEnumerator Delay(RaycastHit hit)
    {
        _canAttack = false;

        yield return new WaitForSeconds(_attackRait);

        Hit(hit);

        _canAttack = true;
    }

    private void OnDisable()
    {
        if (_hitCoroutine != null)
        {
            StopCoroutine(_hitCoroutine);
            _canAttack = true;
        }
    }
}
