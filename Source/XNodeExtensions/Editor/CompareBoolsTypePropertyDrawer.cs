using SiphoinUnityHelpers.XNodeExtensions.Math.Compare;
using UnityEditor;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensionss.Editor
{
    [CustomPropertyDrawer(typeof(CompareBoolsType))]
    public class CompareTypeBoolsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CompareBoolsType compareType = (CompareBoolsType)property.enumValueIndex;

            string[] enumNames = new string[]
            {
            "==",
            "!="
            };

            compareType = (CompareBoolsType)EditorGUI.Popup(position, label.text, (int)compareType, enumNames);

            property.enumValueIndex = (int)compareType;
        }
    }
}
