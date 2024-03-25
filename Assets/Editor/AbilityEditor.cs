using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ability))]
public class AbilityEditor : Editor
{
    SerializedProperty targetEffectsProp;
    SerializedProperty casterEffectsProp;

    void OnEnable()
    {
        targetEffectsProp = serializedObject.FindProperty("_targetEffects");
        casterEffectsProp = serializedObject.FindProperty("_casterEffects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector
        DrawDefaultInspector();

        // Draw the _targetEffects and _casterEffects fields
        EditorGUILayout.PropertyField(targetEffectsProp, true);
        EditorGUILayout.PropertyField(casterEffectsProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
