using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InputButtonAnimator : MonoBehaviour
{
    public InputHubController hub;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        animator.SetBool("IsPressed", hub.Input.isPressed);
    }
}
