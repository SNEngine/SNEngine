using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ReadOnlyAttribute readOnlyAttribute = attribute as ReadOnlyAttribute;

        bool wasEnabled = GUI.enabled;

        switch (readOnlyAttribute.Mode)
        {
            case ReadOnlyMode.Always:
                GUI.enabled = false;
                break;
            case ReadOnlyMode.OnPlayMode:
                GUI.enabled = !Application.isPlaying;
                break;
            case ReadOnlyMode.OnEditor:
                GUI.enabled = !Application.isPlaying && Application.isEditor;
                break;
            default:
                break;
        }

        if (property.propertyType == SerializedPropertyType.ArraySize)
        {
            EditorGUI.LabelField(position, label.text, "Read Only Object Reference");
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }

        GUI.enabled = wasEnabled;
    }
}
