using Unity.Netcode;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private Ray ray;

    private float rayDistance = 4f;

    private Vector3 rayEnd;

    public Animator animator;
    public GameObject targetForAnimation;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.isMoving)
        {
            animator.SetBool("isWalking", playerMovement.isMoving);
            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        else
        {
            animator.SetBool("isWalking", playerMovement.isMoving);
            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 0);
        }

        AimPartOfBodyToTarget();
    }

    private void AimPartOfBodyToTarget()
    {
        ray.origin = playerMovement.PlayerCamera.transform.position;
        ray.direction = playerMovement.PlayerCamera.transform.forward;

        rayEnd = ray.origin + ray.direction * rayDistance;

        targetForAnimation.transform.position = rayEnd;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);
    }
}
