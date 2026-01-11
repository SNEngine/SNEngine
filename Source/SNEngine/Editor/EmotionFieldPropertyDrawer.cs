#if UNITY_EDITOR
using SNEngine.Attributes;
using SNEngine.CharacterSystem;
using UnityEditor;
using UnityEngine;
using SiphoinUnityHelpers.XNodeExtensions.Editor;

namespace SNEngine.Editor
{
    [CustomPropertyDrawer(typeof(EmotionFieldAttribute))]
    public class EmotionFieldPropertyDrawer : PropertyDrawer
    {
        private const float PREVIEW_SIZE = 120f;
        private const float BUTTON_HEIGHT = 22f;
        private const float PADDING = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObject is CharacterNode characterNode && characterNode.Character != null)
            {
                var emotion = characterNode.Character.GetEmotion(property.stringValue);
                if (emotion != null && emotion.Sprite != null)
                    return BUTTON_HEIGHT + PREVIEW_SIZE + (PADDING * 3);
            }
            return BUTTON_HEIGHT + PADDING;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.serializedObject.targetObject is not CharacterNode characterNode || characterNode.Character == null)
            {
                GUI.enabled = false;
                EditorGUI.TextField(position, label, "Assign Character first");
                GUI.enabled = true;
                EditorGUI.EndProperty();
                return;
            }

            Rect buttonRect = new Rect(position.x, position.y, position.width, BUTTON_HEIGHT);
            string btnText = string.IsNullOrEmpty(property.stringValue) ? "Select Emotion..." : property.stringValue;

            if (GUI.Button(buttonRect, btnText, EditorStyles.miniButton))
            {
                EmotionSelectorWindow.Open(characterNode.Character, (val) =>
                {
                    property.serializedObject.Update();
                    property.stringValue = val;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            var emotion = characterNode.Character.GetEmotion(property.stringValue);
            if (emotion != null && emotion.Sprite != null)
            {
                Texture preview = AssetPreview.GetAssetPreview(emotion.Sprite);
                if (preview != null)
                {
                    Rect bgRect = new Rect(position.x, position.y + BUTTON_HEIGHT + PADDING, position.width, PREVIEW_SIZE);
                    GUI.Box(bgRect, GUIContent.none, EditorStyles.helpBox);

                    float aspect = (float)preview.width / preview.height;
                    float drawHeight = PREVIEW_SIZE - 8;
                    float drawWidth = drawHeight * aspect;

                    if (drawWidth > position.width - 8)
                    {
                        drawWidth = position.width - 8;
                        drawHeight = drawWidth / aspect;
                    }

                    Rect textureRect = new Rect(
                        bgRect.x + (bgRect.width - drawWidth) / 2,
                        bgRect.y + (bgRect.height - drawHeight) / 2,
                        drawWidth,
                        drawHeight
                    );

                    GUI.DrawTexture(textureRect, preview, ScaleMode.ScaleToFit);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif