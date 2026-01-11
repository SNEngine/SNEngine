using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Concurrent;

namespace SNEngine.Editor
{
    public partial class NovelCounterWindow : EditorWindow
    {
        private string _reportName = "novel_report.json";
        private string _scanPath = "";
        private StringBuilder _logAccumulator = new StringBuilder();
        private Vector2 _scrollPos;
        private ConcurrentQueue<string> _incomingLines = new ConcurrentQueue<string>();

        [MenuItem("SNEngine/Analytics/Novel Counter")]
        public static void ShowWindow() => GetWindow<NovelCounterWindow>("Novel Counter");

        private void OnEnable()
        {
            string defaultPath = Path.GetDirectoryName(Application.dataPath);
            _scanPath = EditorPrefs.GetString("SNEngine_NovelScanPath", defaultPath);
        }

        private void Update()
        {
            if (!_incomingLines.IsEmpty)
            {
                while (_incomingLines.TryDequeue(out string line))
                {
                    _logAccumulator.AppendLine(line);
                }
                _scrollPos.y = float.MaxValue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("SNEngine Novel Analytics", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            _scanPath = EditorGUILayout.TextField("Scan Path", _scanPath);
            EditorGUIUtility.labelWidth = originalLabelWidth;

            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder to Scan", _scanPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _scanPath = selectedPath;
                    EditorPrefs.SetString("SNEngine_NovelScanPath", _scanPath);
                }
            }
            EditorGUILayout.EndHorizontal();

            _reportName = EditorGUILayout.TextField("Report Name", _reportName);

            EditorGUILayout.Space();

            if (GUILayout.Button("Analyze Assets", GUILayout.Height(30)))
            {
                if (!Directory.Exists(_scanPath))
                {
                    EditorUtility.DisplayDialog("Error", "Path does not exist!", "OK");
                    return;
                }

                _logAccumulator.Clear();
                while (_incomingLines.TryDequeue(out _)) ;

                string args = $"\"{_scanPath}\" --json \"{_reportName}\"";
                NovelCounterLauncher.Run(args, EnqueueLogLine);
            }

            EditorGUILayout.Space();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true));
            EditorGUILayout.SelectableLabel(_logAccumulator.ToString(), EditorStyles.textArea, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Clear Log", GUILayout.Width(100)))
            {
                _logAccumulator.Clear();
            }
        }

        private void EnqueueLogLine(string line) => _incomingLines.Enqueue(line);
    }
}