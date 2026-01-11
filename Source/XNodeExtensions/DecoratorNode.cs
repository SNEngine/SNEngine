using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public abstract class DecoratorNode : Node
    {
        [SerializeField] private int _width = 200;

        [SerializeField] private int _height = 200;

        [SerializeField] private Color _color = new Color(1f, 1f, 1f, 0.1f);

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public Color Color { get => _color; set => _color = value; }
    }
}
