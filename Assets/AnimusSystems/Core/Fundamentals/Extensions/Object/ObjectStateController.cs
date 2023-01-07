using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class ObjectStateController : MonoBehaviour
{
    [SerializeField, ReadOnlyField] private int stateIndex;
    public UnityEvent OnBeforeStateChange;
    public State[] States;

    public int StateIndex { get => stateIndex;
        set
        {
            var newState = Mathf.Clamp(value, 0, States.Length - 1);
            if (stateIndex == newState) return;
            OnBeforeStateChange.Invoke();
            stateIndex = newState;
            States[stateIndex].OnSelect.Invoke();
        }
    }
    public string StateName {
        get => States[StateIndex].name;
        set
        {
            var newStateIndex = Array.FindIndex(States, s => s.name == value);
            if (newStateIndex != -1) StateIndex = newStateIndex;
        }
    }

    public void SelectNext() {
        StateIndex = (StateIndex + 1) % States.Length;
    }
    public void SelectPrevious() {
        if (StateIndex == 0) StateIndex = States.Length - 1;
        else StateIndex--;
    }
    [System.Serializable]
    public class State
    {
        public string name;
        public UnityEvent OnSelect;
    }
}
