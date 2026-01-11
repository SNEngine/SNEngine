using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace SNEngine.Editor
{
    public class WelcomeWindow : EditorWindow
    {
        private static readonly string[] DEFINE_SYMBOLS =
        {
            "DOTWEEN",
            "UNITASK_DOTWEEN_SUPPORT",
            "SNENGINE_SUPPORT"
        };

        private static readonly string[] ENGINE_SCENES =
        {
            "Assets/SNEngine/Source/SNEngine/Scenes/Splash.unity",
            "Assets/SNEngine/Source/SNEngine/Scenes/Main.unity"
        };

        private static readonly string WINDOW_PREF_KEY = "SNEngine.WelcomeWindow.ShowOnStartup";

        private Vector2 scrollPosition;
        private GUIStyle titleStyle;
        private GUIStyle descriptionStyle;
        private GUIStyle buttonStyle;
        private GUIStyle headerStyle;
        private bool stylesInitialized;

        [MenuItem("SNEngine/Welcome Window", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<WelcomeWindow>("SNEngine Welcome");
            window.minSize = new Vector2(500, 550);
            window.titleContent = new GUIContent("SNEngine Welcome");
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(10, 10, 10, 10)
            };

            descriptionStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(10, 10, 5, 10)
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(10, 10, 10, 5),
                fixedHeight = 40
            };

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                margin = new RectOffset(10, 10, 10, 5)
            };

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            InitializeStyles();

            GUILayout.Space(20);
            GUILayout.Label("Welcome to SNEngine!", titleStyle);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(10);
            GUILayout.Label("Current Status:", headerStyle);

            var buildTarget = NamedBuildTarget.Standalone;
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);

            foreach (string symbol in DEFINE_SYMBOLS)
            {
                bool exists = currentDefines.Contains(symbol);
                GUI.color = exists ? Color.green : Color.yellow;
                GUILayout.Label($"{(exists ? "✓" : "+")} {symbol}", descriptionStyle);
            }

            GUI.color = PlayerSettings.SplashScreen.show ? Color.yellow : Color.green;
            GUILayout.Label($"{(PlayerSettings.SplashScreen.show ? "+" : "✓")} Splash Screen Disabled", descriptionStyle);

            GUI.color = Color.white;

            bool tmpAvailable = IsTextMeshProAvailable();
            GUI.color = tmpAvailable ? Color.green : Color.yellow;
            GUILayout.Label($"{(tmpAvailable ? "✓" : "+")} TextMesh Pro", descriptionStyle);
            GUI.color = Color.white;

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Setup SNEngine", buttonStyle))
            {
                PerformFullSetup();
            }

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            bool showOnStartup = !EditorPrefs.GetBool(WINDOW_PREF_KEY, false);
            bool toggleValue = EditorGUILayout.ToggleLeft("Show this window on startup", showOnStartup);
            if (toggleValue != showOnStartup)
                EditorPrefs.SetBool(WINDOW_PREF_KEY, !toggleValue);
            GUILayout.EndHorizontal();
        }

        private void PerformFullSetup()
        {
            AddDefineSymbols();
            DisableUnitySplash();
            SetupSceneList();

            ExecutionOrderManager.SetExecutionOrder();
            AutoIconAssigner.Assign();
            InstallTextMeshProEssentials();

            EditorUtility.DisplayDialog(
                "Setup Complete",
                "SNEngine configuration finished",
                "OK"
            );
        }

        private void DisableUnitySplash()
        {
            PlayerSettings.SplashScreen.show = false;
        }

        private void SetupSceneList()
        {
            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

            foreach (string scenePath in ENGINE_SCENES)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
                {
                    buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                }
            }

            EditorBuildSettings.scenes = buildScenes.ToArray();
        }

        private void AddDefineSymbols()
        {
            var buildTarget = NamedBuildTarget.Standalone;
            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);

            HashSet<string> defineSet = new HashSet<string>(
                currentDefines
                    .Split(';')
                    .Select(d => d.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
            );

            foreach (string symbol in DEFINE_SYMBOLS)
                defineSet.Add(symbol);

            PlayerSettings.SetScriptingDefineSymbols(buildTarget, string.Join(";", defineSet));
        }

        private void InstallTextMeshProEssentials()
        {
            EditorApplication.ExecuteMenuItem(
                "Window/TextMeshPro/Import TMP Essential Resources"
            );
        }

        private bool IsTextMeshProAvailable()
        {
            return Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro") != null ||
                   Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro") != null;
        }
    }
}