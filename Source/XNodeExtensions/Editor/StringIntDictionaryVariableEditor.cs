using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(StringIntDictionaryVariableNode))]
    public class StringIntDictionaryVariableEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_name"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_color"));

            EditorGUILayout.Space(10);

            SerializedProperty list = serializedObject.FindProperty("_serializedItems");

            int indexToRemove = -1;
            bool shouldAdd = false;

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = element.FindPropertyRelative("_key");
                SerializedProperty valueProp = element.FindPropertyRelative("_value");

                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Item {i}", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    indexToRemove = i;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(keyProp, new GUIContent("Key (String)"));
                EditorGUILayout.PropertyField(valueProp, new GUIContent("Value (Int)"));

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("+ Add Pair"))
            {
                shouldAdd = true;
            }

            if (indexToRemove != -1)
            {
                list.DeleteArrayElementAtIndex(indexToRemove);
            }

            if (shouldAdd)
            {
                int index = list.arraySize;
                list.arraySize++;
                var newElement = list.GetArrayElementAtIndex(index);

                var kProp = newElement.FindPropertyRelative("_key");
                var vProp = newElement.FindPropertyRelative("_value");

                if (kProp != null) kProp.stringValue = string.Empty;
                if (vProp != null) vProp.intValue = 0;
            }

            EditorGUILayout.Space(10);

            NodePort port = target.GetPort("_output");
            if (port != null)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Output"), port);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}