using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalAxisController : InputComponentController
{
    public string NegativeName;
    public string PositiveName;
    public bool Invert;
    public float Sensitivity = 1;

    public override void Process()
    {
        VirtualInput.Axis = GetAxis(NegativeName, PositiveName, Invert, Sensitivity);
    }
}
