using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(StringStringDictionaryVariableNode))]
    public class StringStringDictionaryVariableEditor : NodeEditor
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

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Item {i}", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("x", GUILayout.Width(20))) indexToRemove = i;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(element.FindPropertyRelative("_key"), new GUIContent("Key"));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("_value"), new GUIContent("Value"));

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("+ Add Pair")) shouldAdd = true;

            if (indexToRemove != -1) list.DeleteArrayElementAtIndex(indexToRemove);
            if (shouldAdd)
            {
                int index = list.arraySize++;
                var el = list.GetArrayElementAtIndex(index);
                el.FindPropertyRelative("_key").stringValue = "";
                el.FindPropertyRelative("_value").stringValue = "";
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