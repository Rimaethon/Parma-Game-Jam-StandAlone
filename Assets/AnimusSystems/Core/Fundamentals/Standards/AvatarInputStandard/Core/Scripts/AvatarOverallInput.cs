using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AvatarOverallInput : MonoBehaviour
{
    [SerializeField, ReadOnlyField] private bool hasPlayer;
    public bool HasPlayer {
        get => hasPlayer;
        set
        {
            if (hasPlayer == value) return;
            if (value) OnPlayerEnter.Invoke();
            else OnPlayerExit.Invoke();
            hasPlayer = value;
        }
    }
    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;

    public Vector3 CameraPosition;
    [QuaternionField] public Quaternion LookRotation;
    public Vector3 LookPosition;
    public GameObject AvatarLayoutPrefab;
    [SerializeField, HideInInspector] private List<InputEntry> Inputs;
    private Dictionary<string, VirtualInputComponent> inputsMap;
    public Dictionary<string,VirtualInputComponent> inputs {
        get
        {
            if (inputsMap==null)
            {
                inputsMap = new Dictionary<string, VirtualInputComponent>();
                foreach (var e in Inputs) inputsMap.Add(e.key, e.value);
            }
            return inputsMap;
        }
    }

    [System.Serializable]
    class InputEntry
    {
        public string key;
        public VirtualInputComponent value;
    }

    [System.Serializable]
    public class VirtualInputComponent
    {
        public InputType Type;
        public Vector2 Direction;
        public float Axis;
        public UnityEvent OnPress;
        public UnityEvent OnRelease;
        [SerializeField]private bool IsPressed;
        private bool lastIsPressed;
        public bool isPressed
        {
            get { return lastIsPressed; }
            set {
                if (value && !lastIsPressed) OnPress.Invoke();
                else if (!value && lastIsPressed) OnRelease.Invoke();

                lastIsPressed = value;
            }
        }
        public enum InputType { Button, Axis, Direction }
    }
}
