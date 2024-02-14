using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(List<CharacterData>))]
public class CharacterDataListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the individual properties of CharacterData manually
        EditorGUI.PropertyField(position, property.FindPropertyRelative("characterName"));
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("maxHealth"));
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("currentHealth"));
        // Repeat for other properties...

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Calculate the total height needed for the drawn properties
        int propertyCount = 3; // Adjust based on the number of properties
        return EditorGUIUtility.singleLineHeight * propertyCount;
    }
}
