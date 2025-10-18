using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _characterController;

    public Camera PlayerCamera;

    public float MovementSpeed;
    public float RunningSpeed;

    public float Sensitivity;

    public float JumpHeigh;

    public bool isWalking = false;
    public bool isMovingForward = false;
    public bool isMovingRight = false;
    public bool isMovingLeft = false;
    public bool isMovingBack = false;

    private Vector3 _moveDiraction;
    private Vector3 _velocity;

    private float _horizontalMove;
    private float _verticalMove;

    private float _rotationClamp = 35f;

    private float _gravity = -9.81f;

    private Vector2 _mouseDelta;
    private Vector2 _mouseRotation;

    public bool _isGrounded = false;
    private bool _isRunning = false;

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();

    //    _characterController = GetComponent<CharacterController>();

    //    CursorVisabilityController.Instance.SetCursorVisability(false);

    //    if (!IsOwner)
    //    {
    //        PlayerCamera.enabled = false;
    //    }
    //}

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

       CursorVisabilityController.Instance.SetCursorVisability(false);
    }

    private void Update()
    {
        //if (!IsOwner)
        //{
        //    return;
        //}

        GetMouseImput();

        CameraMovement();

        Movement();

        _velocity.y += _gravity * Time.deltaTime;

        _characterController.Move(_velocity * Time.deltaTime);

        if (_characterController.isGrounded)
        {
            _isGrounded = true;

            if (_velocity.y < 0)
            {
                _velocity.y = -2f;
            }

        }
        else
        {
            _isGrounded = false;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (_characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(JumpHeigh * -_gravity);
            }
        }

        if (Keyboard.current.leftShiftKey.isPressed && _characterController.isGrounded && !_isRunning)
        {
            MovementSpeed += RunningSpeed;
            _isRunning = true;
        }
        else if (_characterController.isGrounded && _isRunning)
        {
            MovementSpeed -= RunningSpeed;
            _isRunning = false;
        }
    }

    private void Movement()
    {
        _verticalMove = 0f;
        _horizontalMove = 0f;

        if (Keyboard.current.wKey.isPressed)
        {
            _verticalMove = 1f;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            _verticalMove = -1f;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            _horizontalMove = -1f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            _horizontalMove = 1f;
        }

        isWalking = (_verticalMove < 0 || _verticalMove > 0) || (_horizontalMove < 0 || _horizontalMove > 0) 
            && _characterController.isGrounded;

        isMovingForward = (_verticalMove > 0) && _characterController.isGrounded;
        isMovingBack = (_verticalMove < 0) && _characterController.isGrounded;

        isMovingRight = (_horizontalMove > 0) && _characterController.isGrounded;
        isMovingLeft = (_horizontalMove < 0) && _characterController.isGrounded;

        _moveDiraction = transform.right * _horizontalMove + transform.forward * _verticalMove;

        _characterController.Move(_moveDiraction.normalized * MovementSpeed * Time.deltaTime);
    }

    private void GetMouseImput()
    {
        _mouseDelta = Mouse.current.delta.ReadValue();

        _mouseRotation.x += _mouseDelta.x * Sensitivity;
        _mouseRotation.y -= _mouseDelta.y * Sensitivity;
    }

    private void CameraMovement()
    {
        _mouseRotation.y = Mathf.Clamp(_mouseRotation.y, -_rotationClamp, _rotationClamp);

        PlayerCamera.transform.localRotation = Quaternion.Euler(_mouseRotation.y, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, _mouseRotation.x, 0f);
    }
}