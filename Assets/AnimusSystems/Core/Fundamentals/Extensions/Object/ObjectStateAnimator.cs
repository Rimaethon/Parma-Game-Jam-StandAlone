using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ObjectStateAnimator : MonoBehaviour
{
    public string ActiveState { get; set; }
    public State[] States;
    public State Default;
    public bool UseDefaultState = true;
    private Dictionary<string, State> stateMap = new Dictionary<string, State>();
    private Animator animator;
    private State state;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        foreach (var state in States) stateMap.Add(state.name, state);
    } 

    void Update()
    {
        if (!string.IsNullOrEmpty(ActiveState) && stateMap.TryGetValue(ActiveState, out state)) state.Evaluate(animator);
        else if (UseDefaultState) Default.Evaluate(animator);
    }
    [System.Serializable]
    public class State
    {
        public string name;
        [Header("Float parameters")]
        public FloatParameter[] Floats;
        [Header("Boolean parameters")]
        public BoolParameter[] Booleans;
        [Header("Integer parameters")]
        public IntParameter[] Integers;

        public void Evaluate(Animator animator)
        {
            int i = 0;
            for (i = 0; i < Floats.Length; i++) Floats[i].Evaluate(animator);
            for (i = 0; i < Booleans.Length; i++) Booleans[i].Evaluate(animator);
            for (i = 0; i < Integers.Length; i++) Integers[i].Evaluate(animator);
        }
    }
    public class StateParameter
    {
        public string name;
        public virtual void Evaluate(Animator animator) { }
    }
    [System.Serializable]
    public class FloatParameter:StateParameter
    {
        public float Value;
        [Range(0, 1)] public float Lerp;

        public override void Evaluate(Animator animator)
        {
            animator.SetFloat(name, Mathf.Lerp(animator.GetFloat(name), Value, Lerp));
        }
    }
    [System.Serializable]
    public class BoolParameter:StateParameter
    {
        public bool Value;

        public override void Evaluate(Animator animator)
        {
            animator.SetBool(name, Value);
        }
    }
    [System.Serializable]
    public class IntParameter:StateParameter
    {
        public int Value;
        public override void Evaluate(Animator animator)
        {
            animator.SetInteger(name, Value);
        }
    }
}
