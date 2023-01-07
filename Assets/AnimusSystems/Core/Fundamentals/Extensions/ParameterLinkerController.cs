using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

public class ParameterLinkerController : MonoBehaviour
{
    public Object CustomEntryComponent;
    public string CustomEntryParameter;
    private Transform customEntryPoint;

    public ParameterLink[] Links;

    void Start()
    {
        if (CustomEntryComponent != null)
        {
            var entryField = CustomEntryComponent.GetType().GetField(CustomEntryParameter);
            if (entryField != null)
                customEntryPoint = ((Component)entryField.GetValue(CustomEntryComponent)).transform;
            else
            {
                customEntryPoint = ((Component)(CustomEntryComponent.GetType().GetProperty(CustomEntryParameter).GetValue(CustomEntryComponent))).transform;
            }
        }


        for (int i = 0; i < Links.Length; i++) Links[i].Init(transform, customEntryPoint);
    }
    private void OnTransformParentChanged()
    {
        for (int i = 0; i < Links.Length; i++) Links[i].Init(transform, customEntryPoint);
    }

    void Update()
    {
        for (int i = 0; i < Links.Length; i++) Links[i].Update();
    }

    [System.Serializable]
    public class ParameterLink
    {
        public string name;
        [Header("Source")]
        public string SourceObjectPath;
        public string SourceComponentType;
        public string SourceParameterName;
        public EntryPointMode SourceEntryPoint;

        private object source;
        private FieldInfo sourceField;
        private PropertyInfo sourceProperty;
        private object sourceValue;

        private AnimatorParameterType SourceAnimatorParameterType;
        private Animator sourceAnimator;

        [Header("Target")]
        public string TargetObjectPath;
        public string TargetComponentType;
        public string TargetParameterName;
        public EntryPointMode TargetEntryPoint;

        private object target;
        private FieldInfo targetField;
        private PropertyInfo targetProperty;

        private AnimatorParameterType TargetAnimatorParameterType;
        private Animator targetAnimator;

        const string FloatToStringFormat = "0.00";

        static void GetNestedParameter(ref object source, ref FieldInfo field, ref PropertyInfo property, string path)
        {
            if (source == null) return;
            var names = path.Split('.');
            for (int i=0; i<names.Length;i++)
            {
                if (i+1==names.Length)
                {
                    field = source.GetType().GetField(names[i]);
                    property = source.GetType().GetProperty(names[i]);
                } else
                {
                    var childField = source.GetType().GetField(names[i])?.GetValue(source);
                    if (childField != null) source = childField;
                    else source = source.GetType().GetProperty(names[i])?.GetValue(source);
                }
            }
        }
        static object GetValue(object obj, string parameterName, FieldInfo field, PropertyInfo property, Animator animator, AnimatorParameterType animatorParameterType)
        {
            switch (animatorParameterType)
            {
                case AnimatorParameterType.None:
                    if (field != null) return field.GetValue(obj);
                    if (property != null) return property.GetValue(obj);
                    break;
                case AnimatorParameterType.Boolean:
                    return animator.GetBool(parameterName);
                case AnimatorParameterType.Float:
                    return animator.GetFloat(parameterName);
                case AnimatorParameterType.Integer:
                    return animator.GetInteger(parameterName);
            }
            return null;
        }
        static void SetValue(object obj, string parameterName, FieldInfo field, PropertyInfo property, Animator animator, AnimatorParameterType animatorParameterType, object value)
        {
            switch (animatorParameterType)
            {
                case AnimatorParameterType.None:
                    if (field != null)
                    {
                        if (value.GetType() == typeof(float) && field.GetType() == typeof(string))
                        {
                            field.SetValue(obj, ((float)value).ToString(FloatToStringFormat));
                        }
                        else
                        {
                            field.SetValue(obj, Convert.ChangeType(value, field.GetType()));
                        }
                    }
                    else if (property != null)
                    {
                        if (value.GetType() == typeof(float) && property.PropertyType == typeof(string))
                        {
                            property.SetValue(obj, ((float)value).ToString(FloatToStringFormat));
                        }
                        else
                        {
                            property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                    break;
                case AnimatorParameterType.Boolean:
                    animator.SetBool(parameterName, (bool)Convert.ChangeType(value, typeof(bool)));
                    break;
                case AnimatorParameterType.Float:
                    animator.SetFloat(parameterName, (float)Convert.ChangeType(value, typeof(float)));
                    break;
                case AnimatorParameterType.Integer:
                    animator.SetInteger(parameterName, (int)Convert.ChangeType(value, typeof(int)));
                    break;
            }
        }
        static void GetAnimatorParameterType(object source, ref Animator animator, string name, ref AnimatorParameterType animatorParameterType)
        {
            if (!(source is Animator)) return;
            animator = source as Animator;
            var parameter = animator.parameters.FirstOrDefault(p => p.name == name);
            if (parameter==null) return;
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Bool: animatorParameterType = AnimatorParameterType.Boolean; break;
                case AnimatorControllerParameterType.Float: animatorParameterType = AnimatorParameterType.Float; break;
                case AnimatorControllerParameterType.Int: animatorParameterType = AnimatorParameterType.Integer; break;
            }
        }

        public void Init(Transform thisTransform, Transform customEntryPoint)
        {
            var sourceEntryTransform = SourceEntryPoint == EntryPointMode.ThisTransform ? thisTransform : customEntryPoint;
            var targetEntryTransform = TargetEntryPoint == EntryPointMode.ThisTransform ? thisTransform : customEntryPoint;
            source = sourceEntryTransform.FindByPath(SourceObjectPath)?.GetComponent(SourceComponentType);
            target = targetEntryTransform.FindByPath(TargetObjectPath)?.GetComponent(TargetComponentType);

            GetAnimatorParameterType(source, ref sourceAnimator, SourceParameterName, ref SourceAnimatorParameterType);
            GetAnimatorParameterType(target, ref targetAnimator, TargetParameterName, ref TargetAnimatorParameterType);

            if (SourceAnimatorParameterType == AnimatorParameterType.None)
                GetNestedParameter(ref source, ref sourceField, ref sourceProperty, SourceParameterName);
            if (TargetAnimatorParameterType == AnimatorParameterType.None)
                GetNestedParameter(ref target, ref targetField, ref targetProperty, TargetParameterName);
        }


        public void Update()
        {
            if (source == null || target == null) return;
            SetValue(target, TargetParameterName, targetField, targetProperty, targetAnimator, TargetAnimatorParameterType,
                GetValue(source, SourceParameterName, sourceField, sourceProperty, sourceAnimator, SourceAnimatorParameterType));
        }

        public enum AnimatorParameterType { None, Boolean, Integer, Float }
        public enum EntryPointMode { ThisTransform, CustomEntryPoint }
    }
}
