#if UNITY_EDITOR
using SNEngine.Source.SNEngine.MessageSystem;
using UnityEngine;
using XNodeEditor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(MessageOnScreenNode))]
    public class MessageOnScreenNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            serializedObject.Update();
        }

        public override Color GetTint()
        {
            return new Color(0.29f, 0.8f, 0.9f);
        }
    }
}
#endif