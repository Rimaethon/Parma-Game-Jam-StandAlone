using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputHubController))]
public class InputPostprocessController : MonoBehaviour
{
    public string AvatarInputName;
    public bool NormalizeMagnitude = true;
    private PlayerCameraController playerCamera;
    protected AvatarOverallInput input { get { return playerCamera.ControllableAvatar; } }
    protected InputHubController hub;

    protected virtual void Awake()
    {
        hub = GetComponent<InputHubController>();
        playerCamera = GetComponentInParent<PlayerCameraController>();
        Input.simulateMouseWithTouches = false;
    }

    protected virtual void FixedUpdate()
    {
        hub.Process();
        switch (input.inputs[AvatarInputName].Type)
        {
            case AvatarOverallInput.VirtualInputComponent.InputType.Axis:
                input.inputs[AvatarInputName].Axis = NormalizeMagnitude ? Mathf.Clamp(hub.Input.Axis,-1,1) : hub.Input.Axis;
                break;
            case AvatarOverallInput.VirtualInputComponent.InputType.Button:
                input.inputs[AvatarInputName].isPressed = hub.Input.isPressed;
                break;
            case AvatarOverallInput.VirtualInputComponent.InputType.Direction:
                input.inputs[AvatarInputName].Direction = NormalizeMagnitude ? Vector2.ClampMagnitude(hub.Input.Direction,1): hub.Input.Direction;
                break;
        }
    }
}
