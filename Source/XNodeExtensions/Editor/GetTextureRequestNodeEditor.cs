#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.Web;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(GetTextureRequestNode))]
    public class GetTextureRequestNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();
            GetTextureRequestNode node = target as GetTextureRequestNode;

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_texture") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            SerializedProperty texProp = serializedObject.FindProperty("_texture");
            Texture2D tex = texProp.objectReferenceValue as Texture2D;

            if (tex != null)
            {
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(GUI.skin.box);

                Rect rect = GUILayoutUtility.GetRect(10, 100);
                GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit);

                EditorGUILayout.LabelField($"{tex.width}x{tex.height}", EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.EndVertical();
            }

            NodeEditorGUILayout.PortField(new GUIContent("Downloaded Texture"), node.GetOutputPort("_texture"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif