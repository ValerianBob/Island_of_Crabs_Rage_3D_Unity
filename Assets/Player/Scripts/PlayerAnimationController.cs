using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private RigBuilder PlayerRigBuilder;
    private CharacterController _characterController;

    private PlayerMovement playerMovement;

    private Ray ray;

    private float rayDistance = 4f;

    private Vector3 rayEnd;

    public Animator animator;
    public GameObject targetForAnimation;

    public GameObject RightHandRig;
    public GameObject LeftHandRig;

    public bool isWalking = false;

    private bool _isDead = false;

    public enum WeaponType
    {
        Melee = 0,
        Rifle = 1
    }

    private WeaponType currentWeaponType = WeaponType.Melee;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        _inventoryController = GetComponent<InventoryController>();
        _characterController = GetComponent<CharacterController>();

        SetWeaponType(0);
    }

    private void Update()
    {
        PlayWalkingAnimation();

        animator.SetBool("MeleeIdle", !isWalking);

        ChangePlayerPoseByWeapon();

        if (!_isDead)
        {
            PlayJumpAnimation();
            PlayeMeleeAttack();
            AimPartOfBodyToTarget();
        }
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
        if (_inventoryController.isMeleeItemInHand)
        {
            SetWeaponType(WeaponType.Melee);
            RightHandRig.GetComponent<MultiAimConstraint>().weight = 0f;
            LeftHandRig.GetComponent<TwoBoneIKConstraint>().weight = 0f;
        }
        if (!_inventoryController.isMeleeItemInHand)
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
    }

    private void OnEnable()
    {
        PlayerConditionController.Dead += PlayDeadAnimation;
        PlayerConditionController.Respawn += SetAlive;        
    }

    private void OnDisable()
    {
        PlayerConditionController.Dead -= PlayDeadAnimation;
        PlayerConditionController.Respawn -= SetAlive;
    }

    private void PlayDeadAnimation()
    {
        _isDead = true;

        PlayerRigBuilder.enabled = false;

        animator.SetLayerWeight(1, 0);

        Invoke("ChangeCharacterControllerHeight", 3);

        animator.SetBool("isDead", _isDead);
    }

    private void SetAlive()
    {
        _isDead = false;

        PlayerRigBuilder.enabled = true;
        animator.SetLayerWeight(1, 1);

        _characterController.height = 1.8f;

        transform.Translate(Vector3.up * 40f * Time.deltaTime);

        animator.SetBool("isDead", _isDead);        
    }

    private void ChangeCharacterControllerHeight()
    {
        _characterController.height = 0;
    }
}
