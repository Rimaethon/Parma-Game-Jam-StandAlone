using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TouchscreenSteerController : InputComponentController
{
    public bool Invert;
    public float Sensitivity = 0.01f;
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

        if (touch.phase == TouchPhase.Began) StartPosition = touch.position;
        var direction = touch.position - StartPosition;

        VirtualInput.Axis = Mathf.Clamp((Invert ? -1 : 1) * direction.x * Sensitivity, -1, 1);
    }
}
