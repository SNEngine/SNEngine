using SNEngine.Graphs;
using UnityEditor;
using UnityEngine;
using SiphoinUnityHelpers.XNodeExtensions;
using System.Linq;
using XNodeEditor;

namespace SNEngine.Editor
{
    [CustomEditor(typeof(VariableContainerGraph), true)]
    public class VariableContainerGraphInspector : UnityEditor.Editor
    {
        private VariableContainerGraph _graph;

        private void OnEnable()
        {
            _graph = target as VariableContainerGraph;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawOpenGraphButton();

            DrawGuidInfo();

            GUILayout.Space(5);

            DrawVariablesList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawOpenGraphButton()
        {
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            if (GUILayout.Button("Open", GUILayout.Height(30)))
            {
                NodeEditorWindow.Open(_graph);
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(5);
        }

        private void DrawGuidInfo()
        {
            SerializedProperty guidProp = serializedObject.FindProperty("_guid");
            if (guidProp != null)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("GUID:", EditorStyles.miniBoldLabel, GUILayout.Width(40));
                EditorGUILayout.SelectableLabel(guidProp.stringValue, EditorStyles.miniLabel, GUILayout.Height(15));
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawVariablesList()
        {
            if (_graph == null || _graph.nodes == null) return;

            var varitableNodes = _graph.nodes.OfType<VariableNode>().ToList();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            EditorGUILayout.LabelField($"Variables in Container ({varitableNodes.Count})", EditorStyles.boldLabel);
            GUILayout.Space(5);

            for (int i = 0; i < varitableNodes.Count; i++)
            {
                var node = varitableNodes[i];
                if (node == null) continue;

                const float rowHeight = 44f;
                const float iconSize = 32f;

                Rect rowRect = GUILayoutUtility.GetRect(0, rowHeight, GUILayout.ExpandWidth(true));

                if (i % 2 == 0)
                {
                    EditorGUI.DrawRect(rowRect, new Color(1f, 1f, 1f, 0.03f));
                }

                EditorGUIUtility.AddCursorRect(rowRect, MouseCursor.Link);
                if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
                {
                    NodeEditorWindow window = NodeEditorWindow.Open(_graph);
                    window.Home();
                    Selection.activeObject = node;
                    Event.current.Use();
                }

                Rect colorStrip = new Rect(rowRect.x + 2, rowRect.y + 4, 4, rowRect.height - 8);
                EditorGUI.DrawRect(colorStrip, node.Color);

                GUIContent iconContent = EditorGUIUtility.ObjectContent(null, node.GetType());
                Texture nodeIcon = iconContent.image;
                if (nodeIcon == null) nodeIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

                Rect iconRect = new Rect(rowRect.x + 14, rowRect.y + (rowHeight - iconSize) / 2, iconSize, iconSize);
                if (nodeIcon != null) GUI.DrawTexture(iconRect, nodeIcon, ScaleMode.ScaleToFit);

                Rect labelRect = new Rect(iconRect.xMax + 10, rowRect.y, rowRect.width - iconRect.xMax - 20, rowHeight);

                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.alignment = TextAnchor.MiddleLeft;
                labelStyle.fontSize = 14;
                labelStyle.fontStyle = FontStyle.Bold;

                GUI.Label(labelRect, node.Name, labelStyle);

                GUIStyle typeStyle = new GUIStyle(EditorStyles.miniLabel);
                typeStyle.alignment = TextAnchor.MiddleRight;
                typeStyle.fontSize = 11;
                typeStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
                string typeName = node.GetType().Name.Replace("Node", "");
                GUI.Label(labelRect, $"[{typeName}]  ", typeStyle);

                GUILayout.Space(2);
            }

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }
    }
}