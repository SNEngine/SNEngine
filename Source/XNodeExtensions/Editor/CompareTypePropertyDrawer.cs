using SiphoinUnityHelpers.XNodeExtensions.Math.Compare;
using UnityEditor;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomPropertyDrawer(typeof(CompareType))]
    public class CompareTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CompareType compareType = (CompareType)property.enumValueIndex;

            string[] enumNames = new string[]
            {
            "==",
            "!=",
            ">",
            "<",
            "<=",
            ">="
            };

            compareType = (CompareType)EditorGUI.Popup(position, label.text, (int)compareType, enumNames);

            property.enumValueIndex = (int)compareType;
        }
    }
}