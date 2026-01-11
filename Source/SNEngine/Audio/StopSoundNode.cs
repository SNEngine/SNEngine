namespace SNEngine.Audio
{
    public class StopSoundNode : AudioNodeInteraction
    {
        protected override void Interact(AudioObject input) => input.Stop();
    }
}
