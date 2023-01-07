using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TouchscreenTrackpadController : InputComponentController
{
    public bool HorizontalInvert;
    public float HorizontalSensitivity = 1;
    public bool VerticalInvert;
    public float VerticalSensitivity = 1;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void Process()
    {
        Rect anchorRect = new Rect(rectTransform.anchorMin, rectTransform.anchorMax - rectTransform.anchorMin);
        var touch = Input.touches.FirstOrDefault(t => anchorRect.Contains(new Vector2(t.position.x / Screen.width, t.position.y / Screen.height)));
        VirtualInput.Direction = new Vector2(
            (HorizontalInvert ? -1 : 1) * touch.deltaPosition.x * HorizontalSensitivity,
            (VerticalInvert ? -1 : 1) * touch.deltaPosition.y * VerticalSensitivity);
    }
}
