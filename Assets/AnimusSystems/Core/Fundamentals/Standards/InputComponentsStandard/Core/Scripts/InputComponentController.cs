using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System;

public class InputComponentController : MonoBehaviour
{
    public int DeviceIndex = 0;
    public AvatarOverallInput.VirtualInputComponent VirtualInput;
    public bool IsActive { get
        {
            return
              !(VirtualInput.Axis == 0 &&
                VirtualInput.Direction == Vector2.zero &&
                VirtualInput.isPressed == false);
        } }
   
    protected float GetAnalogButton(string name, bool invert=false)
    {
        if (string.IsNullOrEmpty(name)) return 0;
        switch (name)
        {
            case "None": return 0;
            case "MouseLeft": return Input.GetMouseButton(0)?1:0;
            case "MouseRight": return Input.GetMouseButton(1)?1:0;
            case "MouseMiddle": return Input.GetMouseButton(2)?1:0;
            case "Mouse ScrollWheel":
            case "Mouse X":
            case "Mouse Y":
                return invert ? -Input.GetAxis(name) : Input.GetAxis(name);

        }
        KeyCode button;
        if (Enum.TryParse<KeyCode>(name, out button))
        {
            return Input.GetKey(button) ? 1 : 0;
        }
        #if ENABLE_INPUT_SYSTEM
            if (Gamepad.all.Count > DeviceIndex)
            {
                float value = Convert.ToSingle(Gamepad.all[DeviceIndex].GetChildControl(name).ReadValueAsObject());
                return invert? -value:value;
            }
        #endif
        return 0;
    }
    protected bool GetButton(string name, bool invert=false, float threshold=0.5f)
    {
        if (string.IsNullOrEmpty(name)) return false;
        return invert ? GetAnalogButton(name, invert) < -threshold : GetAnalogButton(name, invert) > threshold;
    }
    protected float GetAxis(string negative, string positive, bool invert=false, float sensitivity = 1)
    {
        if (string.IsNullOrEmpty(negative) || string.IsNullOrEmpty(positive)) return 0;
        if (Input.touchCount > 0 && 
           (negative=="Mouse X" || 
            positive=="Mouse X" ||
            negative=="Mouse Y" ||
            positive=="Mouse Y"))
            return 0;
        if (negative == positive) return GetAnalogButton(positive, invert) * sensitivity;

        float neg = Mathf.Clamp01(GetAnalogButton(negative));
        float pos = Mathf.Clamp01(GetAnalogButton(positive));
        return (invert ? -1 : 1) * (pos - neg) * sensitivity;
    }
    public virtual void Process()
    {

    }

    public enum MouseButtonCode { None, Left, Right, Middle }
    
}
