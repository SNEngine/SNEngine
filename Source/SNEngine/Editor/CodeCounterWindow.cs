using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Concurrent;

namespace SNEngine.Editor
{
    public partial class CodeCounterWindow : EditorWindow
    {
        private string _scanPath = "";
        private bool _generateReport = true;
        private StringBuilder _logAccumulator = new StringBuilder();
        private Vector2 _scrollPos;
        private ConcurrentQueue<string> _incomingLines = new ConcurrentQueue<string>();

        [MenuItem("SNEngine/Analytics/Code Counter")]
        public static void ShowWindow() => GetWindow<CodeCounterWindow>("Code Counter");

        private void OnEnable()
        {
            string defaultPath = Path.GetDirectoryName(Application.dataPath);
            _scanPath = EditorPrefs.GetString("SNEngine_CodeScanPath", defaultPath);
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
            EditorGUILayout.LabelField("SNEngine Code Analytics", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            _scanPath = EditorGUILayout.TextField("Scan Path", _scanPath);
            EditorGUIUtility.labelWidth = originalLabelWidth;

            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Source Folder", _scanPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _scanPath = selectedPath;
                    EditorPrefs.SetString("SNEngine_CodeScanPath", _scanPath);
                }
            }
            EditorGUILayout.EndHorizontal();

            _generateReport = EditorGUILayout.Toggle("Generate JSON Report", _generateReport);

            EditorGUILayout.Space();

            if (GUILayout.Button("Count Lines of Code", GUILayout.Height(30)))
            {
                if (!Directory.Exists(_scanPath))
                {
                    EditorUtility.DisplayDialog("Error", "Path does not exist!", "OK");
                    return;
                }

                _logAccumulator.Clear();
                while (_incomingLines.TryDequeue(out _)) ;

                string args = $"\"{_scanPath}\"";
                if (_generateReport) args += " --report";

                CodeCounterLauncher.Run(args, EnqueueLogLine);
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