using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;

    public float lookSpeed = 60f;
    public float jumpPower = 8f;
    public float gravity = 9.81f;

    [Header("Required references")]
    public Shooter playerShooter;

    public Health playerHealth;
    public List<GameObject> disableWhileDead;

    private bool doubleJumpAvailable;
    private CharacterController controller;
    private InputManager inputManager;
    private Vector3 moveDirection;

    private void Start()
    {
        SetUpCharacterController();
        SetUpInputManager();
    }

    private void SetUpCharacterController()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError("No CharacterController");
    }

    private void SetUpInputManager()
    {
        inputManager = InputManager.instance;
    }

    private void Update()
    {
        if (playerHealth.currentHealth <= 0)
        {
            foreach (var inGameObject in disableWhileDead)
                inGameObject.SetActive(false);
            return;
        }

        foreach (var inGameObject in disableWhileDead)
            inGameObject.SetActive(true);

        ProcessMovement();
        ProcessRotation();
    }

    private void ProcessMovement()
    {
        var leftRightInput = inputManager.horizontalMoveAxis;
        var forwardBackwardInput = inputManager.verticalMoveAxis;
        var jumpPressed = inputManager.jumpPressed;

        if (controller.isGrounded)
        {
            doubleJumpAvailable = true;

            moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput);
            moveDirection = transform.TransformDirection(moveDirection) * moveSpeed;

            if (jumpPressed)
                moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y, forwardBackwardInput * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);

            if (jumpPressed && doubleJumpAvailable)
            {
                moveDirection.y = jumpPower;
                doubleJumpAvailable = false;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }

    private void ProcessRotation()
    {
        var horizontalLookInput = inputManager.horizontalLookAxis;
        var playerRotation = transform.rotation.eulerAngles;
        var rotation = new Vector3(playerRotation.x,
            playerRotation.y + horizontalLookInput * lookSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(rotation);
    }
}