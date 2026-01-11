#if UNITY_EDITOR
using SiphoinUnityHelpers.Editor;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.Editor;
using SiphoinUnityHelpers.XNodeExtensions.Variables;
using SiphoinUnityHelpers.XNodeExtensions.Variables.Set;
using SNEngine.Graphs;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(SetTextureNode))]
    public class SetTextureNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();
            SetTextureNode node = target as SetTextureNode;

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_variable" || tag.name == "_targetGuid" || tag.name == "_enumerable") continue;
                if (tag.name == "_value" || tag.name == "_inputTexture") continue;

                SerializedProperty p = serializedObject.FindProperty(tag.name);
                if (p != null) NodeEditorGUILayout.PropertyField(p);
            }

            DrawVariableSelector(node);

            GUILayout.Space(5);
            DrawTexturePreview();

            NodePort inputPort = node.GetInputPort("_inputTexture");
            if (inputPort != null)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Texture Input"), inputPort);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawVariableSelector(SetTextureNode node)
        {
            string currentVarName = "Select Target Variable";
            if (!string.IsNullOrEmpty(node.TargetGuid))
            {
                var targetNode = (node.graph as BaseGraph)?.GetNodeByGuid(node.TargetGuid) as VariableNode;
                if (targetNode != null) currentVarName = $"Target: {targetNode.name}";
            }

            if (GUILayout.Button(currentVarName, GUILayout.Height(25)))
            {
                VariableselectorWindow.Open(node.graph as BaseGraph, typeof(Texture), (selectedNode) =>
                {
                    var so = new SerializedObject(node);
                    so.FindProperty("_targetGuid").stringValue = selectedNode.GUID;
                    so.ApplyModifiedProperties();
                }, VariableselectorWindow.SelectorMode.LocalOnly);
            }
        }

        private void DrawTexturePreview()
        {
            SerializedProperty valueProp = serializedObject.FindProperty("_value");
            Texture2D currentTexture = valueProp.objectReferenceValue as Texture2D;

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentTexture != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            Rect rect = GUILayoutUtility.GetRect(10, currentTexture != null ? 80 : 35);

            if (GUI.Button(rect, currentTexture == null ? "Select Static Texture" : ""))
            {
                TextureSelectorWindow.Open((selected) => {
                    var so = new SerializedObject(target);
                    var p = so.FindProperty("_value");
                    if (p != null) { p.objectReferenceValue = selected; so.ApplyModifiedProperties(); }
                });
            }

            if (currentTexture != null)
            {
                GUI.DrawTexture(rect, currentTexture, ScaleMode.ScaleToFit);

                Rect labelRect = new Rect(rect.x, rect.yMax - 16, rect.width, 16);
                EditorGUI.DrawRect(labelRect, new Color(0, 0, 0, 0.6f));
                GUI.Label(labelRect, currentTexture.name, EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = prevBg;
        }
    }
}
#endif