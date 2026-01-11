using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions;

[CustomNodeEditor(typeof(SummaryNode))]
public class SummaryNodeEditor : NodeEditor
{
    private SummaryNode summary { get { return _summary != null ? _summary : _summary = target as SummaryNode; } }
    private SummaryNode _summary;
    public static Texture2D corner { get { return _corner != null ? _corner : _corner = Resources.Load<Texture2D>("xnode_corner"); } }
    private static Texture2D _corner;

    public override void OnBodyGUI()
    {

        summary.Height = _summary.Summary.Length + 200;

        summary.Width = _summary.Summary.Length + 150;

        GUIStyle gUIStyle = new GUIStyle(EditorStyles.whiteLargeLabel);

        gUIStyle.fontStyle = FontStyle.Bold;

        gUIStyle.wordWrap = true;

        gUIStyle.richText = true;

        // Draw summary text field
        _summary.Summary = EditorGUILayout.TextArea(_summary.Summary, gUIStyle);

        GUI.DrawTexture(new Rect(summary.Width - 34, summary.Height + 16, 24, 24), corner);

    }

    public override int GetWidth()
    {
        return summary.Width;
    }

    public override Color GetTint()
    {
        return summary.Color;
    }

    public static void AddMouseRect(Rect rect)
    {
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeUpLeft);
    }
}
