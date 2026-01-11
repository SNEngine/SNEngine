#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using SiphoinUnityHelpers.XNodeExtensions.Variables.Set;
using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem;
using SNEngine.Graphs;
using SNEngine.GlobalVariables;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public static class XNodeEditorHelpers
    {
        // --- DICTIONARY METHODS ---

        public static void DrawGetGlobalDictionaryBody(NodeEditor editor, SerializedObject serializedObject)
        {
            serializedObject.Update();
            var guidProp = serializedObject.FindProperty("_guidVariable");
            var keyProp = serializedObject.FindProperty("_key");

            if (guidProp != null)
                DrawDictionarySelector(editor, serializedObject, "_guidVariable", DictionarySelectorWindow.SelectorMode.GlobalOnly);

            var dictNode = FindVariableByGuid(editor.target.graph as BaseGraph, guidProp?.stringValue, true);
            if (dictNode != null && keyProp != null)
                DrawKeyPopup(keyProp, dictNode);

            XNode.NodePort resultPort = editor.target.GetOutputPort("_result");
            if (resultPort != null) NodeEditorGUILayout.PortField(new GUIContent("Value"), resultPort);
            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawGetDictionaryBody(NodeEditor editor, SerializedObject serializedObject)
        {
            serializedObject.Update();
            var guidProp = serializedObject.FindProperty("_guidVariable");
            var keyProp = serializedObject.FindProperty("_key");
            var dictPort = editor.target.GetInputPort("_dictionary");

            VariableNode dictNode = null;
            if (dictPort != null && dictPort.IsConnected)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Dictionary"), dictPort);
                dictNode = dictPort.GetConnection(0).node as VariableNode;
            }
            else if (guidProp != null)
            {
                DrawDictionarySelector(editor, serializedObject, "_guidVariable", DictionarySelectorWindow.SelectorMode.LocalOnly);
                dictNode = FindVariableByGuid(editor.target.graph as BaseGraph, guidProp.stringValue, false);
            }

            if (dictNode != null && keyProp != null)
                DrawKeyPopup(keyProp, dictNode);

            XNode.NodePort valuePort = editor.target.GetOutputPort("_value");
            if (valuePort != null) NodeEditorGUILayout.PortField(new GUIContent("Value"), valuePort);
            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawSetDictionaryBody(NodeEditor editor, SerializedObject serializedObject)
        {
            serializedObject.Update();
            var guidProp = serializedObject.FindProperty("_guidVariable");
            var keyProp = serializedObject.FindProperty("_key");
            var dictPort = editor.target.GetInputPort("_dictionary");
            var valueInputPort = editor.target.GetInputPort("_value");

            VariableNode dictNode = null;
            if (dictPort != null && dictPort.IsConnected)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Dictionary"), dictPort);
                dictNode = dictPort.GetConnection(0).node as VariableNode;
            }
            else if (guidProp != null)
            {
                DrawDictionarySelector(editor, serializedObject, "_guidVariable", DictionarySelectorWindow.SelectorMode.LocalOnly);
                GUILayout.Space(5);

                dictNode = FindVariableByGuid(editor.target.graph as BaseGraph, guidProp.stringValue, false);
                NodeEditorGUILayout.PortField(new GUIContent(" "), dictPort);
            }

            if (dictNode != null && keyProp != null)
                DrawKeyPopup(keyProp, dictNode);

            if (valueInputPort != null) NodeEditorGUILayout.PortField(new GUIContent("New Value"), valueInputPort);
            serializedObject.ApplyModifiedProperties();
        }

        // --- VARIABLE METHODS (RESTORED) ---

        public static void DrawGetGlobalVaritableBody(NodeEditor editor, SerializedObject serializedObject)
        {
            serializedObject.Update();
            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_guidVariable" || tag.name == "_result") continue;
                SerializedProperty prop = serializedObject.FindProperty(tag.name);
                if (prop != null) NodeEditorGUILayout.PropertyField(prop);
            }

            DrawSelector(editor, serializedObject, "_guidVariable", VariableselectorWindow.SelectorMode.GlobalOnly);
            GUILayout.Space(5);


            XNode.NodePort resultPort = editor.target.GetOutputPort("_result");
            if (resultPort != null) NodeEditorGUILayout.PortField(new GUIContent("Value"), resultPort);
            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawSetVaritableBody(NodeEditor editor, SerializedObject serializedObject)
        {
            serializedObject.Update();
            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_targetGuid" || tag.name == "_index") continue;

                if (tag.name == "_variable")
                {
                    XNode.NodePort p = editor.target.GetInputPort("_variable");

                    if (p != null && p.IsConnected)
                    {
                        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
                    }
                    else
                    {
                        DrawSelector(editor, serializedObject, "_targetGuid", VariableselectorWindow.SelectorMode.All);

                        var guidProp = serializedObject.FindProperty("_targetGuid");
                        var targetNode = FindVariableByGuid(editor.target.graph as BaseGraph, guidProp.stringValue, false);

                        if (targetNode != null)
                        {
                            Type nodeType = targetNode.GetType();
                            bool isCollection = false;

                            Type currentType = nodeType;
                            while (currentType != null && currentType != typeof(object))
                            {
                                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(VariableCollectionNode<>))
                                {
                                    isCollection = true;
                                    break;
                                }
                                currentType = currentType.BaseType;
                            }

                            if (isCollection)
                            {
                                EditorGUILayout.Space(2);
                                SerializedProperty indexProp = serializedObject.FindProperty("_index");
                                if (indexProp != null)
                                {
                                    NodeEditorGUILayout.PropertyField(indexProp, new GUIContent("Index"));
                                }
                            }
                        }
                    }
                    GUILayout.Space(5);
                    continue;
                }

                SerializedProperty generalProp = serializedObject.FindProperty(tag.name);
                if (generalProp != null) NodeEditorGUILayout.PropertyField(generalProp);
            }
            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawRemoveAtVariableBody(NodeEditor editor, SerializedObject serializedObject)
        {
            serializedObject.Update();

            // Отрисовка входного порта выполнения (Flow)
            XNode.NodePort enterPort = editor.target.GetInputPort("_enter");
            EditorGUILayout.Space(2);
            if (enterPort != null) NodeEditorGUILayout.PortField(enterPort);

            XNode.NodePort variablePort = editor.target.GetInputPort("_variable");
            XNode.NodePort indexPort = editor.target.GetInputPort("_index");

            if (variablePort != null && variablePort.IsConnected)
            {
                NodeEditorGUILayout.PortField(variablePort);
            }
            else
            {
                DrawSelector(editor, serializedObject, "_targetGuid", VariableselectorWindow.SelectorMode.All);
            }

            EditorGUILayout.Space(2);

            if (indexPort != null && indexPort.IsConnected)
            {
                NodeEditorGUILayout.PortField(indexPort);
            }
            else
            {
                SerializedProperty indexProp = serializedObject.FindProperty("_index");
                if (indexProp != null)
                {
                    EditorGUILayout.PropertyField(indexProp, new GUIContent("Index"));
                    EditorGUILayout.Space(2);
                }
            }

            // Отрисовка выходного порта выполнения (Flow)
            XNode.NodePort exitPort = editor.target.GetOutputPort("_exit");
            if (exitPort != null) NodeEditorGUILayout.PortField(exitPort);

            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawGetCountInCollectionBody(NodeEditor editor, SerializedObject serializedObject)
        {
            serializedObject.Update();

            XNode.NodePort collectionPort = editor.target.GetInputPort("_collection");
            XNode.NodePort countPort = editor.target.GetOutputPort("_count");

            if (collectionPort != null && collectionPort.IsConnected)
            {
                NodeEditorGUILayout.PortField(collectionPort);
            }
            else
            {
                DrawSelector(editor, serializedObject, "_targetGuid", VariableselectorWindow.SelectorMode.All);
            }

            EditorGUILayout.Space(2);

            if (countPort != null)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Count"), countPort);
            }

            serializedObject.ApplyModifiedProperties();
        }
        // --- INTERNAL HELPERS ---

        private static void DrawKeyPopup(SerializedProperty keyProp, VariableNode dictNode)
        {
            if (dictNode is DictionaryVariableNode concreteDict)
            {
                var keys = concreteDict.Keys.Cast<object>().Select(k => k.ToString()).ToArray();
                if (keys.Length > 0)
                {
                    int currentIndex = Array.IndexOf(keys, keyProp.stringValue);
                    if (currentIndex == -1) currentIndex = 0;
                    int newIndex = EditorGUILayout.Popup("Key", currentIndex, keys);
                    keyProp.stringValue = keys[newIndex];
                }
            }
        }

        private static void DrawDictionarySelector(NodeEditor editor, SerializedObject serializedObject, string propertyName, DictionarySelectorWindow.SelectorMode mode)
        {
            var guidProp = serializedObject.FindProperty(propertyName);
            if (guidProp == null) return;

            Type baseType = editor.target.GetType();
            while (baseType != null && (!baseType.IsGenericType ||
                (!baseType.GetGenericTypeDefinition().Name.Contains("DictionaryVariableNode"))))
            {
                baseType = baseType.BaseType;
            }

            if (baseType == null) return;
            Type[] genericArgs = baseType.GetGenericArguments();

            string displayName = "Select Dictionary";
            VariableNode linkedNode = FindVariableByGuid(editor.target.graph as BaseGraph, guidProp.stringValue, mode == DictionarySelectorWindow.SelectorMode.GlobalOnly);
            if (linkedNode != null) displayName = linkedNode.Name;

            if (GUILayout.Button(displayName, GUILayout.Height(24)))
            {
                DictionarySelectorWindow.Open(editor.target.graph as BaseGraph, genericArgs[0], genericArgs[1], (selected) => {
                    var so = new SerializedObject(editor.target);
                    so.FindProperty(propertyName).stringValue = selected.GUID;
                    so.ApplyModifiedProperties();
                }, mode);
            }
        }

        private static void DrawSelector(NodeEditor editor, SerializedObject serializedObject, string propertyName, VariableselectorWindow.SelectorMode mode)
        {
            var guidProp = serializedObject.FindProperty(propertyName);
            if (guidProp == null) return;

            string currentGuid = guidProp.stringValue;
            string displayName = "Select Variable";
            Color buttonColor = new Color(0.25f, 0.25f, 0.25f);

            bool isGlobal = mode == VariableselectorWindow.SelectorMode.GlobalOnly;
            VariableNode linkedNode = FindVariableByGuid(editor.target.graph as BaseGraph, currentGuid, isGlobal);

            if (linkedNode != null)
            {
                displayName = linkedNode.Name;
                buttonColor = linkedNode.Color;
            }

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;
            if (GUILayout.Button(displayName, GUILayout.Height(24)))
            {
                Type genericType = GetGenericType(editor.target.GetType());
                VariableselectorWindow.Open(editor.target.graph as BaseGraph, genericType, (selectedNode) =>
                {
                    var so = new SerializedObject(editor.target);
                    so.FindProperty(propertyName).stringValue = selectedNode.GUID;
                    so.ApplyModifiedProperties();
                }, mode);
            }
            GUI.backgroundColor = prevColor;
        }

        private static VariableNode FindVariableByGuid(BaseGraph currentGraph, string guid, bool isGlobal)
        {
            if (string.IsNullOrEmpty(guid)) return null;
            if (!isGlobal && currentGraph != null)
            {
                var node = currentGraph.GetNodeByGuid(guid) as VariableNode;
                if (node != null) return node;
            }
            if (isGlobal || guid.Contains("-"))
            {
                var containers = Resources.LoadAll<VariableContainerGraph>("");
                foreach (var container in containers)
                {
                    var node = container.nodes.OfType<VariableNode>().FirstOrDefault(n => n.GUID == guid);
                    if (node != null) return node;
                }
            }
            return null;
        }

        private static Type GetGenericType(Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType)
                {
                    Type def = type.GetGenericTypeDefinition();
                    if (def == typeof(SetVariableNode<>) || def.Name.StartsWith("GetVariableValue"))
                        return type.GetGenericArguments()[0];
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
#endif