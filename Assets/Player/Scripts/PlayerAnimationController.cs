using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private Ray ray;

    private float rayDistance = 4f;

    private Vector3 rayEnd;

    public Animator animator;
    public GameObject targetForAnimation;

    public GameObject RightHandRig;
    public GameObject LeftHandRig;

    public bool isWalking = false;

    public enum WeaponType
    {
        Melee = 0,
        Rifle = 1
    }

    private WeaponType currentWeaponType = WeaponType.Melee;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        SetWeaponType(0);
    }

    private void Update()
    {
        PlayWalkingAnimation();

        animator.SetBool("MeleeIdle", !isWalking);

        ChangePlayerPoseByWeapon();

        PlayJumpAnimation();
        PlayeMeleeAttack();
        AimPartOfBodyToTarget();
    }

    private void PlayeMeleeAttack()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && playerMovement._isGrounded && animator.GetInteger("WeaponType") == 0)
        {
            animator.SetTrigger("MeleeAttack");
        }
    }

    private void PlayWalkingAnimation()
    {
        isWalking = playerMovement.isMovingForward ||
                     playerMovement.isMovingBack ||
                     playerMovement.isMovingLeft ||
                     playerMovement.isMovingRight;

        animator.SetBool("isWalkingForward", playerMovement.isMovingForward);
        animator.SetBool("isWalkingBack", playerMovement.isMovingBack);
        animator.SetBool("isWalkingLeft", playerMovement.isMovingLeft);
        animator.SetBool("isWalkingRight", playerMovement.isMovingRight);

        animator.SetLayerWeight(animator.GetLayerIndex("LegsLayer"), 1f);
    }

    private void PlayJumpAnimation()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && playerMovement._isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }

    public void SetWeaponType(WeaponType type)
    {
        currentWeaponType = type;
        animator.SetInteger("WeaponType", (int)type);
    }

    private void ChangePlayerPoseByWeapon()
    {

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SetWeaponType(WeaponType.Melee);
            RightHandRig.GetComponent<MultiAimConstraint>().weight = 0f;
            LeftHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0f;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SetWeaponType(WeaponType.Rifle);
            RightHandRig.GetComponent<MultiAimConstraint>().weight = 1f;
            LeftHandRig.GetComponent<TwoBoneIKConstraint>().weight = 1f;
        }
    }

    private void AimPartOfBodyToTarget()
    {
        ray.origin = playerMovement.PlayerCamera.transform.position;
        ray.direction = playerMovement.PlayerCamera.transform.forward;

        rayEnd = ray.origin + ray.direction * rayDistance;

        targetForAnimation.transform.position = rayEnd;

        //Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);
    }
}
