using UnityEditor;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public abstract class NodeFieldPropertyDrawer : PropertyDrawer
    {
        protected void OnGUIField(Rect position, SerializedProperty property, GUIContent label, string fieldName)
        {
            var node = property.serializedObject.targetObject as Node;

            bool wasEnabled = GUI.enabled;

            GUI.enabled = node.GetPort(fieldName).ConnectionCount > 0 ? false : true;

            EditorGUI.PropertyField(position, property, label);

            GUI.enabled = wasEnabled;
        }
    }
}
