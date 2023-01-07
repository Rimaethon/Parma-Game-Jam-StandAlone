using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentBaseConfiguration))]
public class ComponentBaseConfigurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        var Dict = serializedObject.FindProperty("Configurations");

        for (int i = 0; i < Dict.arraySize; i++)
        {
            var Element = Dict.GetArrayElementAtIndex(i);
            GUI.Box(EditorGUILayout.BeginVertical(), "");
            var DefaultColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0, 0.7f, 0);
            GUI.Box(EditorGUILayout.BeginVertical(), "");
            EditorGUILayout.Space();
            GUI.backgroundColor = DefaultColor;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(Element.FindPropertyRelative("key"), GUIContent.none);
            if (GUILayout.Button("x", GUILayout.Height(15), GUILayout.Width(20)))
            {
                Dict.DeleteArrayElementAtIndex(i);
                i--;
                continue;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            var value = Element.FindPropertyRelative("value");
            var depth = value.depth+1;
            var nextElement = value.Copy();
            nextElement.Next(false);
            while (value.Next(true))
            {
                if (SerializedProperty.EqualContents(value, nextElement)) break;
                if (value.depth > depth) continue;
                EditorGUILayout.PropertyField(value);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        if (GUILayout.Button("Add")) Dict.InsertArrayElementAtIndex(Dict.arraySize);
        serializedObject.ApplyModifiedProperties();
    }
}
