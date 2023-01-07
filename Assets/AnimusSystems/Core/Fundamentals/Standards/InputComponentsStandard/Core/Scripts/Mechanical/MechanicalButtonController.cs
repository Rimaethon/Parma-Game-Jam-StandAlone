using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalButtonController : InputComponentController
{
    public string Name;
    public float Threshold = 0.5f;
    public bool Invert;

    public override void Process()
    {
        VirtualInput.isPressed = GetButton(Name, Invert, Threshold);
    }
}
