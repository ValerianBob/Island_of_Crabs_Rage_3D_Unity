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
        PlayWalkingAnimation();

        AimPartOfBodyToTarget();
    }

    private void PlayWalkingAnimation()
    {
        if (playerMovement.isMovingForward)
        {
            animator.SetBool("isWalking", playerMovement.isMovingForward);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        else if (playerMovement.isMovingBack)
        {
            animator.SetBool("isWalkingBack", playerMovement.isMovingBack);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        else if (playerMovement.isMovingLeft)
        {
            animator.SetBool("isWalkingLeft", playerMovement.isMovingLeft);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        else if (playerMovement.isMovingRight)
        {
            animator.SetBool("isWalkingRight", playerMovement.isMovingRight);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        //Forward or Back and Left
        else if (playerMovement.isMovingForward && playerMovement.isMovingLeft)
        {
            animator.SetBool("isWalkingLeft", playerMovement.isMovingLeft);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        else if (playerMovement.isMovingBack && playerMovement.isMovingLeft)
        {
            animator.SetBool("isWalkingLeft", playerMovement.isMovingRight);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        //Forward or Back and Right
        else if (playerMovement.isMovingForward && playerMovement.isMovingRight)
        {
            animator.SetBool("isWalkingRight", playerMovement.isMovingRight);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        else if (playerMovement.isMovingBack && playerMovement.isMovingRight)
        {
            animator.SetBool("isWalkingRight", playerMovement.isMovingRight);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 1f);
        }
        else
        {
            animator.SetBool("isWalking", playerMovement.isMovingForward);
            animator.SetBool("isWalkingBack", playerMovement.isMovingBack);
            animator.SetBool("isWalkingLeft", playerMovement.isMovingLeft);
            animator.SetBool("isWalkingRight", playerMovement.isMovingRight);

            animator.SetLayerWeight(animator.GetLayerIndex("Legs"), 0);
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
