using UnityEditor;

namespace DeepSky.Haze
{
    [CustomEditor(typeof(DS_HazeCore))]
    public class DS_HazeCoreEditor : Editor
    {
        private const string kHelpTxt = "This is the time at which zones are evaluated during rendering. " +
            "Animate it, or set via a time-of-day system, to create transitions throughout a day/night cycle.";

        /// <summary>
        /// Custom Inspector drawing for DS_HazeCore components.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(kHelpTxt, MessageType.Info);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HeightFalloff"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Time"));
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}