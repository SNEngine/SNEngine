using UnityEngine;
using XNode;

namespace SNEngine.Audio
{
    public class PlaySoundOneShotNode : AudioNodeInteraction
    {
        [Input(ShowBackingValue.Never)] public AudioClip _remoteClip;
        [SerializeField] private AudioClip _clip;
        [Input, SerializeField, Range(0f, 1f)] private float _volumeScale = 1f;

        protected override void Interact(AudioObject input)
        {
            AudioClip clipToPlay = GetInputValue(nameof(_remoteClip), _clip);

            if (clipToPlay != null)
                input.PlayOneShot(clipToPlay, _volumeScale);
        }
    }
}