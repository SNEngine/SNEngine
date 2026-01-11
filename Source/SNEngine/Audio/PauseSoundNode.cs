namespace SNEngine.Audio
{
    public class PauseSoundNode : AudioNodeInteraction
    {
        protected override void Interact(AudioObject input) => input.Pause();
    }
}
