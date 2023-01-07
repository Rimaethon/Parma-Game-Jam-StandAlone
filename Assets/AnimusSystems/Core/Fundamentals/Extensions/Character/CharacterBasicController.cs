using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBasicController : MonoBehaviour
{
    [Header("Input")]
    public string MovementDirection = "Movement";
    public string JumpButton = "Jump";
    private AvatarOverallInput input;

    [Header("Parameters")]
    public float MoveSpeed = 2;
    public float JumpPower = 5;

    private bool isGrounded;
    public bool IsGrounded
    {
        get => isGrounded;
        private set
        {
            if (isGrounded && !value) OnAirborne.Invoke();
            else if (!isGrounded && value) OnLand.Invoke();
            isGrounded = value;
        }
    }

    public UnityEvent OnLand;
    public UnityEvent OnAirborne;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        input = GetComponent<AvatarOverallInput>();
        rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        var rotation = Quaternion.Euler(0, input.LookRotation.eulerAngles.y, 0);
        var velocity = new Vector3(input.inputs[MovementDirection].Direction.x, 0, input.inputs[MovementDirection].Direction.y);
        velocity = rotation*velocity*MoveSpeed;
        velocity.y = rigidbody.velocity.y;

        IsGrounded = Physics.Raycast(transform.position+transform.up*0.1f, -transform.up, 0.2f);
        if (IsGrounded && input.inputs[JumpButton].isPressed) velocity.y = JumpPower;
        
        rigidbody.velocity = velocity;
    }
    private void Update()
    {
        transform.localRotation = Quaternion.Euler(0, input.LookRotation.eulerAngles.y, 0);
    }
}
