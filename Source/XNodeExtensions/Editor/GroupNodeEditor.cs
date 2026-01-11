using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions;

[CustomNodeEditor(typeof(GroupNode))]
	public class GroupNodeEditor : NodeEditor {
		private GroupNode group { get { return _group != null ? _group : _group = target as GroupNode; } }
		private GroupNode _group;
		public static Texture2D corner { get { return _corner != null ? _corner : _corner = Resources.Load<Texture2D>("xnode_corner"); } }
		private static Texture2D _corner;
		private bool isDragging;
		private Vector2 size;

		public override void OnBodyGUI() {
			Event e = Event.current;
			switch (e.type) {
				case EventType.MouseDrag:
					if (isDragging) {
						group.Width = Mathf.Max(200, (int) e.mousePosition.x + 16);
						group.Height = Mathf.Max(100, (int) e.mousePosition.y - 34);
						NodeEditorWindow.current.Repaint();
					}
					break;
				case EventType.MouseDown:
					// Ignore everything except left clicks
					if (e.button != 0) return;
					if (NodeEditorWindow.current.nodeSizes.TryGetValue(target, out size)) {
						// Mouse position checking is in node local space
						Rect lowerRight = new Rect(size.x - 34, size.y - 34, 30, 30);
						if (lowerRight.Contains(e.mousePosition)) {
							isDragging = true;
						}
					}
					break;
				case EventType.MouseUp:
					isDragging = false;
					// Select nodes inside the group
					if (Selection.Contains(target)) {
						List<Object> selection = Selection.objects.ToList();
						// Select Nodes
						selection.AddRange(group.GetNodes());
						// Select Reroutes
						foreach (Node node in target.graph.nodes) {
							if (node != null)
							{
								foreach (NodePort port in node.Ports) {
									for (int i = 0; i < port.ConnectionCount; i++) {
										List<Vector2> reroutes = port.GetReroutePoints(i);
										for (int k = 0; k < reroutes.Count; k++) {
											Vector2 p = reroutes[k];
											if (p.x < group.position.x) continue;
											if (p.y < group.position.y) continue;
											if (p.x > group.position.x + group.Width) continue;
											if (p.y > group.position.y + group.Height + 30) continue;
											if (NodeEditorWindow.current.selectedReroutes.Any(x => x.port == port && x.connectionIndex == i && x.pointIndex == k)) continue;
											NodeEditorWindow.current.selectedReroutes.Add(
												new XNodeEditor.Internal.RerouteReference(port, i, k)
											);
										}
									}
								}	
							}
							else
							{
								continue;
							}
						}
						Selection.objects = selection.Distinct().ToArray();
					}
					break;
				case EventType.Repaint:
					// Move to bottom
					if (target.graph.nodes.IndexOf(target) != 0) {
						target.graph.nodes.Remove(target);
						target.graph.nodes.Insert(0, target);
					}
					// Add scale cursors
					if (NodeEditorWindow.current.nodeSizes.TryGetValue(target, out size)) {
						Rect lowerRight = new Rect(target.position, new Vector2(30, 30));
						lowerRight.y += size.y - 34;
						lowerRight.x += size.x - 34;
						lowerRight = NodeEditorWindow.current.GridToWindowRect(lowerRight);
						NodeEditorWindow.current.onLateGUI += () => AddMouseRect(lowerRight);
					}
					break;
			}

			// Control height of node
			GUILayout.Space(group.Height);

			GUI.DrawTexture(new Rect(group.Width - 34, group.Height + 16, 24, 24), corner);
		}

		public override int GetWidth() {
			return group.Width;
		}

		public override Color GetTint() {
			return group.Color;
		}

		public static void AddMouseRect(Rect rect) {
			EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeUpLeft);
		}
	}
