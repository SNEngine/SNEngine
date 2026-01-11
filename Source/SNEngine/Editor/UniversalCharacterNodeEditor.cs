#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.CharacterSystem;
using SNEngine.DialogSystem;
using SNEngine.Animations;
using System;
using SiphoinUnityHelpers.XNodeExtensions.Editor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(AsyncCharacterNode))]
    public class UniversalCharacterNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            string characterFieldName = GetCharacterFieldName(target);

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (characterFieldName != null && tag.name == characterFieldName) continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            if (characterFieldName != null)
            {
                DrawCharacterSelector(characterFieldName);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private string GetCharacterFieldName(object node)
        {
            if (node is CharacterNode || node is DialogNode)
                return "_character";

            Type type = node.GetType();
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType)
                {
                    Type genDef = type.GetGenericTypeDefinition();
                    if (genDef == typeof(AnimationNode<>) ||
                        genDef == typeof(AnimationInOutNode<>) ||
                        genDef == typeof(DissolveNode<>))
                    {
                        Type[] args = type.GetGenericArguments();
                        if (args.Length > 0 && args[0] == typeof(Character))
                            return "_target";
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        private void DrawCharacterSelector(string fieldName)
        {
            SerializedProperty prop = serializedObject.FindProperty(fieldName);
            if (prop == null) return;

            Character currentCharacter = prop.objectReferenceValue as Character;

            GUILayout.Space(8);

            string charName = currentCharacter != null ? currentCharacter.name : "Select Character";
            GUIContent content = new GUIContent(charName);

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentCharacter != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            if (GUILayout.Button(content, GUILayout.Height(32)))
            {
                CharacterSelectorWindow.Open((selected) =>
                {
                    prop.serializedObject.Update();
                    prop.objectReferenceValue = selected;
                    prop.serializedObject.ApplyModifiedProperties();
                });
            }

            GUI.backgroundColor = prevBg;
        }
    }
}
#endif