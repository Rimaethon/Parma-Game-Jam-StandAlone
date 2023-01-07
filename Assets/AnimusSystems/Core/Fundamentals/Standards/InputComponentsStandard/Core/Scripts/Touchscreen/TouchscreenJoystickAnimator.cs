using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TouchscreenJoystickAnimator : MonoBehaviour
{
    public Graphic Base;
    public Graphic Button;
    public TouchscreenJoystickController Controller;

    private Color baseColor;
    private Color buttonColor;
    private RectTransform joystickRect;
    private Canvas canvas;

    void Start()
    {
        baseColor = Base.color;
        buttonColor = Button.color;
        joystickRect = Base.transform.parent.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        Base.color = Controller.IsActive ? baseColor : Color.clear;
        Button.color = Controller.IsActive ? buttonColor : Color.clear;

        if (Controller.IsActive)
        {
            Vector2 startPosition; 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickRect, Controller.StartPosition, canvas.worldCamera, out startPosition);
            Base.rectTransform.anchoredPosition = startPosition;
            Button.rectTransform.anchoredPosition = Controller.VirtualInput.Direction * Base.rectTransform.sizeDelta.x;
        }
    }
}
