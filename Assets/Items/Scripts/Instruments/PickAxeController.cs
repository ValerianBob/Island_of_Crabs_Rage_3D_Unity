using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickAxeController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private LayerMask LayerMask;

    [SerializeField] private ParticleSystem StoneDebris;

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

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, RayDistance, LayerMask))
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && _canAttack && _hit.collider.CompareTag("Ore"))
            {
                Debug.Log($"Pick Axe hitted : {_hit.collider.gameObject.name}");

                _hitCoroutine = StartCoroutine(Delay(_hit));
            }
        }

        Debug.DrawRay(_ray.origin, _ray.direction * RayDistance, Color.red);
    }

    private void Hit(RaycastHit hit)
    {
        if (hit.collider == null) return;

        Transform parent = hit.collider.transform;

        while (parent != null)
        {
            OreController oreController = parent.GetComponent<OreController>();
            if (oreController != null)
            {
                Debug.Log($"Ore found: {parent.name}");
                oreController.FarmOre(QuntityToEarn);

                ParticleSystem tempStoneDebris = Instantiate(StoneDebris, _hit.point, StoneDebris.transform.rotation);
                tempStoneDebris.Play();

                SoundsController.Instance.PlayInstruments(1, _hit.point);

                return;
            }

            parent = parent.parent;
        }

        if (parent == null)
        {
            Debug.LogWarning("Ore parent not found!");
            return;
        }
        else
        {
            Debug.Log($"This ore : {parent}");
        }
    }

    private IEnumerator Delay(RaycastHit hit)
    {
        _canAttack = false;

        yield return new WaitForSeconds(AttackDelay);

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
