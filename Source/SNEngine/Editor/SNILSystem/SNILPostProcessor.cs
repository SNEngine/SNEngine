using System;
using System.Collections.Generic;
using System.IO;
using SNEngine.Graphs;
using UnityEditor;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILPostProcessor
    {
        private static Dictionary<string, DialogueGraph> _createdGraphs = new Dictionary<string, DialogueGraph>();
        private static List<JumpToReference> _pendingJumps = new List<JumpToReference>();

        public static void RegisterGraph(string name, DialogueGraph graph)
        {
            // Store graphs using a sanitized lowercase key for robust matching
            var key = SanitizeForAssetName(name).ToLowerInvariant();
            if (!_createdGraphs.ContainsKey(key))
            {
                _createdGraphs[key] = graph;
                SNILDebug.Log($"Registered graph: {name} (key: {key})");
            }
        }

        public static bool IsGraphRegistered(string name)
        {
            var key = SanitizeForAssetName(name).ToLowerInvariant();
            return _createdGraphs.ContainsKey(key);
        }

        public static void RegisterJumpToReference(object node, string fieldName, string targetDialogueName)
        {
            _pendingJumps.Add(new JumpToReference
            {
                Node = node,
                FieldName = fieldName,
                TargetDialogueName = targetDialogueName
            });
            SNILDebug.Log($"Registered pending jump: {targetDialogueName}");
        }

        public static void ProcessAllReferences()
        {
            SNILDebug.Log($"Processing {_pendingJumps.Count} pending jumps for {_createdGraphs.Count} registered graphs");

            // Log pending jumps and registered graph keys for debugging
            foreach (var j in _pendingJumps)
            {
                SNILDebug.Log($"Pending jump -> Target: '{j.TargetDialogueName}' Field: '{j.FieldName}' Node: {GetNodeInfo(j.Node)}");
            }
            foreach (var g in _createdGraphs)
            {
                SNILDebug.Log($"Registered graph key: '{g.Key}' -> asset name: '{g.Value?.name}'");
            }

            foreach (var jumpRef in _pendingJumps)
            {
                SNILDebug.Log($"Processing jump: {jumpRef.TargetDialogueName}");

                var field = jumpRef.Node.GetType().GetField(jumpRef.FieldName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (field == null)
                {
                    SNILDebug.LogWarning($"Could not find field '{jumpRef.FieldName}' on node type {jumpRef.Node.GetType().Name} for jump reference to '{jumpRef.TargetDialogueName}'.");
                    continue;
                }

                // Normalize target name for robust matching (trim, remove invalid filename chars)
                string sanitizedTargetName = SanitizeForAssetName(jumpRef.TargetDialogueName);
                string lowerKey = sanitizedTargetName.ToLowerInvariant();

                // Try to load the asset directly by path first (use sanitized name for path safety)
                string assetPath = $"Assets/SNEngine/Source/SNEngine/Resources/Dialogues/{sanitizedTargetName}.asset";
                DialogueGraph realGraph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(assetPath);
                SNILDebug.Log($"Tried direct load for '{sanitizedTargetName}' at path: {assetPath} -> {(realGraph != null ? "FOUND" : "NOT FOUND")}");

                // If not found, try searching by asset name (case-insensitive) using sanitized name
                if (realGraph == null)
                {
                    // First try a name: filter which should be more exact
                    string nameFilter = $"name:{sanitizedTargetName} t:DialogueGraph";
                    var guids = AssetDatabase.FindAssets(nameFilter);
                    if (guids.Length == 0)
                    {
                        // fallback to loose search
                        string filter = $"t:DialogueGraph {sanitizedTargetName}";
                        guids = AssetDatabase.FindAssets(filter);
                    }

                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        realGraph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(path);
                        SNILDebug.Log($"Found asset by search: {path}");
                    }
                }

                // If still not found, check registered graphs from this import (case-insensitive and sanitized match)
                if (realGraph == null && _createdGraphs.Count > 0)
                {
                    SNILDebug.Log($"Checking registered graphs for key: '{lowerKey}'");
                    if (_createdGraphs.ContainsKey(lowerKey))
                    {
                        realGraph = _createdGraphs[lowerKey];
                        SNILDebug.Log($"Resolved to registered graph with key: {lowerKey}");
                    }
                    else
                    {
                        // try a more flexible match
                        foreach (var kvp in _createdGraphs)
                        {
                            if (kvp.Key.Equals(lowerKey, StringComparison.OrdinalIgnoreCase))
                            {
                                realGraph = kvp.Value;
                                SNILDebug.Log($"Resolved to registered graph by case-insensitive match: {kvp.Key}");
                                break;
                            }
                        }
                    }
                }

                if (realGraph != null)
                {
                    field.SetValue(jumpRef.Node, realGraph);
                    SNILDebug.Log($"Successfully set jump reference from {GetNodeInfo(jumpRef.Node)} to {jumpRef.TargetDialogueName}");
                }
                else
                {
                    SNILDebug.LogWarning($"Could not resolve jump target '{jumpRef.TargetDialogueName}' (tried path: {assetPath}). Ensure the dialogue exists and is named correctly.");
                }
            }

            // Очищаем списки после обработки
            _pendingJumps.Clear();
            _createdGraphs.Clear();
        }

        private static string SanitizeForAssetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name?.Trim() ?? string.Empty;
            var s = name.Trim();
            foreach (char c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }

        private static string GetNodeInfo(object node)
        {
            if (node is SiphoinUnityHelpers.XNodeExtensions.BaseNode baseNode)
            {
                return $"{baseNode.GetType().Name} ({baseNode.GUID})";
            }
            return node.GetType().Name;
        }

        private class JumpToReference
        {
            public object Node { get; set; }
            public string FieldName { get; set; }
            public string TargetDialogueName { get; set; }
        }
    }
}