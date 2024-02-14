using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(CharacterData))]
public class IngredientDrawerUIE : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        // EditorGUI.BeginProperty(position, label, property);

        // // Draw label
        // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // // Don't make child fields be indented
        // var indent = EditorGUI.indentLevel;
        // EditorGUI.indentLevel = 0;

        // // Calculate rects
        // var charNameRect = new Rect(position.x, position.y, position.width, position.height);
        // var maxHealthRect = new Rect(position.x, position.y, position.width + 50, position.height);
        // var currentHealthRect = new Rect(position.x, position.y, position.width + 100, position.height);

        // // Draw fields - pass GUIContent.none to each so they are drawn without labels
        // EditorGUI.PropertyField(charNameRect, property.FindPropertyRelative("characterName"), GUIContent.none);
        // EditorGUI.PropertyField(maxHealthRect, property.FindPropertyRelative("maxHealth"), GUIContent.none);
        // EditorGUI.PropertyField(currentHealthRect, property.FindPropertyRelative("currentHealth"), GUIContent.none);

        // // Set indent back to what it was
        // EditorGUI.indentLevel = indent;

        // EditorGUI.EndProperty();
    }
}


// [CustomPropertyDrawer(typeof(CharacterData))]
// public class CharacterDataListDrawer : PropertyDrawer
// {
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         EditorGUI.BeginProperty(position, label, property);

//         // Draw the individual properties of CharacterData manually
//         position.height = EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(position, property.FindPropertyRelative("characterName"));

//         position.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(position, property.FindPropertyRelative("maxHealth"));

//         position.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(position, property.FindPropertyRelative("currentHealth"));

//         EditorGUI.EndProperty();
//     }

//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         // Calculate the total height needed for the drawn properties
//         return EditorGUIUtility.singleLineHeight * 3; // Adjust based on the number of properties
//     }
// }
