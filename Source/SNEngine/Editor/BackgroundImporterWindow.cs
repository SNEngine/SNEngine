using System.IO;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor
{
    public class BackgroundImporterWindow : EditorWindow
    {
        private const string BACKGROUND_SPRITE_FOLDER = "Assets/SNEngine/Source/SNEngine/Sprites/Backgrounds";
        private const int PREVIEW_HEIGHT_OFFSET = 30;

        private string _selectedFilePath = string.Empty;
        private Sprite _previewSprite;

        [MenuItem("SNEngine/Background Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<BackgroundImporterWindow>("Background Importer");
            window.minSize = new Vector2(500, 300);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width * 0.4f), GUILayout.ExpandHeight(true));
            DrawConfigurationPanel();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
            DrawPreviewPanel();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawConfigurationPanel()
        {
            EditorGUILayout.LabelField("1. Select Image File", EditorStyles.boldLabel);

            // Button to open file dialog
            if (GUILayout.Button("Select Image File (.png, .jpg, .tga)"))
            {
                string path = EditorUtility.OpenFilePanel("Select Background Image", "", "png,jpg,jpeg,tga");
                if (!string.IsNullOrEmpty(path))
                {
                    _selectedFilePath = path;
                    LoadTemporaryPreviewSprite(_selectedFilePath);
                }
            }

            EditorGUILayout.Space(20);

            // Import button
            GUI.enabled = !string.IsNullOrEmpty(_selectedFilePath);
            if (GUILayout.Button("Import"))
            {
                ImportAndSaveFile();
            }
            GUI.enabled = true;
        }

        private void LoadTemporaryPreviewSprite(string path)
        {
            if (_previewSprite != null)
            {
                DestroyImmediate(_previewSprite);
            }

            if (!File.Exists(path)) return;

            byte[] fileData = File.ReadAllBytes(path);

            Texture2D tempTexture = new Texture2D(2, 2);
            if (tempTexture.LoadImage(fileData))
            {
                _previewSprite = Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), Vector2.one * 0.5f, 100f);
            }
            else
            {
                DestroyImmediate(tempTexture);
            }
            Repaint();
        }

        private void DrawPreviewPanel()
        {
            EditorGUILayout.LabelField("Sprite Preview", EditorStyles.boldLabel);
            Rect previewAreaRect = GUILayoutUtility.GetRect(0f, 1000f, 0f, 1000f);

            Sprite spriteToDraw = _previewSprite;

            if (spriteToDraw != null)
            {
                Texture2D texture = spriteToDraw.texture;
                if (texture != null)
                {
                    Rect r = spriteToDraw.rect;

                    Rect uv = new Rect(r.x / texture.width, r.y / texture.height, r.width / texture.width, r.height / texture.height);

                    Rect availableSpriteRect = new Rect(
                        previewAreaRect.x,
                        previewAreaRect.y,
                        previewAreaRect.width,
                        previewAreaRect.height - PREVIEW_HEIGHT_OFFSET
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
                Rect nullSpriteRect = new Rect(previewAreaRect.x, previewAreaRect.y, previewAreaRect.width, previewAreaRect.height - PREVIEW_HEIGHT_OFFSET);
                GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.gray } };
                EditorGUI.LabelField(nullSpriteRect, "No Sprite Selected", labelStyle);
            }

            GUILayout.FlexibleSpace();
        }

        private void ImportAndSaveFile()
        {
            if (string.IsNullOrEmpty(_selectedFilePath)) return;

            string fileName = Path.GetFileName(_selectedFilePath);
            string destinationPath = Path.Combine(BACKGROUND_SPRITE_FOLDER, fileName).Replace('\\', '/');

            if (!AssetDatabase.IsValidFolder(BACKGROUND_SPRITE_FOLDER))
            {
                string[] parts = BACKGROUND_SPRITE_FOLDER.Split('/');
                string current = "Assets";
                for (int i = 1; i < parts.Length; i++)
                {
                    string folderPath = Path.Combine(current, parts[i]).Replace('\\', '/');
                    if (!AssetDatabase.IsValidFolder(folderPath))
                    {
                        AssetDatabase.CreateFolder(current, parts[i]);
                    }
                    current = folderPath;
                }
                AssetDatabase.Refresh();
            }

            if (File.Exists(destinationPath))
            {
                if (!EditorUtility.DisplayDialog("Confirm Overwrite",
                    $"A file named '{fileName}' already exists in the target folder. Overwrite it?",
                    "Overwrite", "Cancel"))
                {
                    return;
                }
            }

            try
            {
                File.Copy(_selectedFilePath, destinationPath, true);

                AssetDatabase.ImportAsset(destinationPath);

                TextureImporter importer = AssetImporter.GetAtPath(destinationPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.SaveAndReimport();
                }


                _selectedFilePath = string.Empty;
                if (_previewSprite != null)
                {
                    DestroyImmediate(_previewSprite);
                }
                Repaint();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Import Error", $"Failed to import file. Error: {e.Message}", "OK");
                Debug.LogError($"Import failed: {e.Message}");
            }
        }
    }
}