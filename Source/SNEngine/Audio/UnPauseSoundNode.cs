namespace SNEngine.Audio
{
    public class UnPauseSoundNode : AudioNodeInteraction
    {
        protected override void Interact(AudioObject input) => input.UnPause();
    }
}
