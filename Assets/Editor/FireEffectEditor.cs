using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FireEffect))]
public class FireEffectEditor : Editor
{
    SerializedProperty fireDataProp;
    SerializedProperty damagePerTurnProp;

    void OnEnable()
    {
        fireDataProp = serializedObject.FindProperty("FireData");
        damagePerTurnProp = serializedObject.FindProperty("DamagePerTurn");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector
        DrawDefaultInspector();

        // Draw the _targetEffects and _casterEffects fields
        EditorGUILayout.PropertyField(fireDataProp, true);
        EditorGUILayout.PropertyField(damagePerTurnProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}
