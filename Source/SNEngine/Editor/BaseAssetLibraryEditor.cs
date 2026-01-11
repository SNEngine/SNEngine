#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SNEngine.Serialization;

namespace SNEngine.Editor
{
    [CustomEditor(typeof(BaseAssetLibrary), true)]
    public class BaseAssetLibraryEditor : UnityEditor.Editor
    {
        private SerializedProperty _entriesProp;

        private void OnEnable()
        {
            _entriesProp = serializedObject.FindProperty("_entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Library Contents (Read Only)", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (_entriesProp == null || _entriesProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Library is empty or property not found.", MessageType.Info);
            }
            else
            {
                GUI.enabled = false;
                DrawCompactList();
                GUI.enabled = true;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCompactList()
        {
            for (int i = 0; i < _entriesProp.arraySize; i++)
            {
                SerializedProperty entry = _entriesProp.GetArrayElementAtIndex(i);

                SerializedProperty guidProp = entry.FindPropertyRelative("<Guid>k__BackingField");
                SerializedProperty assetProp = entry.FindPropertyRelative("<Asset>k__BackingField");

                Object asset = assetProp != null ? assetProp.objectReferenceValue : null;
                string guid = guidProp != null ? guidProp.stringValue : "N/A";

                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                Rect previewRect = GUILayoutUtility.GetRect(32, 32, GUILayout.Width(32));
                if (asset != null)
                {
                    Texture2D texture = AssetPreview.GetAssetPreview(asset);
                    if (texture != null)
                        GUI.DrawTexture(previewRect, texture, ScaleMode.ScaleToFit);
                    else
                        EditorGUI.DrawRect(previewRect, new Color(0.15f, 0.15f, 0.15f));
                }
                else
                {
                    EditorGUI.DrawRect(previewRect, new Color(0.3f, 0.1f, 0.1f));
                }

                // Text Data
                EditorGUILayout.BeginVertical();
                string assetName = asset != null ? asset.name : "Missing Reference";
                EditorGUILayout.LabelField(assetName, EditorStyles.boldLabel);

                GUIStyle guidStyle = new GUIStyle(EditorStyles.miniLabel);
                guidStyle.normal.textColor = Color.gray;
                EditorGUILayout.LabelField(guid, guidStyle);
                EditorGUILayout.EndVertical();

                GUI.enabled = true;
                if (asset != null && GUILayout.Button("Ping", GUILayout.Width(40), GUILayout.Height(30)))
                {
                    EditorGUIUtility.PingObject(asset);
                }
                GUI.enabled = false;

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif