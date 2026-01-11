using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes.Switch;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public abstract class BaseSwitchNodeEditor<T> : NodeEditor
    {
        private const float PortWidth = 24f;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var node = target as SwitchNode<T>;
            if (node == null) return;

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_enter"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_value"));

            EditorGUILayout.Space(6);
            DrawInlineCases(node);
            EditorGUILayout.Space(6);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_default"));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInlineCases(SwitchNode<T> node)
        {
            SerializedProperty casesProp = serializedObject.FindProperty("_cases");
            int indexToRemove = -1;

            for (int i = 0; i < casesProp.arraySize; i++)
            {
                SerializedProperty element = casesProp.GetArrayElementAtIndex(i);
                string portName = GetPortNameFromProperty(element);
                NodePort port = node.GetOutputPort(portName);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    indexToRemove = i;
                }

                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(element, GUIContent.none, true);
                EditorGUILayout.EndVertical();

                GUILayout.Space(PortWidth);

                EditorGUILayout.EndHorizontal();

                Rect rowRect = GUILayoutUtility.GetLastRect();

                if (port != null)
                {
                    Vector2 portPos = new Vector2(
                        rowRect.xMax - PortWidth * 0.5f,
                        rowRect.center.y - 8f
                    );

                    NodeEditorGUILayout.PortField(portPos, port);
                }

                EditorGUILayout.Space(2);
            }

            if (indexToRemove != -1)
            {
                casesProp.DeleteArrayElementAtIndex(indexToRemove);
                serializedObject.ApplyModifiedProperties();
                SyncPorts();
            }

            if (GUILayout.Button("Add Case", EditorStyles.miniButton))
            {
                int lastIndex = casesProp.arraySize;
                casesProp.InsertArrayElementAtIndex(lastIndex);

                SerializedProperty newElem = casesProp.GetArrayElementAtIndex(lastIndex);

                if (lastIndex > 0)
                {
                    SerializedProperty prevElem = casesProp.GetArrayElementAtIndex(lastIndex - 1);
                    AutoIncrementValue(newElem, prevElem);
                }
                else
                {
                    ResetToDefault(newElem);
                }

                serializedObject.ApplyModifiedProperties();
                SyncPorts();
            }
        }

        private void AutoIncrementValue(SerializedProperty next, SerializedProperty prev)
        {
            switch (prev.propertyType)
            {
                case SerializedPropertyType.Integer:
                    next.intValue = prev.intValue + 1;
                    break;
                case SerializedPropertyType.Float:
                    next.doubleValue = prev.doubleValue + 1.0;
                    break;
            }
        }

        private void ResetToDefault(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.Float:
                    prop.doubleValue = 0.0;
                    break;
            }
        }

        protected void SyncPorts()
        {
            var node = target as SwitchNode<T>;
            if (node == null) return;

            SerializedProperty casesProperty = serializedObject.FindProperty("_cases");
            HashSet<string> requiredPorts = new HashSet<string>();

            for (int i = 0; i < casesProperty.arraySize; i++)
            {
                string portName = GetPortNameFromProperty(
                    casesProperty.GetArrayElementAtIndex(i)
                );

                requiredPorts.Add(portName);

                if (!node.HasPort(portName))
                {
                    node.AddDynamicOutput(
                        typeof(NodeControlExecute),
                        Node.ConnectionType.Multiple,
                        Node.TypeConstraint.None,
                        portName
                    );
                }
            }

            List<string> portsToRemove = new List<string>();

            foreach (NodePort port in node.DynamicOutputs)
            {
                if (!requiredPorts.Contains(port.fieldName))
                {
                    portsToRemove.Add(port.fieldName);
                }
            }

            foreach (string portName in portsToRemove)
            {
                node.RemoveDynamicPort(portName);
            }
        }

        protected string GetPortNameFromProperty(SerializedProperty prop)
        {
            string[] parts = prop.propertyPath.Split('[', ']');
            return "case_" + parts[parts.Length - 2];
        }
    }
}