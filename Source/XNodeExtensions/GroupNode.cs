using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public class GroupNode : DecoratorNode
    {
        private const int OFFSET_HEIGHT = 30;

        [SerializeField] private string _name = "New Group";

        /// <summary> Gets nodes in this group </summary>
        public List<Node> GetNodes()
        {
            List<Node> result = new List<Node>();
            foreach (Node node in graph.nodes)
            {
                if (node == this) continue;
                if (node == null) continue;
                if (node.position.x < position.x) continue;
                if (node.position.y < position.y) continue;
                if (node.position.x > position.x + Width) continue;
                if (node.position.y > position.y + Height + OFFSET_HEIGHT) continue;
                result.Add(node);
            }
            return result;
        }

        private void OnValidate()
        {
            name = _name;
        }
    }

    
}
