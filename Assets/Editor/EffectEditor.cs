using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Effect))]
public class EffectEditor : Editor
{
    SerializedProperty dataProp;

    void OnEnable()
    {
        dataProp = serializedObject.FindProperty("data");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector
        DrawDefaultInspector();

        // Draw the _targetEffects and _casterEffects fields
        EditorGUILayout.PropertyField(dataProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
