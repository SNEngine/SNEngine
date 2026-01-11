using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.CharacterSystem;
using SNEngine.DialogSystem;
using SNEngine.Editor.SNILSystem;
using SNEngine.Editor.SNILSystem.NodeCreation;
using SNEngine.Graphs;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class DisplayedInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            // Check if the instruction matches the pattern "Displayed {character} says {text}" or "Displayed {character} says {text} with emotion {emotion}"
            var pattern = @"^Displayed\s+.+\s+says\s+.+(?:\s+with\s+emotion\s+.+)?$";
            return Regex.IsMatch(instruction.Trim(), pattern, RegexOptions.IgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized. 'name:' instruction must be processed before node instructions.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;

            // Parse the instruction to extract character, text, and optionally emotion
            // Pattern: "Displayed {character} says {text}" or "Displayed {character} says {text} with emotion {emotion}"
            var match = Regex.Match(instruction.Trim(), @"^Displayed\s+(.+?)\s+says\s+(.+?)(?:\s+with\s+emotion\s+(.+))?$", RegexOptions.IgnoreCase);
            if (!match.Success || match.Groups.Count < 3)
            {
                return InstructionResult.Error($"Invalid Displayed instruction format: {instruction}. Expected format: 'Displayed {{character}} says {{text}}' or 'Displayed {{character}} says {{text}} with emotion {{emotion}}'");
            }

            string characterName = match.Groups[1].Value.Trim();
            string text = match.Groups[2].Value.Trim();
            string emotion = match.Groups[3]?.Value.Trim() ?? "Default"; // Default emotion if not specified

            // Create ShowCharacterNode
            var showCharacterType = SNILTypeResolver.GetNodeType("ShowCharacterNode");
            if (showCharacterType == null)
            {
                return InstructionResult.Error("ShowCharacterNode type not found.");
            }

            var showCharacterNode = dialogueGraph.AddNode(showCharacterType) as CharacterNode;
            if (showCharacterNode == null)
            {
                return InstructionResult.Error("Failed to create ShowCharacterNode.");
            }

            // Find the character by name and assign it to the node
            var character = FindCharacterByName(characterName);

            var dialogNodeType = SNILTypeResolver.GetNodeType("DialogNode");
            if (dialogNodeType == null)
            {
                return InstructionResult.Error("DialogNode type not found.");
            }

            var dialogNode = dialogueGraph.AddNode(dialogNodeType) as DialogNode;
            if (dialogNode == null)
            {
                return InstructionResult.Error("Failed to create DialogNode.");
            }

            dialogNode.name = NodeFormatter.ToTitleCase($"{characterName} says");

            // Assign the character and text to the dialog node
            if (character != null)
            {
                var characterField = typeof(DialogNode).GetField("_character", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (characterField != null)
                {
                    characterField.SetValue(dialogNode, character);
                }
            }
            else
            {
                SNILDebug.LogWarning($"Character '{characterName}' not found for dialog node. Character assignment may fail at runtime.");
            }

            // Set the text for the dialog
            var textField = typeof(PrinterTextNode).GetField("_text", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (textField != null)
            {
                textField.SetValue(dialogNode, text);
            }

            // Always create and configure the ShowCharacterNode, but check emotion validity
            if (character != null)
            {
                // Check if the emotion exists for this character
                var emotionObj = character.GetEmotion(emotion);
                if (emotionObj == null)
                {
                    // Emotion not found, use default emotion instead of skipping
                    SNILDebug.LogWarning($"Emotion '{emotion}' not found for character '{characterName}'. Using default emotion instead.");
                    emotion = "Default"; // Use default emotion
                }

                // Use reflection to set the character field for ShowCharacterNode
                var characterField = typeof(CharacterNode).GetField("_character", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (characterField != null)
                {
                    characterField.SetValue(showCharacterNode, character);
                }

                // Set the emotion for the ShowCharacterNode
                var emotionField = typeof(ShowCharacterNode).GetField("_emotion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (emotionField != null)
                {
                    emotionField.SetValue(showCharacterNode, emotion);
                }
            }
            else
            {
                // If character not found, we'll set the name for later resolution
                // This is a fallback - ideally the character should be found in the project
                SNILDebug.LogWarning($"Character '{characterName}' not found in project. Character assignment may fail at runtime.");
            }

            showCharacterNode.name = NodeFormatter.ToTitleCase($"Show {characterName}");

            AssetDatabase.AddObjectToAsset(showCharacterNode, dialogueGraph);

            // Position the ShowCharacterNode appropriately
            showCharacterNode.position = new Vector2(context.Nodes.Count * 250, 0);

            // Add to context after positioning
            context.Nodes.Add(showCharacterNode);

            // Connect to previous node in main flow
            NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, showCharacterNode, context);

            // Position the DialogNode to the right of the ShowCharacterNode to avoid overlap
            dialogNode.position = new Vector2(showCharacterNode.position.x + 250, showCharacterNode.position.y);

            // Connect ShowCharacterNode to DialogNode
            var exitPort = showCharacterNode.GetExitPort();
            var enterPort = dialogNode.GetEnterPort();
            if (exitPort != null && enterPort != null)
            {
                exitPort.Connect(enterPort);
            }

            // Add dialogNode to context after connection
            if (!context.Nodes.Contains(dialogNode))
            {
                context.Nodes.Add(dialogNode);
            }

            // Update context.LastNode to be the dialogNode since it's the final node in the sequence
            context.LastNode = dialogNode;

            AssetDatabase.AddObjectToAsset(dialogNode, dialogueGraph);

            // Update the last node to be the dialog node for subsequent connections
            context.LastNode = dialogNode;

            return InstructionResult.Ok(dialogNode);
        }

        private Character FindCharacterByName(string characterName)
        {
            // Search for the character asset in the project
            string[] guids = AssetDatabase.FindAssets($"t:Character {characterName}");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<Character>(path);
            }

            // If not found by name, try to find any character asset that might match
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var character = AssetDatabase.LoadAssetAtPath<Character>(path);
                if (character != null && string.Equals(character.name, characterName, StringComparison.OrdinalIgnoreCase))
                {
                    return character;
                }
            }

            return null;
        }
    }
}