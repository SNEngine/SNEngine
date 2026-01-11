using UnityEngine;

namespace SNEngine.Audio
{
    public class Set3DSettingsNode : AudioNodeInteraction
    {
        [Input, SerializeField, Range(0f, 1f)] private float _spatialBlend = 1f;
        [Input, SerializeField, Min(0f)] private float _minDistance = 1f;
        [Input, SerializeField, Min(0.1f)] private float _maxDistance = 500f;

        protected override void Interact(AudioObject input)
        {
            input.Set3DSettings(_spatialBlend, _minDistance, _maxDistance);
        }
    }
}
