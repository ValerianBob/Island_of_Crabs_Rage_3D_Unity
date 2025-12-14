using UnityEngine;
using UnityEngine.InputSystem;

public class MusketController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private ParticleSystem Blood;

    private Ray _ray;

    private RaycastHit _hit;

    private float rayDistance = 50f;

    private float _NextTime = 0;

    public float _fireRate;

    public int Damage;

    private void Update()
    {
        _ray.origin = PlayerCamera.transform.position;
        _ray.direction = PlayerCamera.transform.forward;

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, rayDistance))
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && Time.time > _NextTime)
            {
                _NextTime = Time.time + _fireRate;

                EnemyController tempEnemy = _hit.collider.gameObject.GetComponent<EnemyController>();
                
                if (tempEnemy != null)
                {
                    tempEnemy.TakeDamage(Damage);

                    ParticleSystem tempBlood = Instantiate(Blood, _hit.point, Blood.transform.rotation);
                    tempBlood.Play();
                }

                SoundsController.Instance.PlayGun(0, transform.position);
                Invoke("PlayeReloadSound", 0.7f);

                Debug.DrawRay(_ray.origin, _ray.direction * rayDistance);
            }
        }
    }

    private void PlayeReloadSound()
    {
        SoundsController.Instance.PlayGun(1, transform.position);
    }
}
