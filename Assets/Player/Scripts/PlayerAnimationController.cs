using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (playerMovement._isGrounded)
        {
            animator.SetTrigger("MeleeIdle");
        }

        PlayJumpAnimation();
        PlayWalkingAnimation();
        PlayeMeleeAttack();
        AimPartOfBodyToTarget();
    }

    private void PlayeMeleeAttack()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && playerMovement._isGrounded)
        {
            animator.SetTrigger("MeleeAttack");
        }
    }

    private void PlayWalkingAnimation()
    {
        bool isWalking = playerMovement.isMovingForward ||
                     playerMovement.isMovingBack ||
                     playerMovement.isMovingLeft ||
                     playerMovement.isMovingRight;

        animator.SetBool("isWalking", playerMovement.isMovingForward);
        animator.SetBool("isWalkingBack", playerMovement.isMovingBack);
        animator.SetBool("isWalkingLeft", playerMovement.isMovingLeft);
        animator.SetBool("isWalkingRight", playerMovement.isMovingRight);

        animator.SetLayerWeight(animator.GetLayerIndex("Legs"), isWalking ? 1f : 0f);
    }

    private void PlayJumpAnimation()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && playerMovement._isGrounded)
        {
            animator.SetTrigger("Jump");
        }
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
