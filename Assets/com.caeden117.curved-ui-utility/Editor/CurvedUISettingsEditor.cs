using UnityEditor;

namespace CurvedUIUtility.Editor
{
    [CustomEditor(typeof(CurvedUIController))]
    public class CurvedUISettingsEditor : UnityEditor.Editor
    {
        private SettingsSource settingsSource;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settingsSource"));

            settingsSource = (SettingsSource)serializedObject.FindProperty("settingsSource").enumValueIndex;

            EditorGUILayout.Space();

            switch (settingsSource)
            {
                case SettingsSource.FromStartingSettings:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("startingCurveSettings"));
                    break;

                case SettingsSource.FromScriptableObject:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("startingCurveObject"));
                    break;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("curveTransition"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
