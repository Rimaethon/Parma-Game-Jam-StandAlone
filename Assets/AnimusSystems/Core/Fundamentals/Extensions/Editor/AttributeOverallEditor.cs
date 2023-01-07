using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyField))]
public class ReadOnlyFieldDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        // avoid the field being completely hidden from inspector
        GUI.enabled = (attribute as ReadOnlyField).RuntimeReadOnly?!Application.isPlaying:false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
[CustomPropertyDrawer(typeof(LockableField))]
public class LockableFieldDrawer: PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var enabled = GUI.enabled;
        GUI.enabled = !property.serializedObject.FindProperty("lock" + property.name).boolValue;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = enabled;
    }
}
[CustomPropertyDrawer(typeof(QuaternionField))]
public class QuaternionFieldDrawer: PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var Euler = EditorGUI.Vector3Field(position, property.name, property.quaternionValue.eulerAngles);
        property.quaternionValue = Quaternion.Euler(Euler);
    }
}
[CustomPropertyDrawer(typeof(MinMaxField))]
public class MinMaxFieldDrawer:PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxField minMax = attribute as MinMaxField;
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            EditorGUI.LabelField(position, label);

            float shift = 0.4f;
            position.x += position.width * shift;
            position.xMax -= position.width * shift;

            int floatFieldWidth = 50;
            int spacing = 5;
            var minPosition = new Rect(position.x, position.y, floatFieldWidth, position.height);
            var maxPosition = new Rect(position.xMax - floatFieldWidth, position.y, floatFieldWidth, position.height);
            var sliderPosition = new Rect(position.x + floatFieldWidth+spacing, position.y, position.width - (floatFieldWidth+spacing)*2, position.height);

            var min = EditorGUI.FloatField(minPosition, property.vector2Value.x);
            var max = EditorGUI.FloatField(maxPosition, property.vector2Value.y);
            EditorGUI.MinMaxSlider(sliderPosition, ref min, ref max, minMax.MinLimit, minMax.MaxLimit);

            property.vector2Value = new Vector2(min, max);
        }
    }
}
 