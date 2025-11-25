using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    
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
            if (Mouse.current.leftButton.wasPressedThisFrame && _canAttack && _hit.collider.CompareTag("Palma"))
            {
                _hitCoroutine = StartCoroutine(Delay());
            }
        }

        Debug.DrawRay(_ray.origin, _ray.direction * RayDistance, Color.red);
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
                Debug.Log($"Palm found: {parent.name}");
                palmController.FarmWood(QuntityToEarn);
                return;
            }

            parent = parent.parent;
        }

        if (parent == null)
        {
            Debug.LogWarning("Palm parent not found!");
            return;
        }
        else
        {
            Debug.Log($"This palm : {parent}");
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
