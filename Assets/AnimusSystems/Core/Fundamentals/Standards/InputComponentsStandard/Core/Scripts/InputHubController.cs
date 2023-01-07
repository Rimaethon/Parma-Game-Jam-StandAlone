using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputHubController : MonoBehaviour
{
    public Mode inputMode { get { return InputMode; } }
    [SerializeField, ReadOnlyField] private Mode InputMode;
    public UnityEvent OnInputModeChanged;
    public AvatarOverallInput.VirtualInputComponent Input;

    [SerializeField]private InputComponentController Keyboard;
    [SerializeField]private InputComponentController Mouse;
    [SerializeField]private InputComponentController Gamepad;
    [SerializeField]private InputComponentController Touchscreen;

    public void Process()
    {
        if (Touchscreen) Touchscreen.Process();
        if (Gamepad) Gamepad.Process();
        if (Mouse) Mouse.Process();
        if (Keyboard) Keyboard.Process();

        var newMode = InputMode;
        if (Touchscreen && Touchscreen.IsActive) newMode = Mode.Touchscreen;
        else if (Gamepad && Gamepad.IsActive) newMode = Mode.Gamepad;
        else if (Mouse && Mouse.IsActive) newMode = Mode.Mouse;
        else if (Keyboard && Keyboard.IsActive) newMode = Mode.Keyboard;

        if (newMode!=InputMode)
        {
            InputMode = newMode;
            OnInputModeChanged.Invoke();
        }
        switch (InputMode)
        {
            case Mode.Keyboard: Input = Keyboard.VirtualInput; break;
            case Mode.Mouse: Input = Mouse.VirtualInput; break;
            case Mode.Gamepad: Input = Gamepad.VirtualInput; break;
            case Mode.Touchscreen: Input = Touchscreen.VirtualInput; break;
        }
    }
    public enum Mode { None, Keyboard, Mouse, Gamepad, Touchscreen }
}
