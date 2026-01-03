using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusketController : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private ParticleSystem Blood;

    [SerializeField] private ParticleSystem MuzzleFlash;

    [SerializeField] private Transform MuzzlePoint;

    [SerializeField] private Light MuzzleLight;

    [SerializeField] private InventoryController _inventoryController;

    [SerializeField] private LayerMask LayerMaskWater;

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

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, rayDistance, LayerMaskWater))
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && Time.time > _NextTime && !_inventoryController.isOpened)
            {
                _NextTime = Time.time + _fireRate;

                EnemyController tempEnemy = _hit.collider.gameObject.GetComponent<EnemyController>();

                bool hasBullets = _inventoryController.TakeBullet();

                if (tempEnemy != null)
                {
                    if (hasBullets)
                    {
                        tempEnemy.TakeDamage(Damage);

                        ParticleSystem tempBlood = Instantiate(Blood, _hit.point, Blood.transform.rotation);
                        tempBlood.Play();
                        ParticleSystem tempMuzzle = Instantiate(MuzzleFlash, MuzzlePoint.position, MuzzleFlash.transform.rotation);
                        tempMuzzle.Play();
                        StartCoroutine("ShowAndHidMuzzleLight");

                        SoundsController.Instance.PlayGun(0, transform.position);
                    }
                    else
                    {
                        SoundsController.Instance.PlayGun(1, transform.position);
                        Notifications.Instance.CreateNotification("No Bullets", Color.red);
                    }
                }
                else
                {
                    if (hasBullets)
                    {
                        ParticleSystem tempMuzzle = Instantiate(MuzzleFlash, MuzzlePoint.position, MuzzleFlash.transform.rotation);
                        tempMuzzle.Play();
                        StartCoroutine("ShowAndHidMuzzleLight");

                        SoundsController.Instance.PlayGun(0, transform.position);
                    }
                    else
                    {
                        SoundsController.Instance.PlayGun(1, transform.position);
                        Notifications.Instance.CreateNotification("No Bullets", Color.red);
                    }
                }
                
            }
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && Time.time > _NextTime)
            {
                _NextTime = Time.time + _fireRate;

                bool hasBullets = _inventoryController.TakeBullet();

                if (hasBullets)
                {
                    ParticleSystem tempMuzzle = Instantiate(MuzzleFlash, MuzzlePoint.position, MuzzleFlash.transform.rotation);
                    tempMuzzle.Play();
                    StartCoroutine("ShowAndHidMuzzleLight");

                    SoundsController.Instance.PlayGun(0, transform.position);
                }
                else
                {
                    SoundsController.Instance.PlayGun(1, transform.position);
                    Notifications.Instance.CreateNotification("No Bullets", Color.red);
                }
            }
        }
    }

    private IEnumerator ShowAndHidMuzzleLight()
    {
        MuzzleLight.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        MuzzleLight.gameObject.SetActive(false);
    }
}
