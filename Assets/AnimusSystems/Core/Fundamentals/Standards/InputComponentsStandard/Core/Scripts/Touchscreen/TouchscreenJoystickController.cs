using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TouchscreenJoystickController : InputComponentController
{
    public bool HorizontalInvert;
    public float HorizontalSensitivity = 0.01f;
    public bool VerticalInvert;
    public float VerticalSensitivity = 0.01f;

    private RectTransform rectTransform;
    public Vector2 StartPosition { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void Process()
    {
        Rect anchorRect = new Rect(rectTransform.anchorMin, rectTransform.anchorMax - rectTransform.anchorMin);
        var touch = Input.touches.FirstOrDefault(t => anchorRect.Contains(new Vector2(t.position.x / Screen.width, t.position.y / Screen.height)));
        if (touch.Equals(default(Touch)))
        {
            VirtualInput.Direction = Vector2.zero;
            return;
        }
        if (touch.phase == TouchPhase.Began) StartPosition = touch.position;
        var direction = touch.position - StartPosition;

        VirtualInput.Direction = Vector2.ClampMagnitude(new Vector2(
            (HorizontalInvert ? -1 : 1) * direction.x * HorizontalSensitivity,
            (VerticalInvert ? -1 : 1) * direction.y * VerticalSensitivity), 1);

    }
}
