using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using XNode;
using SNEngine.Graphs;
using SNEngine.Services;
using SNEngine.DialogSystem;

namespace SNEngine.Editor
{
    public class DialogueManagerWindow : EditorWindow
    {
        private const string DialoguesResourcePath = "Dialogues";
        private const string StartDialogueName = "_startDialogue";
        private string _newDialogueName = "NewDialogue";
        private string _searchQuery = "";
        private Vector2 _scrollPosition;

        private static DialogueService _dialogueService;

        [MenuItem("SNEngine/Dialogue Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<DialogueManagerWindow>("Dialogue Manager");
            window.minSize = new Vector2(500, 400);
        }

        private void OnGUI()
        {
            DrawTopPanel();
            EditorGUILayout.Space(2);
            DrawSearchBar();
            DrawListSection();
        }

        private void DrawTopPanel()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Create:", EditorStyles.miniLabel, GUILayout.Width(45));
            _newDialogueName = EditorGUILayout.TextField(_newDialogueName, GUILayout.Height(18));
            if (GUILayout.Button("Create Asset", EditorStyles.miniButton, GUILayout.Width(90)))
            {
                CreateNewDialogue();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _searchQuery = EditorGUILayout.TextField(_searchQuery, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(45)))
            {
                _searchQuery = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawListSection()
        {
            var allDialogueAssets = Resources.LoadAll<DialogueGraph>(DialoguesResourcePath)
                .Where(d => d != null)
                .OrderBy(d => d.name != StartDialogueName)
                .ThenBy(d => d.name)
                .ToArray();

            var filteredAssets = string.IsNullOrEmpty(_searchQuery)
                ? allDialogueAssets
                : allDialogueAssets.Where(d =>
                    d.name.IndexOf(_searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Space(5);
            for (int i = 0; i < filteredAssets.Length; i++)
            {
                DrawDialogueItem(filteredAssets[i], i % 2 == 0);
                GUILayout.Space(2);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawDialogueItem(DialogueGraph graph, bool isEven)
        {
            float rowHeight = 50f;
            bool isStart = graph.name == StartDialogueName;

            Rect rect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(rowHeight));

            if (isStart)
                EditorGUI.DrawRect(rect, new Color(1f, 0.8f, 0f, 0.1f));
            else if (isEven)
                EditorGUI.DrawRect(rect, new Color(1f, 1f, 1f, 0.03f));

            GUILayout.Space(5);

            EditorGUILayout.BeginVertical(GUILayout.Width(40), GUILayout.Height(rowHeight));
            GUILayout.FlexibleSpace();
            Texture icon = AssetPreview.GetMiniThumbnail(graph);
            Rect iconRect = GUILayoutUtility.GetRect(36, 36);
            if (icon) GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            if (isStart)
            {
                Rect starRect = new Rect(iconRect.xMax - 12, iconRect.yMax - 12, 14, 14);
                GUI.Label(starRect, EditorGUIUtility.IconContent("Favorite"));
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUILayout.Height(rowHeight));
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            float nameWidth = EditorStyles.boldLabel.CalcSize(new GUIContent(graph.name)).x;
            EditorGUILayout.LabelField(graph.name, EditorStyles.boldLabel, GUILayout.Width(nameWidth + 4));

            if (isStart)
            {
                GUIStyle entryStyle = new GUIStyle(EditorStyles.miniLabel);
                entryStyle.normal.textColor = new Color(1f, 0.7f, 0f);
                entryStyle.alignment = TextAnchor.MiddleLeft;
                EditorGUILayout.LabelField("[ENTRY POINT]", entryStyle, GUILayout.Height(18));
            }
            EditorGUILayout.EndHorizontal();

            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            EditorGUILayout.LabelField("Resources/Dialogues", pathStyle);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical(GUILayout.Height(rowHeight));
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();

            if (Application.isPlaying)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Play", GUILayout.Width(45), GUILayout.Height(22))) PlayDialogue(graph);
                GUI.backgroundColor = Color.white;
                GUILayout.Space(4);
            }

            if (GUILayout.Button("Open", GUILayout.Width(50), GUILayout.Height(22))) OpenGraphInEditor(graph);
            GUILayout.Space(4);
            if (GUILayout.Button("Rename", GUILayout.Width(65), GUILayout.Height(22)))
            {
                Event.current.Use();
                DialogueRenameWindow.OpenWindow(graph);
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(4);
            if (GUILayout.Button("Duplicate", GUILayout.Width(67), GUILayout.Height(22)))
            {
                Event.current.Use();
                DialogueDuplicateWindow.Open(graph);
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(4);

            GUI.backgroundColor = new Color(1f, 0.35f, 0.35f);
            if (GUILayout.Button("Delete", GUILayout.Width(60), GUILayout.Height(22)))
            {
                if (EditorUtility.DisplayDialog("Delete", $"Delete {graph.name}?", "Yes", "No"))
                    DeleteDialogue(graph);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }

        private void CreateNewDialogue()
        {
            if (string.IsNullOrWhiteSpace(_newDialogueName)) return;
            DialogueCreatorEditor.CreateNewDialogueAssetFromName(_newDialogueName);
            AssetDatabase.Refresh();
        }

        private void DeleteDialogue(NodeGraph graph)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(graph));
            AssetDatabase.Refresh();
        }

        private void OpenGraphInEditor(NodeGraph graph)
        {
            Selection.activeObject = graph;
            DialogueCreatorEditor.OpenGraph(graph);
        }

        private void PlayDialogue(DialogueGraph graph)
        {
            if (!Application.isPlaying) return;
            if (_dialogueService is null) _dialogueService = NovelGame.Instance.GetService<DialogueService>();
            NovelGame.Instance.ResetStateServices();
            NovelGame.Instance.GetService<MainMenuService>().Hide();
            _dialogueService.StopCurrentDialogue();
            _dialogueService.JumpToDialogue(graph);
        }
    }
}