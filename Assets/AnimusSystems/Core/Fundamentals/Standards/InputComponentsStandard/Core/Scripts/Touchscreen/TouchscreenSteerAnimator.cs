using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchscreenSteerAnimator : MonoBehaviour
{
    public Graphic Steer;
    public float SteerAngle = 45;
    public TouchscreenSteerController Controller;

    private Color steerColor;
    private Canvas canvas;
    private RectTransform steerRect;

    void Start()
    {
        steerColor = Steer.color;
        canvas = GetComponentInParent<Canvas>();
        steerRect = Steer.transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        Steer.color = Controller.IsActive ? steerColor : Color.clear;
        if (Controller.IsActive)
        {
            Vector2 startPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(steerRect, Controller.StartPosition, canvas.worldCamera, out startPosition);
            Steer.rectTransform.anchoredPosition = startPosition;
            var euler = Steer.transform.localRotation.eulerAngles;
            euler.z = Controller.VirtualInput.Axis * SteerAngle;
            Steer.transform.localRotation  = Quaternion.Euler(euler);
        }
    }
}
