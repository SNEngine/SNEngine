#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.BackgroundSystem;
using SiphoinUnityHelpers.Editor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(SetBackgroundNode))]
    public class SetBackgroundNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_sprite") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            GUILayout.Space(10);

            SerializedProperty spriteProp = serializedObject.FindProperty("_sprite");
            Sprite currentSprite = spriteProp.objectReferenceValue as Sprite;

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentSprite != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            Rect rect = GUILayoutUtility.GetRect(10, currentSprite != null ? 100 : 40);

            string btnText = currentSprite != null ? "" : "Select Background";

            if (GUI.Button(rect, btnText))
            {
                OpenSelector();
            }

            if (currentSprite != null)
            {
                Texture2D preview = AssetPreview.GetAssetPreview(currentSprite);
                if (preview != null)
                {
                    GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
                }

                Rect labelRect = new Rect(rect.x, rect.yMax - 18, rect.width, 18);
                EditorGUI.DrawRect(labelRect, new Color(0, 0, 0, 0.6f));
                GUI.Label(labelRect, currentSprite.name, EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.EndVertical();

            GUI.backgroundColor = prevBg;
            serializedObject.ApplyModifiedProperties();
        }

        private void OpenSelector()
        {
            SpriteSelectorWindow.Open((selected) => {
                var so = new SerializedObject(target);
                var p = so.FindProperty("_sprite");
                if (p != null)
                {
                    p.objectReferenceValue = selected;
                    so.ApplyModifiedProperties();
                }
            }, SpriteSelectorWindow.SpriteCategory.Backgrounds);
        }
    }
}
#endif