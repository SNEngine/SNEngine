using SiphoinUnityHelpers.XNodeExtensions.Math.Compare;
using UnityEditor;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensionss.Editor
{
    [CustomPropertyDrawer(typeof(CompareObjectsType))]
    public class CompareTypeObjectsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CompareObjectsType compareType = (CompareObjectsType)property.enumValueIndex;

            string[] enumNames = new string[]
            {
            "==",
            "!="
            };

            compareType = (CompareObjectsType)EditorGUI.Popup(position, label.text, (int)compareType, enumNames);

            property.enumValueIndex = (int)compareType;
        }
    }
}
