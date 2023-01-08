using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playerRb;
    PlayerInput playerInput;
    public Animator PlayerAnim;
    bool isGrounded, canJump, isAttack;
    Vector3 movementInput;
    float jumpForce = 400f, moveForce = 1000f, maxSpeed = 5f;


    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        canJump = true;
        isGrounded = true;
        isAttack = true;
    }

    void FixedUpdate()
    {

        CheckGrounded();
        HandleMovement();



        AttackAnimController();
    }
    private void Update()
    {
        MovementAnimController();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //Standard jump. Coroutine sets canJump to TRUE after 0.1 seconds. 
        //CheckGrounded() uses raycast to check if the character is grounded.
        //We can use a simple state machine as well but I think it isn't necessary for now
        Debug.Log("I can Jummppppppppppppp");
        if (context.performed && isGrounded && canJump)
        {
            PlayerAnim.SetTrigger("jump");
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

        playerRb.AddRelativeForce(movementInput * moveForce, ForceMode.Force);
        playerRb.velocity = Vector3.ClampMagnitude(playerRb.velocity, maxSpeed);
    }

    void CheckGrounded()
    {
        //Using raycast to check if the character is grounded. We can add layer mask if necessary.
        RaycastHit hit;
        Ray landingRay = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(landingRay, out hit, .1f) && canJump)
        {
            Debug.Log("Raycast working");
            isGrounded = true;
        }
    }

    IEnumerator CheckCanJump()
    {
        //Jump input has 0.1 seconds cooldown to prevent input duplication.
        yield return new WaitForSeconds(0.1f);
        canJump = true;
    }


    void AttackAnimController()
    {
        if (isAttack)
        {

            if (Input.GetMouseButtonDown(0))
            {
                isAttack = false;
                PlayerAnim.SetTrigger("solkesme");
                StartCoroutine(AttackCoolDown());
            }
            else if (Input.GetMouseButtonDown(1))
            {
                isAttack = false;
                PlayerAnim.SetTrigger("saðkesme");
                StartCoroutine(AttackCoolDown());
            }
            else if (Input.GetMouseButtonDown(2))
            {
                isAttack = false;
                PlayerAnim.SetTrigger("dikkesme");
                StartCoroutine(AttackCoolDown());
            }

        }


    }
    void MovementAnimController()
    {
        if (Input.GetKeyDown("w"))
        {
            PlayerAnim.SetBool("w", true);
        }
        if (Input.GetKeyUp("w"))
        {
            PlayerAnim.SetBool("w", false);
        }

        if (Input.GetKeyDown("a"))
        {
            PlayerAnim.SetBool("a", true);
        }
        if (Input.GetKeyUp("a"))
        {
            PlayerAnim.SetBool("a", false);
        }

        if (Input.GetKeyDown("s"))
        {
            PlayerAnim.SetBool("s", true);
        }
        if (Input.GetKeyUp("s"))
        {
            PlayerAnim.SetBool("s", false);
        }

        if (Input.GetKeyDown("d"))
        {
            PlayerAnim.SetBool("d", true);
        }
        if (Input.GetKeyUp("d"))
        {
            PlayerAnim.SetBool("d", false);
        }


    }

    IEnumerator AttackCoolDown()
    {

        yield return new WaitForSeconds(1f);
        isAttack = true;

    }


}
