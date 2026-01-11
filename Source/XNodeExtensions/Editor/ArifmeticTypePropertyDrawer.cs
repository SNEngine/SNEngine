using SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic;
using UnityEditor;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ArifmeticType))]
    public class FrifmeticTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ArifmeticType arifmeticType = (ArifmeticType)property.enumValueIndex;

            string[] enumNames = new string[]
            {
            "+",
            "-",
            "/",
            "*",
            "%"
            };

            arifmeticType = (ArifmeticType)EditorGUI.Popup(position, label.text, (int)arifmeticType, enumNames);

            property.enumValueIndex = (int)arifmeticType;
        }
    }
}