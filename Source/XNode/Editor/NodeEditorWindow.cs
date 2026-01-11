using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace XNodeEditor
{
    [InitializeOnLoad]
    public partial class NodeEditorWindow : EditorWindow
    {
        public static NodeEditorWindow current;

        private static Dictionary<XNode.NodeGraph, NodeEditorWindow> _graphWindows = new Dictionary<XNode.NodeGraph, NodeEditorWindow>();

        public Dictionary<XNode.NodePort, Rect> portConnectionPoints { get { return _portConnectionPoints; } }
        private Dictionary<XNode.NodePort, Rect> _portConnectionPoints = new Dictionary<XNode.NodePort, Rect>();
        [SerializeField] private NodePortReference[] _references = new NodePortReference[0];
        [SerializeField] private Rect[] _rects = new Rect[0];

        private Func<bool> isDocked
        {
            get
            {
                if (_isDocked == null) _isDocked = this.GetIsDockedDelegate();
                return _isDocked;
            }
        }
        private Func<bool> _isDocked;

        [System.Serializable]
        private class NodePortReference
        {
            [SerializeField] private XNode.Node _node;
            [SerializeField] private string _name;

            public NodePortReference(XNode.NodePort nodePort)
            {
                _node = nodePort.node;
                _name = nodePort.fieldName;
            }

            public XNode.NodePort GetNodePort()
            {
                if (_node == null) return null;
                return _node.GetPort(_name);
            }
        }

        private void OnDisable()
        {
            if (graph != null && _graphWindows.ContainsKey(graph) && _graphWindows[graph] == this)
            {
                _graphWindows.Remove(graph);
            }

            int count = portConnectionPoints.Count;
            _references = new NodePortReference[count];
            _rects = new Rect[count];
            int index = 0;
            foreach (var portConnectionPoint in portConnectionPoints)
            {
                _references[index] = new NodePortReference(portConnectionPoint.Key);
                _rects[index] = portConnectionPoint.Value;
                index++;
            }
        }

        private void OnEnable()
        {
            int length = _references.Length;
            if (length == _rects.Length)
            {
                for (int i = 0; i < length; i++)
                {
                    XNode.NodePort nodePort = _references[i].GetNodePort();
                    if (nodePort != null)
                        _portConnectionPoints.Add(nodePort, _rects[i]);
                }
            }

            if (graph != null && !_graphWindows.ContainsKey(graph))
            {
                _graphWindows[graph] = this;
            }
        }

        public Dictionary<XNode.Node, Vector2> nodeSizes { get { return _nodeSizes; } }
        private Dictionary<XNode.Node, Vector2> _nodeSizes = new Dictionary<XNode.Node, Vector2>();
        public XNode.NodeGraph graph;
        public Vector2 panOffset { get { return _panOffset; } set { _panOffset = value; Repaint(); } }
        private Vector2 _panOffset;
        public float zoom { get { return _zoom; } set { _zoom = Mathf.Clamp(value, NodeEditorPreferences.GetSettings().minZoom, NodeEditorPreferences.GetSettings().maxZoom); Repaint(); } }
        private float _zoom = 1;

        void OnFocus()
        {
            current = this;
            ValidateGraphEditor();
            if (graphEditor != null)
            {
                graphEditor.OnWindowFocus();
                if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            }
            dragThreshold = Math.Max(1f, Screen.width / 1000f);
        }

        void OnLostFocus()
        {
            if (graphEditor != null) graphEditor.OnWindowFocusLost();
        }

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            XNode.NodeGraph nodeGraph = Selection.activeObject as XNode.NodeGraph;
            if (nodeGraph && !AssetDatabase.Contains(nodeGraph))
            {
                Open(nodeGraph);
            }
        }

        private void ValidateGraphEditor()
        {
            NodeGraphEditor graphEditor = NodeGraphEditor.GetEditor(graph, this);
            if (this.graphEditor != graphEditor && graphEditor != null)
            {
                this.graphEditor = graphEditor;
                graphEditor.OnOpen();
            }
        }

        public static NodeEditorWindow Init()
        {
            NodeEditorWindow w = CreateInstance<NodeEditorWindow>();
            w.titleContent = new GUIContent("xNode");
            w.wantsMouseMove = true;
            w.Show();
            return w;
        }

        public void Save()
        {
            if (AssetDatabase.Contains(graph))
            {
                EditorUtility.SetDirty(graph);
                if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            }
            else SaveAs();
        }

        public void SaveAs()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save NodeGraph", "NewNodeGraph", "asset", "");
            if (string.IsNullOrEmpty(path)) return;
            XNode.NodeGraph existingGraph = AssetDatabase.LoadAssetAtPath<XNode.NodeGraph>(path);
            if (existingGraph != null) AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(graph, path);
            EditorUtility.SetDirty(graph);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        private void DraggableWindow(int windowID)
        {
            GUI.DragWindow();
        }

        public Vector2 WindowToGridPosition(Vector2 windowPosition)
        {
            return (windowPosition - (position.size * 0.5f) - (panOffset / zoom)) * zoom;
        }

        public Vector2 GridToWindowPosition(Vector2 gridPosition)
        {
            return (position.size * 0.5f) + (panOffset / zoom) + (gridPosition / zoom);
        }

        public Rect GridToWindowRectNoClipped(Rect gridRect)
        {
            gridRect.position = GridToWindowPositionNoClipped(gridRect.position);
            return gridRect;
        }

        public Rect GridToWindowRect(Rect gridRect)
        {
            gridRect.position = GridToWindowPosition(gridRect.position);
            gridRect.size /= zoom;
            return gridRect;
        }

        public Vector2 GridToWindowPositionNoClipped(Vector2 gridPosition)
        {
            Vector2 center = position.size * 0.5f;
            float xOffset = Mathf.Round(center.x * zoom + (panOffset.x + gridPosition.x));
            float yOffset = Mathf.Round(center.y * zoom + (panOffset.y + gridPosition.y));
            return new Vector2(xOffset, yOffset);
        }

        public void SelectNode(XNode.Node node, bool add)
        {
            if (add)
            {
                List<Object> selection = new List<Object>(Selection.objects);
                selection.Add(node);
                Selection.objects = selection.ToArray();
            }
            else Selection.objects = new Object[] { node };
        }

        public void DeselectNode(XNode.Node node)
        {
            List<Object> selection = new List<Object>(Selection.objects);
            selection.Remove(node);
            Selection.objects = selection.ToArray();
        }

        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            XNode.NodeGraph nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as XNode.NodeGraph;
            if (nodeGraph != null)
            {
                Open(nodeGraph);
                return true;
            }
            return false;
        }

        public static NodeEditorWindow Open(XNode.NodeGraph graph)
        {
            if (!graph) return null;

            string title = graph.name;
            MethodInfo methodInfo = graph.GetType().GetMethod("GetWindowTitle", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (methodInfo != null)
            {
                object result = methodInfo.Invoke(graph, null);
                if (result != null) title = result.ToString();
            }

            return Open(graph, title);
        }

        public static NodeEditorWindow Open(XNode.NodeGraph graph, string windowTitle)
        {
            if (!graph) return null;

            if (_graphWindows.ContainsKey(graph) && _graphWindows[graph] != null)
            {
                NodeEditorWindow existingWindow = _graphWindows[graph];
                existingWindow.Focus();
                return existingWindow;
            }

            NodeEditorWindow w = CreateInstance<NodeEditorWindow>();

            GUIContent content = EditorGUIUtility.ObjectContent(graph, graph.GetType());
            w.titleContent = new GUIContent(windowTitle, content.image);

            w.wantsMouseMove = true;
            w.Show();

            if (w.graph != null && _graphWindows.ContainsKey(w.graph) && _graphWindows[w.graph] == w)
            {
                _graphWindows.Remove(w.graph);
            }

            w.graph = graph;
            _graphWindows[graph] = w;

            if (graph.nodes.Count > 0)
            {
                XNode.Node startNode = graph.nodes[0];
                if (startNode != null)
                {
                    Vector2 nodeSize = w.nodeSizes.ContainsKey(startNode) ? w.nodeSizes[startNode] / 2 : Vector2.zero;
                    w.panOffset = -startNode.position - nodeSize;
                }
            }
            else
            {
                w.zoom = 1;
                w.panOffset = Vector2.zero;
            }

            return w;
        }

        public static void RepaintAll()
        {
            CleanupNullWindows();

            foreach (var window in _graphWindows.Values)
            {
                if (window != null)
                    window.Repaint();
            }
        }

        private static void CleanupNullWindows()
        {
            var keysToRemove = new List<XNode.NodeGraph>();
            foreach (var kvp in _graphWindows)
            {
                if (kvp.Value == null)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _graphWindows.Remove(key);
            }
        }
    }
}