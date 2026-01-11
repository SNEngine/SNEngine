#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.Audio;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(PlaySoundOneShotNode))]
    public class PlaySoundOneShotNodeEditor : NodeEditor
    {
        private Texture2D _fallbackIcon;
        private const string FALLBACK_ICON_PATH = "Assets/SNEngine/Source/SNEngine/Editor/Sprites/audio_editor_icon.png";

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            if (_fallbackIcon == null)
            {
                _fallbackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(FALLBACK_ICON_PATH);
            }

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_clip") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            GUILayout.Space(5);

            SerializedProperty clipProp = serializedObject.FindProperty("_clip");
            AudioClip currentClip = clipProp.objectReferenceValue as AudioClip;

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentClip != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            Rect rect = GUILayoutUtility.GetRect(10, 70);

            if (GUI.Button(rect, ""))
            {
                AudioClipSelectorWindow.Open((selected) => {
                    var so = new SerializedObject(target);
                    var p = so.FindProperty("_clip");
                    if (p != null)
                    {
                        p.objectReferenceValue = selected;
                        so.ApplyModifiedProperties();
                    }
                });
            }

            if (currentClip == null)
            {
                GUI.Label(rect, "Select Clip", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                if (_fallbackIcon != null)
                {
                    GUI.DrawTexture(rect, _fallbackIcon, ScaleMode.ScaleToFit);
                }

                Rect labelRect = new Rect(rect.x, rect.yMax - 16, rect.width, 16);
                EditorGUI.DrawRect(labelRect, new Color(0, 0, 0, 0.6f));
                GUI.Label(labelRect, currentClip.name, EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = prevBg;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif