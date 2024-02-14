using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(PartyManager))]
public class PartyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
// [CustomEditor(typeof(PartyManager))]
// public class PartyManagerEditor : Editor
// {
//     SerializedProperty activePartyMembersProperty;

//     void OnEnable()
//     {
//         // Initialize serialized properties
//         activePartyMembersProperty = serializedObject.FindProperty("activePartyMembers");
//     }

//     public override void OnInspectorGUI()
//     {
//         // Update the serialized object
//         serializedObject.Update();

//         // Draw default inspector
//         DrawDefaultInspector();

//         // Draw custom inspector for Active Party Members
//         DrawActivePartyMembers();

//         // Apply modifications
//         serializedObject.ApplyModifiedProperties();
//     }

//     void DrawActivePartyMembers()
//     {
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("Active Party Members", EditorStyles.boldLabel);

//         // Iterate through Active Party Members and draw each element
//         for (int i = 0; i < activePartyMembersProperty.arraySize; i++)
//         {
//             SerializedProperty element = activePartyMembersProperty.GetArrayElementAtIndex(i);

//             // Use PropertyField to draw the element
//             EditorGUILayout.PropertyField(element, new GUIContent("Member " + i), true);
//         }
//     }
// }
