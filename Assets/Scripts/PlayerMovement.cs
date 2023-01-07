using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playerRb;
    PlayerInput playerInput;

    bool isGrounded, canJump;
    Vector3 movementInput;
    float jumpForce = 400f, moveForce = 1000f, maxSpeed = 10f;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        
        canJump = true;
    }

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //Standard jump. Coroutine sets canJump to TRUE after 0.1 seconds. 
        //CheckGrounded() uses raycast to check if the character is grounded.
        //We can use a simple state machine as well but I think it isn't necessary for now
        if (context.performed && isGrounded && canJump)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            canJump = false;
            StartCoroutine(CheckCanJump());
        }
    }

    public void Movement(InputAction.CallbackContext context)
    {
        Vector2 inputVector2 = context.ReadValue<Vector2>();
        movementInput = new Vector3(inputVector2.x, 0, inputVector2.y);
    }

    public void HandleMovement()
    {
        //Takes the input vector from Movement() to get movement direction. Also using Vector3.ClampMagnitude to set a max speed. 
        playerRb.AddForce(movementInput * moveForce, ForceMode.Force);
        playerRb.velocity = Vector3.ClampMagnitude(playerRb.velocity, maxSpeed);
    }

    void CheckGrounded()
    {
        //Using raycast to check if the character is grounded. We can add layer mask if necessary.
        RaycastHit hit;
        Ray landingRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(landingRay, out hit, .1f) && canJump)
        {
            isGrounded = true;
        }
    }

    IEnumerator CheckCanJump()
    {
        //Jump input has 0.1 seconds cooldown to prevent input duplication.
        yield return new WaitForSeconds(0.1f);
        canJump = true;
    }
}
