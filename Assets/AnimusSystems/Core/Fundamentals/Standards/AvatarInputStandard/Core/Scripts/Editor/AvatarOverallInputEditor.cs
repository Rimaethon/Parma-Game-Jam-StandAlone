using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AvatarOverallInput))]
public class AvatarOverallInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        var Dict = serializedObject.FindProperty("Inputs");
        
        for (int i=0; i<Dict.arraySize; i++)
        {
            var Element = Dict.GetArrayElementAtIndex(i);
            GUI.Box(EditorGUILayout.BeginVertical(),"");
            var DefaultColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0,0.7f,0.7f);
            GUI.Box(EditorGUILayout.BeginVertical(),"");
            EditorGUILayout.Space();
            GUI.backgroundColor = DefaultColor;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(Element.FindPropertyRelative("key"),GUIContent.none);
            var type = Element.FindPropertyRelative("value.Type");
            EditorGUILayout.PropertyField(type, GUIContent.none);
            if (GUILayout.Button("x", GUILayout.Height(15)))
            {
                Dict.DeleteArrayElementAtIndex(i);
                i--;
                continue;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            switch((AvatarOverallInput.VirtualInputComponent.InputType)type.enumValueIndex)
            {
                case AvatarOverallInput.VirtualInputComponent.InputType.Button:
                    EditorGUILayout.PropertyField(Element.FindPropertyRelative("value.IsPressed"));
                    EditorGUILayout.PropertyField(Element.FindPropertyRelative("value.OnPress"));
                    EditorGUILayout.PropertyField(Element.FindPropertyRelative("value.OnRelease"));
                    break;
                case AvatarOverallInput.VirtualInputComponent.InputType.Axis:
                    EditorGUILayout.PropertyField(Element.FindPropertyRelative("value.Axis"));
                    break;
                case AvatarOverallInput.VirtualInputComponent.InputType.Direction:
                    EditorGUILayout.PropertyField(Element.FindPropertyRelative("value.Direction"));
                    break;
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        if (GUILayout.Button("Add")) Dict.InsertArrayElementAtIndex(Dict.arraySize);
        serializedObject.ApplyModifiedProperties();
    }
}
