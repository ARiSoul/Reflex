using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelConfig))]
public sealed class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var level = (LevelConfig)target;

        EditorGUILayout.Space(10);

        float total = level.TotalWeight;

        if (total <= 0f)
        {
            EditorGUILayout.HelpBox(
                "All weights are 0. Target picking will be undefined (will fallback).",
                MessageType.Warning);
        }
        else
        {
            // If you choose to treat them like probabilities, this is useful guidance.
            if (Mathf.Abs(total - 1f) > 0.01f)
            {
                EditorGUILayout.HelpBox(
                    $"Weights total is {total:0.###}. This is OK (treated as weights), " +
                    $"but you can normalize to make them sum to 1.",
                    MessageType.Info);
            }
        }

        using (new EditorGUI.DisabledScope(total <= 0f))
        {
            if (GUILayout.Button("Normalize weights (sum = 1)"))
            {
                Undo.RecordObject(level, "Normalize Weights");
                level.NormalizeWeights();
                EditorUtility.SetDirty(level);
            }
        }
    }
}
