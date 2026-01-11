using UnityEditor;
using UnityEngine;
using DG.Tweening;
using SNEngine;
using System.Linq;

namespace SNEngine.Editor
{
    public class SNEngineSettingsWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private SNEngineRuntimeSettings _runtimeSettingsAsset;
        private SerializedObject _runtimeSettingsSO;
        private UnityEditor.Editor _runtimeSettingsEditor;

        [MenuItem("SNEngine/Settings")]
        public static void ShowWindow()
        {
            SNEngineSettingsWindow window = GetWindow<SNEngineSettingsWindow>("SNEngine Settings");
            window.minSize = new Vector2(350, 250);
        }

        private void OnEnable()
        {
            FindRuntimeSettings();
        }

        private void OnSelectionChange()
        {
            if (_runtimeSettingsAsset == null)
            {
                FindRuntimeSettings();
                Repaint();
            }
        }

        private void FindRuntimeSettings()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(SNEngineRuntimeSettings)} {nameof(SNEngineRuntimeSettings)}");

            _runtimeSettingsAsset = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<SNEngineRuntimeSettings>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(s => s != null);

            if (_runtimeSettingsAsset != null)
            {
                _runtimeSettingsSO = new SerializedObject(_runtimeSettingsAsset);
                _runtimeSettingsEditor = UnityEditor.Editor.CreateEditor(_runtimeSettingsAsset);
            }
            else
            {
                _runtimeSettingsSO = null;
                _runtimeSettingsEditor = null;
            }
        }

        public void OnGUI()
        {
            GUILayout.Label("SNEngine Global Editor Settings", new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 10, 10)
            });

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawRuntimeSettings();

            EditorGUILayout.EndScrollView();


            Rect footerRect = new Rect(0, position.height - 20, position.width, 20);
            EditorGUI.LabelField(footerRect, "Version: 1.0", EditorStyles.centeredGreyMiniLabel);
        }


        private void DrawRuntimeSettings()
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Character Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (_runtimeSettingsEditor != null && _runtimeSettingsSO != null)
            {
                _runtimeSettingsSO.Update();

                SerializedProperty showSplashProp = _runtimeSettingsSO.FindProperty(nameof(SNEngineRuntimeSettings.ShowVideoSplash));
                EditorGUILayout.PropertyField(showSplashProp, new GUIContent("Show Video Splash"));

                EditorGUILayout.Space(5);

                SerializedProperty enableCrossfadeProp = _runtimeSettingsSO.FindProperty(nameof(SNEngineRuntimeSettings.EnableCrossfade));
                SerializedProperty crossfadeDurationProp = _runtimeSettingsSO.FindProperty(nameof(SNEngineRuntimeSettings.CrossfadeDuration));
                SerializedProperty crossfadeEaseProp = _runtimeSettingsSO.FindProperty(nameof(SNEngineRuntimeSettings.CrossfadeEase));

                EditorGUILayout.PropertyField(enableCrossfadeProp, new GUIContent("Enable Crossfade"));

                using (new EditorGUI.DisabledScope(!enableCrossfadeProp.boolValue))
                {
                    EditorGUILayout.PropertyField(crossfadeDurationProp, new GUIContent("Crossfade Duration"));
                    EditorGUILayout.PropertyField(crossfadeEaseProp, new GUIContent("Crossfade Ease"));
                }

                _runtimeSettingsSO.ApplyModifiedProperties();

                if (GUILayout.Button("Ping Settings Asset"))
                {
                    Selection.activeObject = _runtimeSettingsAsset;
                    EditorGUIUtility.PingObject(_runtimeSettingsAsset);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("SNEngineRuntimeSettings.asset not found. Please create it.", MessageType.Warning);

                if (GUILayout.Button("Create SNEngine Runtime Settings Asset"))
                {
                    if (EditorUtility.DisplayDialog("Settings Not Found",
                                                    "SNEngineRuntimeSettings.asset not found. Create it in 'Assets/Resources'?",
                                                    "Create", "Cancel"))
                    {
                        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                        {
                            AssetDatabase.CreateFolder("Assets", "Resources");
                        }

                        var newSettings = ScriptableObject.CreateInstance<SNEngineRuntimeSettings>();
                        AssetDatabase.CreateAsset(newSettings, "Assets/Resources/SNEngineRuntimeSettings.asset");
                        AssetDatabase.SaveAssets();

                        FindRuntimeSettings();
                        if (_runtimeSettingsAsset != null)
                        {
                            Selection.activeObject = _runtimeSettingsAsset;
                            EditorGUIUtility.PingObject(_runtimeSettingsAsset);
                        }
                    }
                }
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }
    }
}