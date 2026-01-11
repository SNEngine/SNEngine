using SiphoinUnityHelpers.XNodeExtensions.Math.Compare;
using UnityEditor;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomPropertyDrawer(typeof(CompareStringsType))]
    public class CompareStringsTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CompareStringsType compareType = (CompareStringsType)property.enumValueIndex;

            string[] enumNames = new string[]
            {
            "==",
            "!=",
            ">",
            "<",
            "<=",
            ">="
            };

            compareType = (CompareStringsType)EditorGUI.Popup(position, label.text, (int)compareType, enumNames);

            property.enumValueIndex = (int)compareType;
        }
    }
}