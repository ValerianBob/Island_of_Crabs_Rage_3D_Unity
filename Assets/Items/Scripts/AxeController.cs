using UnityEngine;
using UnityEngine.InputSystem;

public class AxeController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    private Ray _ray;

    private RaycastHit _hit;

    public float RayDistance;

    public int QuntityToEarn;

    private void Update()
    {
        _ray.origin = PlayerCamera.transform.position;
        _ray.direction = Camera.main.transform.forward;

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, RayDistance))
        {
            Transform hitTransform = _hit.collider.transform.parent;

            if (Mouse.current.leftButton.wasPressedThisFrame && _hit.collider.CompareTag("Palma"))
            {
                PalmaController palmaController = hitTransform.GetComponent<PalmaController>();
                palmaController.FarmWood(QuntityToEarn);
            }
        }

        Debug.DrawRay(_ray.origin, _ray.direction * RayDistance, Color.red);
    }
}
