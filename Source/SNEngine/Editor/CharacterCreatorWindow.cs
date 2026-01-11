using SNEngine.CharacterSystem;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace SNEngine.Editor
{
    public class CharacterCreatorWindow : EditorWindow
    {
        private const int NAME_HEIGHT = 30;
        private const string GRID_CACHE_PATH = "Assets/SNEngine/Source/SNEngine/Editor/Cache/GridTexture.asset";
        private const string CHARACTER_SAVE_PATH = "Assets/SNEngine/Source/SNEngine/Resources/Characters";
        private const float PREVIEW_HEIGHT = 880f;

        private enum EditorMode { Creation, Editing, FolderImport }
        private EditorMode _selectedMode = EditorMode.Creation;

        private Character _character;
        private Character[] _characterAssets;
        private string[] _characterNames;
        private int _selectedCharacterIndex = 0;

        private string _characterName = "New Character";
        private string _description = string.Empty;
        private Color _colorName = Color.white;
        private Texture2D _gridTexture;
        private int _selectedEmotionIndex = 0;
        private bool _isDirty = false;
        private Vector2 _scrollPosition;

        private SerializedObject _serializedObject;
        private Character _editingCharacterAsset;
        private DefaultAsset _importFolder;

        [MenuItem("SNEngine/Character Creator")]
        public static void ShowWindow()
        {
            GetWindow<CharacterCreatorWindow>("Character Creator");
        }

        private void OnEnable()
        {
            LoadGridTexture();
            InitializeMode(EditorMode.Creation);
        }

        private void OnDisable()
        {
            if (_selectedMode == EditorMode.Editing && _isDirty && _character != null && _editingCharacterAsset != null)
            {
                if (EditorUtility.DisplayDialog("Unsaved Changes",
                   "You have unsaved changes in the current character. Do you want to save them?",
                   "Save", "Discard"))
                {
                    SaveCharacterAsset(true);
                }
            }

            if (_serializedObject != null)
            {
                _serializedObject.Dispose();
            }
            if (_character != null && (_selectedMode == EditorMode.Creation || _selectedMode == EditorMode.FolderImport) && AssetDatabase.GetAssetPath(_character) == string.Empty)
            {
                DestroyImmediate(_character);
            }
        }

        private void InitializeMode(EditorMode newMode)
        {
            if (_selectedMode == EditorMode.Editing && _isDirty && _character != null && newMode != EditorMode.Editing)
            {
                if (EditorUtility.DisplayDialog("Unsaved Changes",
                   "You have unsaved changes in the character being edited. Do you want to save them before switching modes?",
                   "Save", "Discard"))
                {
                    SaveCharacterAsset(true);
                }
            }

            if (_serializedObject != null)
            {
                _serializedObject.Dispose();
            }

            _selectedMode = newMode;
            _isDirty = false;
            _scrollPosition = Vector2.zero;

            if (_selectedMode == EditorMode.Creation || _selectedMode == EditorMode.FolderImport)
            {
                _characterName = "New Character";
                _description = string.Empty;
                _colorName = Color.white;
                _character = ScriptableObject.CreateInstance<Character>();
                _character.Editor_SetName(_characterName);
                _character.Editor_SetDescription(_description);
                _character.Editor_SetColorName(_colorName);
                InitializeSerializedObject();
            }
            else if (_selectedMode == EditorMode.Editing)
            {
                LoadCharacterAssets();
                if (_characterAssets.Any())
                {
                    _selectedCharacterIndex = 0;
                    _character = _characterAssets[_selectedCharacterIndex];
                    InitializeSerializedObject();
                }
                else
                {
                    _character = null;
                }
            }

            Repaint();
        }

        private void LoadCharacterAssets()
        {
            if (!Directory.Exists(CHARACTER_SAVE_PATH))
            {
                _characterNames = new string[] { "No Characters Found" };
                _characterAssets = new Character[0];
                _selectedCharacterIndex = 0;
                return;
            }

            string[] assetGuids = AssetDatabase.FindAssets("t:Character", new[] { CHARACTER_SAVE_PATH });

            var characters = assetGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Character>)
                .Where(c => c != null)
                .OrderBy(c => c.OriginalName)
                .ToList();

            _characterAssets = characters.ToArray();
            _characterNames = characters.Select(c => c.OriginalName).ToArray();

            if (_characterNames.Length == 0)
            {
                _characterNames = new string[] { "No Characters Found" };
                _characterAssets = new Character[0];
                _selectedCharacterIndex = 0;
            }
            else
            {
                if (_selectedCharacterIndex >= _characterNames.Length)
                {
                    _selectedCharacterIndex = 0;
                }
            }
        }

        private void InitializeSerializedObject()
        {
            if (_serializedObject != null)
            {
                _serializedObject.Dispose();
            }
            if (_character == null) return;

            _serializedObject = new SerializedObject(_character);

            _characterName = _character.OriginalName;
            _description = _character.Description;
            _colorName = _character.ColorName;

            if (_character.Emotions.Any())
            {
                _selectedEmotionIndex = 0;
            }
            else
            {
                _selectedEmotionIndex = -1;
            }
        }

        private void LoadGridTexture()
        {
            _gridTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(GRID_CACHE_PATH);

            if (_gridTexture == null)
            {
                _gridTexture = CreateGridTexture(16, new Color(0.15f, 0.15f, 0.15f, 1f), new Color(0.2f, 0.2f, 0.2f, 1f));

                string directory = Path.GetDirectoryName(GRID_CACHE_PATH);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                AssetDatabase.CreateAsset(_gridTexture, GRID_CACHE_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private Texture2D CreateGridTexture(int cellSize, Color lineColor, Color bgColor)
        {
            Texture2D texture = new Texture2D(cellSize, cellSize);
            Color[] pixels = new Color[cellSize * cellSize];

            for (int y = 0; y < cellSize; y++)
            {
                for (int x = 0; x < cellSize; x++)
                {
                    if (x == 0 || y == 0)
                    {
                        pixels[y * cellSize + x] = lineColor;
                    }
                    else
                    {
                        pixels[y * cellSize + x] = bgColor;
                    }
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Point;
            return texture;
        }

        public void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            EditorMode newMode = (EditorMode)GUILayout.Toolbar((int)_selectedMode, new string[] { "Create New", "Edit Existing", "Import Folder" }, GUILayout.ExpandWidth(true));

            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            if (newMode != _selectedMode)
            {
                InitializeMode(newMode);
            }

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width * 0.5f), GUILayout.ExpandHeight(true));

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            switch (_selectedMode)
            {
                case EditorMode.Creation:
                    DrawCreationPanel();
                    break;
                case EditorMode.Editing:
                    DrawEditingPanel();
                    break;
                case EditorMode.FolderImport:
                    DrawFolderImportPanel();
                    break;
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
            DrawPreviewPanel();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawCreationPanel()
        {
            EditorGUILayout.LabelField("Character Creation", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawConfigurationFields(true, true);

            EditorGUILayout.Space();
            if (_character != null && GUILayout.Button("Create and Save Character Asset"))
            {
                SaveCharacterAsset(false);
            }

            GUILayout.FlexibleSpace();
        }

        private void DrawEditingPanel()
        {
            EditorGUILayout.LabelField("Edit Existing Character", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_characterAssets == null || _characterAssets.Length == 0)
            {
                EditorGUILayout.HelpBox("No Character assets found in: " + CHARACTER_SAVE_PATH, MessageType.Info);
                if (GUILayout.Button("Reload Assets"))
                {
                    LoadCharacterAssets();
                    Repaint();
                }
                GUILayout.FlexibleSpace();
                return;
            }

            EditorGUI.BeginChangeCheck();

            _selectedCharacterIndex = EditorGUILayout.Popup("Select Character", _selectedCharacterIndex, _characterNames);

            if (EditorGUI.EndChangeCheck())
            {
                Character selectedAsset = _characterAssets[_selectedCharacterIndex];

                if (_character != selectedAsset)
                {
                    _character = selectedAsset;
                    InitializeSerializedObject();
                }
                _isDirty = false;
            }

            if (_character != null)
            {
                DrawConfigurationFields(false, true);

                EditorGUILayout.Space();
                if (_character != null && GUILayout.Button("Apply Changes to Asset"))
                {
                    SaveCharacterAsset(true);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select a Character Asset to begin editing.", MessageType.Info);
            }

            GUILayout.FlexibleSpace();
        }

        private void DrawFolderImportPanel()
        {
            EditorGUILayout.LabelField("Import Character from Folder", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawConfigurationFields(true, false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sprite Import Settings", EditorStyles.boldLabel);

            _importFolder = (DefaultAsset)EditorGUILayout.ObjectField("Sprites Folder", _importFolder, typeof(DefaultAsset), false);

            if (_importFolder != null)
            {
                string path = AssetDatabase.GetAssetPath(_importFolder);
                if (!Directory.Exists(path))
                {
                    EditorGUILayout.HelpBox("Selected object is not a valid folder.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Ready to import sprites from:\n{path}", MessageType.Info);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Generate and Save Character"))
                    {
                        GenerateCharacterFromFolder(path);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please select a folder containing sprite assets.", MessageType.Warning);
            }

            GUILayout.FlexibleSpace();
        }

        private void GenerateCharacterFromFolder(string folderPath)
        {
            if (_character == null || _serializedObject == null) return;

            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

            if (guids.Length == 0)
            {
                EditorUtility.DisplayDialog("No Sprites Found", "The selected folder does not contain any sprites.", "OK");
                return;
            }

            List<Sprite> sprites = new List<Sprite>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            sprites = sprites.OrderBy(s => s.name).ToList();

            _serializedObject.Update();
            SerializedProperty emotionsProp = _serializedObject.FindProperty("_emotions");
            emotionsProp.ClearArray();
            emotionsProp.arraySize = sprites.Count;

            for (int i = 0; i < sprites.Count; i++)
            {
                SerializedProperty element = emotionsProp.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = element.FindPropertyRelative("_name");
                SerializedProperty spriteProp = element.FindPropertyRelative("_sprite");

                nameProp.stringValue = sprites[i].name;
                spriteProp.objectReferenceValue = sprites[i];
            }

            _serializedObject.ApplyModifiedProperties();

            SaveCharacterAsset(false);
        }

        private void DrawConfigurationFields(bool isCreationMode, bool showEmotionsList)
        {
            if (_character == null || _serializedObject == null)
            {
                EditorGUILayout.HelpBox("Character data is not loaded.", MessageType.Error);
                return;
            }

            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            SerializedProperty nameProp = _serializedObject.FindProperty("_name");

            if (isCreationMode)
            {
                nameProp.stringValue = EditorGUILayout.TextField("Name", nameProp.stringValue);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Name (Asset Name)", nameProp.stringValue);
                EditorGUI.EndDisabledGroup();
            }

            SerializedProperty colorProp = _serializedObject.FindProperty("_colorName");
            colorProp.colorValue = EditorGUILayout.ColorField("Name Color", colorProp.colorValue);

            EditorGUILayout.LabelField("Description");
            SerializedProperty descProp = _serializedObject.FindProperty("_description");
            descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(100));

            if (showEmotionsList)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Emotions", EditorStyles.boldLabel);

                SerializedProperty emotionsProp = _serializedObject.FindProperty("_emotions");
                if (emotionsProp != null)
                {
                    EditorGUILayout.PropertyField(emotionsProp, true);
                }
                else
                {
                    EditorGUILayout.HelpBox("Emotion property not found.", MessageType.Error);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                _isDirty = true;
            }

            _serializedObject.ApplyModifiedProperties();

            _characterName = nameProp.stringValue;
            _colorName = colorProp.colorValue;
            _description = descProp.stringValue;
        }

        private void SaveCharacterAsset(bool isEditingExisting)
        {
            if (_character == null) return;

            if (isEditingExisting)
            {
                EditorUtility.SetDirty(_character);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                _isDirty = false;
                Debug.Log($"Character '{_character.OriginalName}' saved successfully.");
            }
            else
            {
                string defaultFileName = string.IsNullOrWhiteSpace(_characterName) ? "NewCharacter" : _characterName;

                if (!Directory.Exists(CHARACTER_SAVE_PATH))
                {
                    Directory.CreateDirectory(CHARACTER_SAVE_PATH);
                    AssetDatabase.Refresh();
                }

                string path = $"{CHARACTER_SAVE_PATH}/{defaultFileName}.asset";

                if (AssetDatabase.LoadAssetAtPath<Character>(path) != null)
                {
                    if (!EditorUtility.DisplayDialog("Confirm Overwrite",
                        $"An asset named '{defaultFileName}' already exists in the Characters folder. Do you want to overwrite it?",
                        "Overwrite", "Cancel"))
                    {
                        return;
                    }
                }

                _character.name = defaultFileName;

                AssetDatabase.CreateAsset(_character, path);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"New Character '{defaultFileName}' created successfully at {path}");

                if (_selectedMode == EditorMode.FolderImport)
                {
                    InitializeMode(EditorMode.FolderImport);
                }
                else
                {
                    InitializeMode(EditorMode.Creation);
                }
            }
        }

        private void DrawPreviewPanel()
        {
            EditorGUILayout.LabelField("Emotion Preview", EditorStyles.boldLabel);

            if (_character == null || _serializedObject == null)
            {
                EditorGUILayout.HelpBox(_selectedMode == EditorMode.Editing ? "Select a Character Asset to view its preview." : "Character data is not loaded.", MessageType.Info);
                GUILayout.FlexibleSpace();
                return;
            }

            SerializedProperty emotionsProp = _serializedObject.FindProperty("_emotions");
            if (emotionsProp == null || emotionsProp.arraySize == 0)
            {
                if (_selectedMode == EditorMode.FolderImport)
                {
                    EditorGUILayout.HelpBox("Select a folder and import to preview emotions.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Character has no emotions. Add at least one.", MessageType.Warning);
                }
                GUILayout.FlexibleSpace();
                return;
            }

            if (_selectedEmotionIndex >= emotionsProp.arraySize || _selectedEmotionIndex < 0)
            {
                _selectedEmotionIndex = 0;
            }

            if (emotionsProp.arraySize > 0)
            {
                string[] emotionNames = new string[emotionsProp.arraySize];
                for (int i = 0; i < emotionsProp.arraySize; i++)
                {
                    SerializedProperty element = emotionsProp.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = element.FindPropertyRelative("_name");
                    emotionNames[i] = nameProp.stringValue;
                }
                _selectedEmotionIndex = EditorGUILayout.Popup("Select Emotion Preview", _selectedEmotionIndex, emotionNames, GUILayout.ExpandWidth(true));

                EditorGUILayout.Space(5);
            }

            SerializedProperty emotionElement = emotionsProp.GetArrayElementAtIndex(_selectedEmotionIndex);
            SerializedProperty spriteProp = emotionElement.FindPropertyRelative("_sprite");

            Rect previewAreaRect = GUILayoutUtility.GetRect(0f, PREVIEW_HEIGHT, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));

            if (_gridTexture != null)
            {
                GUI.DrawTextureWithTexCoords(previewAreaRect, _gridTexture, new Rect(0, 0, previewAreaRect.width / _gridTexture.width, previewAreaRect.height / _gridTexture.height));
            }
            else
            {
                EditorGUI.DrawRect(previewAreaRect, new Color(0.15f, 0.15f, 0.15f, 1f));
            }

            string hexColor = ColorUtility.ToHtmlStringRGB(_colorName);
            string coloredName = $"<color=#{hexColor}>{_characterName}</color>";

            GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                richText = true,
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            Rect nameRect = new Rect(
                previewAreaRect.x,
                previewAreaRect.y + previewAreaRect.height - NAME_HEIGHT,
                previewAreaRect.width,
                NAME_HEIGHT
            );
            EditorGUI.LabelField(nameRect, coloredName, nameStyle);

            float padding = 8f;
            Rect availableSpriteRect = new Rect(
                previewAreaRect.x + padding,
                previewAreaRect.y + padding,
                previewAreaRect.width - 2 * padding,
                previewAreaRect.height - NAME_HEIGHT - 2 * padding
            );

            if (spriteProp.objectReferenceValue != null)
            {
                Sprite sprite = (Sprite)spriteProp.objectReferenceValue;
                Texture2D texture = sprite.texture;

                if (texture != null)
                {
                    Rect r = sprite.rect;
                    Rect uv = new Rect(
                        r.x / texture.width,
                        r.y / texture.height,
                        r.width / texture.width,
                        r.height / texture.height
                    );

                    float spriteRatio = r.width / r.height;

                    float finalWidth = availableSpriteRect.height * spriteRatio;
                    float finalHeight = availableSpriteRect.height;

                    if (finalWidth > availableSpriteRect.width)
                    {
                        finalWidth = availableSpriteRect.width;
                        finalHeight = finalWidth / spriteRatio;
                    }

                    Rect centeredSpriteRect = new Rect(
                        availableSpriteRect.x + (availableSpriteRect.width - finalWidth) / 2,
                        availableSpriteRect.y + (availableSpriteRect.height - finalHeight) / 2,
                        finalWidth,
                        finalHeight
                    );

                    GUI.DrawTextureWithTexCoords(centeredSpriteRect, texture, uv);
                }
            }
            else
            {
                Rect nullSpriteRect = new Rect(previewAreaRect.x, previewAreaRect.y, previewAreaRect.width, previewAreaRect.height - NAME_HEIGHT);
                GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.gray } };
                EditorGUI.LabelField(nullSpriteRect, "No Sprite", labelStyle);
            }

            GUILayout.FlexibleSpace();
        }

        public static void CreateBlankCharacter()
        {
            if (!Directory.Exists(CHARACTER_SAVE_PATH))
            {
                Directory.CreateDirectory(CHARACTER_SAVE_PATH);
            }

            Character character = ScriptableObject.CreateInstance<Character>();
            character.Editor_SetName("Blank Character");
            character.Editor_SetDescription(string.Empty);
            character.Editor_SetColorName(Color.white);

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{CHARACTER_SAVE_PATH}/Blank Character.asset");

            AssetDatabase.CreateAsset(character, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = character;
            EditorGUIUtility.PingObject(character);
        }
    }

}