using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalDirection2DController : InputComponentController
{
    public string HorizontalNegativeName;
    public string HorizontalPositiveName;
    public bool HorizontalInvert;
    public float HorizontalSensitivity = 1;

    public string VerticalNegativeName;
    public string VerticalPositiveName;
    public bool VerticalInvert;
    public float VerticalSensitivity = 1;

    public override void Process()
    {
        VirtualInput.Direction = new Vector2(
            GetAxis(HorizontalNegativeName, HorizontalPositiveName, HorizontalInvert, HorizontalSensitivity),
            GetAxis(VerticalNegativeName, VerticalPositiveName, VerticalInvert, VerticalSensitivity));
    }
}
