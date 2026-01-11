using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEditor;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ErrorNodeFieldAttribute))]
    public class ErrorNodeFieldPropertyDrawer : NodeFieldPropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIField(position, property, label, "_exception");
        }
    }
}
