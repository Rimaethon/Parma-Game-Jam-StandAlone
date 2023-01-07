using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LayoutSwitchUI : MonoBehaviour
{
    public UnityEvent OnKeyboardMouseInput;
    public UnityEvent OnGamepadInput;
    public UnityEvent OnTouchscreenInput;

    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        OnKeyboardMouseInput.Invoke();
#elif UNITY_WII || UNITY_PS4 || UNITY_XBOXONE
        OnGamepadInput.Invoke();
#else
        OnTouchscreenInput.Invoke();
#endif
        foreach (var hub in GetComponentsInChildren<InputHubController>())
        {
            hub.OnInputModeChanged.AddListener(delegate
            {
                switch (hub.inputMode)
                {
                    case InputHubController.Mode.None:
                    case InputHubController.Mode.Keyboard:
                    case InputHubController.Mode.Mouse:
                        OnKeyboardMouseInput.Invoke();
                        break;
                    case InputHubController.Mode.Gamepad:
                        OnGamepadInput.Invoke();
                        break;
                    case InputHubController.Mode.Touchscreen:
                        OnTouchscreenInput.Invoke();
                        break;
                }
            });
        }    
    }
}
