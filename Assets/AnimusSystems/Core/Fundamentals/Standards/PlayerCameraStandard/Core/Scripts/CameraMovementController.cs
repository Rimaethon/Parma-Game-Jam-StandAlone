using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    [Header("Input")]
    public string HorizontalDirection = "CameraHorizontal";
    public string RiseButton = "CameraRise";
    public string DescendButton = "CameraDescend";
    private AvatarOverallInput input;

    [Header("Parameters")]
    public MotionType Motion;
    public float Speed = 0.1f;

    void Start()
    {
        input = GetComponentInParent<AvatarOverallInput>();
    }
    public void ToggleMotionType()
    {
        Motion = Motion == MotionType.Local ? MotionType.Global : MotionType.Local;
    }

    void LateUpdate()
    {
        var direction = input.inputs[HorizontalDirection].Direction;
        var rise = input.inputs[RiseButton].isPressed;
        var descend = input.inputs[DescendButton].isPressed;

        var rotation = Motion == MotionType.Global ? Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward,Vector3.up),Vector3.up) : transform.rotation;
        var movement = Vector3.ClampMagnitude(new Vector3(direction.x, rise ? 1 : descend ? -1 : 0, direction.y),1);

        transform.position += rotation*movement*Speed;        
    }
    public enum MotionType { Local, Global }
}
