using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playerRb;
    PlayerInput PlayerInput;

    bool isGrounded, canJump;

    float jumpForce = 400f, moveForce = 1000f;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        canJump = true;
        PlayerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        PlayerInput.onActionTrigger += PlayerInputActionTriggered;
    }

    public void PlayerInputActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == "Movement")
        {
            Movement(context);
        }
    }

    void Update()
    {
        CheckGrounded();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(isGrounded + "," + canJump);

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
        playerRb.AddForce(context.ReadValue<Vector2>() * moveForce);
        Debug.Log(context.ReadValue<Vector2>());
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        Ray landingRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(landingRay, out hit, .1f) && canJump)
        {
            isGrounded = true;
        }
    }

    IEnumerator CheckCanJump()
    {
        yield return new WaitForSeconds(0.1f);
        canJump = true;
    }
}
