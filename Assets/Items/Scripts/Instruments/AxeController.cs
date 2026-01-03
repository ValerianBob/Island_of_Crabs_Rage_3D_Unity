using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private ParticleSystem WoodDebris;

    [SerializeField] private InventoryController _inventoryController;

    private Ray _ray;

    private RaycastHit _hit;

    public float RayDistance;

    public int QuntityToEarn;

    public float AttackDelay;

    private Coroutine _hitCoroutine;

    private bool _canAttack = true;

    private void Update()
    {
        _ray.origin = PlayerCamera.transform.position;
        _ray.direction = Camera.main.transform.forward;

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, RayDistance))
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && _canAttack && _hit.collider.CompareTag("Palma") && !_inventoryController.isOpened)
            {
                _hitCoroutine = StartCoroutine(Delay());
            }
        }
    }

    private void Hit()
    {
        if (_hit.collider == null) return;

        Transform parent = _hit.collider.transform;

        while (parent != null)
        {
            PalmaController palmController = parent.GetComponent<PalmaController>();
            if (palmController != null)
            {
                palmController.FarmWood(QuntityToEarn);

                ParticleSystem tempWoodDebris = Instantiate(WoodDebris, _hit.point, WoodDebris.transform.rotation);
                tempWoodDebris.Play();

                SoundsController.Instance.PlayInstruments(0, _hit.point);

                return;
            }

            parent = parent.parent;
        }

        if (parent == null)
        {
            return;
        }
        else
        {
        }
    }

    private IEnumerator Delay()
    {
        _canAttack = false;

        yield return new WaitForSeconds(AttackDelay);

        Hit();

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
