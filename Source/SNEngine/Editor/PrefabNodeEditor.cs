#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.Variables;
using SiphoinUnityHelpers.Editor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(PrefabNode))]
    public class PrefabNodeEditor : NodeEditor
    {
        private Texture2D _fallbackIcon;
        private const string FALLBACK_ICON_PATH = "Assets/SNEngine/Source/SNEngine/Editor/Sprites/prefab_editor_icon.png";

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            if (_fallbackIcon == null)
            {
                _fallbackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(FALLBACK_ICON_PATH);
            }

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_value") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            GUILayout.Space(5);

            SerializedProperty valueProp = serializedObject.FindProperty("_value");
            GameObject currentPrefab = valueProp.objectReferenceValue as GameObject;

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentPrefab != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            Rect rect = GUILayoutUtility.GetRect(10, currentPrefab != null ? 70 : 30);

            if (GUI.Button(rect, currentPrefab == null ? "Select Prefab" : ""))
            {
                PrefabSelectorWindow.Open((selected) => {
                    var so = new SerializedObject(target);
                    var p = so.FindProperty("_value");
                    if (p != null)
                    {
                        p.objectReferenceValue = selected;
                        so.ApplyModifiedProperties();
                    }
                });
            }

            if (currentPrefab != null)
            {
                Texture preview = AssetPreview.GetAssetPreview(currentPrefab);

                if (preview == null || AssetPreview.IsLoadingAssetPreview(currentPrefab.GetInstanceID()))
                {
                    preview = _fallbackIcon != null ? _fallbackIcon : EditorGUIUtility.IconContent("Prefab Icon").image;
                }

                if (preview != null) GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);

                Rect labelRect = new Rect(rect.x, rect.yMax - 16, rect.width, 16);
                EditorGUI.DrawRect(labelRect, new Color(0, 0, 0, 0.6f));
                GUI.Label(labelRect, currentPrefab.name, EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = prevBg;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif