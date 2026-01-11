#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.Variables.Set;
using SiphoinUnityHelpers.XNodeExtensions.Editor;
using SiphoinUnityHelpers.Editor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(SetSpriteNode))]
    public class SetSpriteNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_varitable" || tag.name == "_targetGuid" || tag.name == "_enumerable") continue;
                if (tag.name == "_value") continue;

                SerializedProperty p = serializedObject.FindProperty(tag.name);
                if (p != null) NodeEditorGUILayout.PropertyField(p);
            }

            GUILayout.Space(5);

            SerializedProperty valueProp = serializedObject.FindProperty("_value");
            Sprite currentSprite = valueProp.objectReferenceValue as Sprite;

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentSprite != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            Rect rect = GUILayoutUtility.GetRect(10, currentSprite != null ? 70 : 30);

            if (GUI.Button(rect, currentSprite == null ? "Select Sprite" : ""))
            {
                SpriteSelectorWindow.Open((selected) => {
                    var so = new SerializedObject(target);
                    var p = so.FindProperty("_value");
                    if (p != null)
                    {
                        p.objectReferenceValue = selected;
                        so.ApplyModifiedProperties();
                    }
                }, SpriteSelectorWindow.SpriteCategory.All);
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

            var inputPort = target.GetInputPort("_varitable");
            if (inputPort != null && !inputPort.IsConnected)
            {
                XNodeEditorHelpers.DrawSetVaritableBody(this, serializedObject);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif