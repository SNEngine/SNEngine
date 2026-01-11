using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public class NewGameObjectNode : BaseNodeInteraction
    {
        [Input, SerializeField] private string _name;

        [SerializeField, Output(ShowBackingValue.Never)] private GameObject _gameObject;

       private GameObject _result;

        public override void Execute()
        {
            var endName = _name;

            string valueFromInputName = GetDataFromPort<string>(nameof(_name));

            if (valueFromInputName != null)
            {
                endName = valueFromInputName;
            }

            _result = new GameObject(endName);
        }

        public override object GetValue(NodePort port)
        {
            return _result;
        }



    }
}
