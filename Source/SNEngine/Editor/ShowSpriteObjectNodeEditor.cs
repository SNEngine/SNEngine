#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.SpriteObjectSystem;
using SiphoinUnityHelpers.Editor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(ShowSpriteObjectNode))]
    public class ShowSpriteObjectNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();
            ShowSpriteObjectNode node = target as ShowSpriteObjectNode;

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_sprite") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            GUILayout.Space(10);

            SerializedProperty spriteProp = serializedObject.FindProperty("_sprite");
            Sprite currentSprite = spriteProp.objectReferenceValue as Sprite;

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentTextureColor(currentSprite);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            Rect rect = GUILayoutUtility.GetRect(10, currentSprite != null ? 80 : 32);
            string btnText = currentSprite != null ? "" : "Select Sprite";

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

                Rect labelRect = new Rect(rect.x, rect.yMax - 16, rect.width, 16);
                EditorGUI.DrawRect(labelRect, new Color(0, 0, 0, 0.6f));
                GUI.Label(labelRect, currentSprite.name, EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = prevBg;

            var spritePort = node.GetInputPort("_sprite");
            if (spritePort != null)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Sprite Input"), spritePort);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private Color currentTextureColor(Sprite s) => s != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

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
            }, SpriteSelectorWindow.SpriteCategory.All);
        }
    }
}
#endif