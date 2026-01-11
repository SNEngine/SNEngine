using System.Collections.Generic;
using UnityEngine;

namespace XNode
{
    public static class NodeHighlighter
    {
        private static HashSet<Node> highlightedNodes = new HashSet<Node>();
        private static Dictionary<Node, Color> customHighlightColors = new Dictionary<Node, Color>();

        public static void HighlightNode(Node node)
        {
            if (node != null)
            {
                highlightedNodes.Add(node);
                customHighlightColors.Remove(node);
                RequestRepaint();
            }
        }

        public static void HighlightNode(Node node, Color highlightColor)
        {
            if (node != null)
            {
                highlightedNodes.Add(node);
                customHighlightColors[node] = highlightColor;
                RequestRepaint();
            }
        }

        public static void RemoveHighlight(Node node)
        {
            if (node != null)
            {
                highlightedNodes.Remove(node);
                customHighlightColors.Remove(node);
                RequestRepaint();
            }
        }

        public static bool IsNodeHighlighted(Node node)
        {
            return node != null && highlightedNodes.Contains(node);
        }

        public static Color GetHighlightColor(Node node)
        {
            if (node != null && customHighlightColors.ContainsKey(node))
            {
                return customHighlightColors[node];
            }
            return GetDefaultHighlightColor();
        }

        public static Color GetDefaultHighlightColor()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorGUIUtility.isProSkin ?
                new Color32(255, 255, 255, 255) :
                new Color32(0, 0, 0, 255);
#else
            return Color.white;
#endif
        }

        public static void ClearAllHighlights()
        {
            highlightedNodes.Clear();
            customHighlightColors.Clear();
            RequestRepaint();
        }

        public static HashSet<Node> GetHighlightedNodes()
        {
            return new HashSet<Node>(highlightedNodes);
        }

        private static void RequestRepaint()
        {
#if UNITY_EDITOR
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
        }
    }
}