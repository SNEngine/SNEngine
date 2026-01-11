using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Tag
{
    public class SetTagForGameObject : SetDataToGameObjectNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _tag;
        public override void SetData(GameObject gameObject)
        {
            string tag = _tag;

            string inputTag = GetDataFromPort<string>(nameof(_tag));

            if (inputTag != null)
            {
                tag = inputTag;
            }

            gameObject.tag = tag;
        }
    }
}
