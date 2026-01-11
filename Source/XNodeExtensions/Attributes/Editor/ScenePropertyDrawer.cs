using SiphoinUnityHelpers.Attributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SiphoinUnityHelpers.Editor
{
    [CustomPropertyDrawer(typeof(SceneFieldAttribute))]
    public class ScenePropertyDrawer : PropertyDrawer
    {
        private List<string> _scenesNames = new List<string>();
        private int _selectedIndex = -1;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
                var scenes = EditorBuildSettings.scenes;

                _scenesNames.Clear();

                foreach (var scene in scenes)
                {
                string selectedScenePath = scene.path;

                string[] pathParts = selectedScenePath.Split('/');

                string sceneNameWithExtension = pathParts[pathParts.Length - 1];

                string sceneName = sceneNameWithExtension.Replace(".unity", string.Empty);

                _scenesNames.Add(sceneName);
                }

                string selectedSceneName = property.stringValue;

                _selectedIndex = _scenesNames.IndexOf(selectedSceneName);

                _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, _scenesNames.ToArray());

                if (_selectedIndex != -1)
                {
               property.stringValue = _scenesNames[_selectedIndex];

                }
            
        }

        private void GetShortSceneName ()
        {

        }
    }

}
