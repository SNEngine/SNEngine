using SNEngine.DialogSystem;
using SNEngine.Graphs;
using SNEngine.Services;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace SNEngine.Editor
{
    [CustomEditor(typeof(DialogueGraph), true)]
    public class DialogueGraphEditorInspector : UnityEditor.Editor
    {
        private DialogueGraph _graph;
        private static DialogueService _dialogueService;
        private static Texture2D _nodeIcon;

        private static Texture2D NodeIcon
        {
            get
            {
                if (_nodeIcon == null)
                {
                    _nodeIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SNEngine/Source/SNEngine/Editor/Sprites/node_editor_icon.png");
                }
                return _nodeIcon;
            }
        }

        private void OnEnable()
        {
            _graph = target as DialogueGraph;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPlayButton();

            DrawGuidInfo();

            GUILayout.Space(5);

            DrawNodesList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPlayButton()
        {
            if (Application.isPlaying)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Play Dialogue", GUILayout.Height(40)))
                {
                    if (_graph != null)
                    {
                        if (_dialogueService is null)
                        {
                            _dialogueService = NovelGame.Instance.GetService<DialogueService>();
                        }

                        NovelGame.Instance.ResetStateServices();
                        NovelGame.Instance.GetService<MainMenuService>().Hide();
                        _dialogueService.JumpToDialogue(_graph);
                    }
                }
                GUI.backgroundColor = Color.white;
                GUILayout.Space(5);
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play Mode to test 'Play Dialogue'.", MessageType.Info);
            }
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

        private void DrawNodesList()
        {
            if (_graph == null || _graph.nodes == null) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            EditorGUILayout.LabelField($"Nodes in Graph ({_graph.nodes.Count})", EditorStyles.boldLabel);
            GUILayout.Space(5);

            for (int i = 0; i < _graph.nodes.Count; i++)
            {
                var node = _graph.nodes[i];
                if (node == null) continue;

                Rect rect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(32));

                if (i % 2 == 0)
                {
                    EditorGUI.DrawRect(rect, new Color(1f, 1f, 1f, 0.03f));
                }

                GUILayout.Space(5);

                EditorGUILayout.BeginVertical(GUILayout.Width(24), GUILayout.Height(32));
                GUILayout.FlexibleSpace();
                Rect iconRect = GUILayoutUtility.GetRect(20, 20);
                if (NodeIcon != null)
                {
                    GUI.DrawTexture(iconRect, NodeIcon, ScaleMode.ScaleToFit);
                }
                else
                {
                    EditorGUI.DrawRect(iconRect, new Color(1f, 1f, 1f, 0.1f));
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();

                GUILayout.Space(8);

                EditorGUILayout.BeginVertical(GUILayout.Height(32));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(node.name, EditorStyles.label);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }
    }
}