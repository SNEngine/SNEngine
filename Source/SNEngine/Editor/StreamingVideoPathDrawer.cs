using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SNEngine.VideoPlayerSystem
{
    [CustomPropertyDrawer(typeof(StreamingVideoPathAttribute))]
    public class StreamingVideoPathDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [StreamingVideoPath] with strings.");
                return;
            }

            StreamingVideoPathAttribute attr = attribute as StreamingVideoPathAttribute;
            bool hideLabel = attr != null && attr.HideLabel;

            GUIContent labelToDraw = hideLabel ? GUIContent.none : label;

            string streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
            {
                EditorGUI.LabelField(position, labelToDraw, new GUIContent("StreamingAssets missing"));
                return;
            }

            string[] extensions = { "*.mp4", "*.webm", "*.mov", "*.avi" };
            List<string> filePaths = new List<string>();

            foreach (string ext in extensions)
            {
                filePaths.AddRange(Directory.GetFiles(streamingAssetsPath, ext, SearchOption.AllDirectories));
            }

            string[] options = filePaths.Select(fullPath =>
            {
                string relative = fullPath.Replace(streamingAssetsPath, "").Replace("\\", "/");
                if (relative.StartsWith("/")) relative = relative.Substring(1);
                return relative;
            }).ToArray();

            if (options.Length == 0)
            {
                EditorGUI.LabelField(position, labelToDraw, new GUIContent("No videos found"));
                return;
            }

            int currentIndex = -1;
            string currentPath = property.stringValue;

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == currentPath)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1 && !string.IsNullOrEmpty(currentPath))
            {
                currentIndex = 0;
            }

            int newIndex = EditorGUI.Popup(position, labelToDraw.text, currentIndex, options, EditorStyles.popup);

            if (newIndex >= 0 && newIndex < options.Length)
            {
                property.stringValue = options[newIndex];
            }
        }
    }
}