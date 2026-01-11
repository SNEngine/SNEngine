using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.Audio.Music;
using SNEngine.Editor;
using System.Linq;

namespace SNEngine.Audio
{
    [CustomNodeEditor(typeof(SetPlaylistMusicNode))]
    public class SetPlaylistMusicNodeEditor : NodeEditor
    {
        private Texture2D _customAudioIcon;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            if (_customAudioIcon == null)
            {
                _customAudioIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SNEngine/Source/SNEngine/Editor/Sprites/audio_editor_icon.png");
            }

            string[] excludes = { "m_Script", "graph", "position", "ports", "_input" };

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator, true);
            }

            DrawAudioList();

            foreach (XNode.NodePort dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAudioList()
        {
            SerializedProperty inputProp = serializedObject.FindProperty("_input");
            XNode.NodePort port = target.GetPort("_input");

            GUILayout.BeginVertical(EditorStyles.helpBox);

            Rect headerRect = EditorGUILayout.GetControlRect();
            NodeEditorGUILayout.PortField(headerRect.position, port);

            EditorGUI.LabelField(new Rect(headerRect.x + 20, headerRect.y, headerRect.width - 20, headerRect.height),
                "Playlist (" + inputProp.arraySize + ")", EditorStyles.boldLabel);

            int indexToRemove = -1;

            if (inputProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Playlist is empty", MessageType.None);
            }
            else
            {
                for (int i = 0; i < inputProp.arraySize; i++)
                {
                    SerializedProperty element = inputProp.GetArrayElementAtIndex(i);
                    AudioClip currentClip = element.objectReferenceValue as AudioClip;

                    EditorGUILayout.BeginHorizontal();

                    Rect fullRect = EditorGUILayout.GetControlRect(true, 22);
                    Rect iconRect = new Rect(fullRect.x, fullRect.y + 2, 18, 18);
                    Rect buttonRect = new Rect(fullRect.x + 22, fullRect.y, fullRect.width - 45, fullRect.height);
                    Rect removeRect = new Rect(fullRect.xMax - 22, fullRect.y + 1, 20, 20);

                    if (_customAudioIcon != null)
                    {
                        GUI.DrawTexture(iconRect, _customAudioIcon, ScaleMode.ScaleToFit);
                    }
                    else
                    {
                        GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("AudioClip Icon").image, ScaleMode.ScaleToFit);
                    }

                    string displayName = currentClip != null ? currentClip.name : "None (AudioClip)";
                    if (GUI.Button(buttonRect, displayName, EditorStyles.objectField))
                    {
                        int index = i;
                        AudioClipSelectorWindow.Open((selectedClip) =>
                        {
                            element.serializedObject.Update();
                            var p = element.serializedObject.FindProperty("_input");
                            p.GetArrayElementAtIndex(index).objectReferenceValue = selectedClip;
                            element.serializedObject.ApplyModifiedProperties();
                        });
                    }

                    if (GUI.Button(removeRect, "x", EditorStyles.miniButton))
                    {
                        indexToRemove = i;
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
            }

            if (GUILayout.Button("+ Add Clip", EditorStyles.miniButton))
            {
                inputProp.arraySize++;
            }

            GUILayout.EndVertical();

            if (indexToRemove != -1)
            {
                inputProp.DeleteArrayElementAtIndex(indexToRemove);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}