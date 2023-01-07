using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ObjectMovementAnimator : MonoBehaviour
{
    public Transform RootTransform;

    [Header("Parameter names")]
    public string VelocityXName = "DX";
    public string VelocityYName = "DY";
    public string VelocityZName = "DZ";
    public string DampTimeName = "MovementDamp";

    private Animator animator;
    private Vector3 lastPosition;
    private Transform lastParent;
    private Vector3 velocity;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = RootTransform.localPosition;
    }
    private void Update()
    {
        var delta = RootTransform.InverseTransformDirection(velocity);
        var damp = animator.GetFloat(DampTimeName);
        animator.SetFloat(VelocityXName, delta.x, damp, Time.deltaTime);
        animator.SetFloat(VelocityYName, delta.y, damp, Time.deltaTime);
        animator.SetFloat(VelocityZName, delta.z, damp, Time.deltaTime);
    }
    void FixedUpdate()
    {
        velocity = (RootTransform.position - (lastParent?lastParent.TransformPoint(lastPosition):lastPosition)) / Time.fixedDeltaTime;
        lastParent = RootTransform.parent;
        lastPosition = RootTransform.localPosition;
    }
}
