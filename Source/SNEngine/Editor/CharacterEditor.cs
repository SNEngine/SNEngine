using SNEngine.CharacterSystem;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
namespace SNEngine.Editor
{
    [CustomEditor(typeof(Character))]
    public class CharacterEditor : UnityEditor.Editor
    {
        private const int PREVIEW_SIZE = 450;
        private const int NAME_HEIGHT = 24;
        private const string GRID_CACHE_PATH = "Assets/SNEngine/Source/SNEngine/Editor/Cache/GridTexture.asset";
        private int _selectedEmotionIndex = 0;
        private Texture2D _gridTexture;

        private void OnEnable()
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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorName"));

            EditorGUILayout.Space();

            SerializedProperty emotionsProp = serializedObject.FindProperty("_emotions");
            EditorGUILayout.PropertyField(emotionsProp, true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Character Preview", EditorStyles.boldLabel);

            if (emotionsProp.arraySize > 0)
            {
                string[] emotionNames = new string[emotionsProp.arraySize];
                for (int i = 0; i < emotionsProp.arraySize; i++)
                {
                    SerializedProperty element = emotionsProp.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = element.FindPropertyRelative("_name");
                    emotionNames[i] = nameProp.stringValue;
                }
                _selectedEmotionIndex = EditorGUILayout.Popup("Select Emotion", _selectedEmotionIndex, emotionNames, GUILayout.ExpandWidth(true));

                if (_selectedEmotionIndex >= emotionsProp.arraySize)
                {
                    _selectedEmotionIndex = 0;
                }

                SerializedProperty emotionElement = emotionsProp.GetArrayElementAtIndex(_selectedEmotionIndex);
                SerializedProperty spriteProp = emotionElement.FindPropertyRelative("_sprite");

                Character characterTarget = (Character)target;

                Rect previewAreaRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE, GUILayout.ExpandWidth(true));

                if (_gridTexture != null)
                {
                    GUI.DrawTextureWithTexCoords(previewAreaRect, _gridTexture, new Rect(0, 0, previewAreaRect.width / _gridTexture.width, previewAreaRect.height / _gridTexture.height));
                }
                else
                {
                    EditorGUI.DrawRect(previewAreaRect, new Color(0.15f, 0.15f, 0.15f, 1f));
                }

                string hexColor = ColorUtility.ToHtmlStringRGB(characterTarget.ColorName);
                string coloredName = $"<color=#{hexColor}>{characterTarget.OriginalName}</color>";

                GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    richText = true,
                    fontSize = 18,
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

                        // 1. Вычисляем соотношение сторон (Aspect Ratio)
                        float spriteRatio = r.width / r.height;

                        // 2. Вычисляем размеры, сохраняющие пропорции, внутри доступной области
                        float finalWidth = availableSpriteRect.height * spriteRatio;
                        float finalHeight = availableSpriteRect.height;

                        if (finalWidth > availableSpriteRect.width)
                        {
                            finalWidth = availableSpriteRect.width;
                            finalHeight = finalWidth / spriteRatio;
                        }

                        // 3. Центрируем спрайт
                        Rect centeredSpriteRect = new Rect(
                            availableSpriteRect.x + (availableSpriteRect.width - finalWidth) / 2,
                            availableSpriteRect.y + (availableSpriteRect.height - finalHeight) / 2,
                            finalWidth,
                            finalHeight
                        );

                        // 4. Отрисовываем
                        GUI.DrawTextureWithTexCoords(centeredSpriteRect, texture, uv);
                    }
                }
                else
                {
                    Rect nullSpriteRect = new Rect(previewAreaRect.x, previewAreaRect.y, previewAreaRect.width, previewAreaRect.height - NAME_HEIGHT);
                    GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.gray } };
                    EditorGUI.LabelField(nullSpriteRect, "No Sprite", labelStyle);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Character has no emotions. Add at least one.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}