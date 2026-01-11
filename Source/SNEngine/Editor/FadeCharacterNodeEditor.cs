#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.CharacterSystem.Animations.Fade;
using SNEngine.CharacterSystem;
using SiphoinUnityHelpers.XNodeExtensions.Editor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(FadeCharacterNode))]
    public class FadeCharacterNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();
            FadeCharacterNode node = target as FadeCharacterNode;

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_target") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            GUILayout.Space(10);

            SerializedProperty targetProp = serializedObject.FindProperty("_target");
            Character currentCharacter = targetProp.objectReferenceValue as Character;

            string charName = currentCharacter != null ? currentCharacter.name : "Select Character";
            GUIContent content = new GUIContent(charName);

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = currentCharacter != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            if (GUILayout.Button(content, GUILayout.Height(32)))
            {
                CharacterSelectorWindow.Open((selected) => {
                    var so = new SerializedObject(target);
                    var prop = so.FindProperty("_target");
                    if (prop != null)
                    {
                        prop.objectReferenceValue = selected;
                        so.ApplyModifiedProperties();
                    }
                });
            }

            GUI.backgroundColor = prevBg;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif