using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Name
{
    public class SetNameToGameObjectNode : SetDataToGameObjectNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _name;
        public override void SetData(GameObject gameObject)
        {
            string name = _name;

            string inputName = GetDataFromPort<string>(nameof(_name));

            if (inputName != null)
            {
                name = inputName;
            }

            gameObject.name = name;
        }
    }
}
