using System.IO;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILImportWindow : EditorWindow
    {
        private string _selectedFilePath = "";
        private string _graphName = "NewSNILGraph";
        private Vector2 _scrollPosition;

        [MenuItem("SNEngine/SNIL/SNIL Importer", priority = 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<SNILImportWindow>("SNIL Importer");
            window.minSize = new Vector2(400, 300);
        }

        private void OnGUI()
        {
            GUILayout.Label("SNIL Script Importer", EditorStyles.boldLabel);
            
            EditorGUILayout.Space(10);
            
            GUILayout.Label("Select SNIL Script File:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(_selectedFilePath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFilePanel(
                    "Select SNIL Script",
                    "",
                    "snil"
                );
                
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _selectedFilePath = selectedPath;
                    
                    // Auto-populate graph name from file name
                    _graphName = Path.GetFileNameWithoutExtension(selectedPath);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            GUILayout.Label("Graph Name:", EditorStyles.boldLabel);
            _graphName = EditorGUILayout.TextField(_graphName);
            
            EditorGUILayout.Space(10);
            
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_selectedFilePath));
            if (GUILayout.Button("Import SNIL Script", GUILayout.Height(30)))
            {
                ImportSNILScript();
            }
            EditorGUI.EndDisabledGroup();
            
            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                EditorGUILayout.Space(10);
                GUILayout.Label("File Preview:", EditorStyles.boldLabel);
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(150));
                
                try
                {
                    string[] lines = File.ReadAllLines(_selectedFilePath);
                    
                    for (int i = 0; i < Mathf.Min(lines.Length, 50); i++) // Limit preview to 50 lines
                    {
                        EditorGUILayout.TextField(lines[i]);
                    }
                    
                    if (lines.Length > 50)
                    {
                        EditorGUILayout.LabelField($"... and {lines.Length - 50} more lines");
                    }
                }
                catch (System.Exception e)
                {
                    EditorGUILayout.HelpBox($"Error reading file: {e.Message}", MessageType.Error);
                }
                
                EditorGUILayout.EndScrollView();
            }
        }

        private void ImportSNILScript()
        {
            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a SNIL script file.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_graphName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a graph name.", "OK");
                return;
            }

            // Call the SNIL compiler to import the script
            SNILCompiler.ImportScript(_selectedFilePath);
            
            // Refresh the asset database
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", $"SNIL script imported successfully as '{_graphName}'!", "OK");
        }
    }
}