namespace SNEngine.VideoPlayerSystem
{
    public class StopVideoNode : VideoInteractionNode
    {
        protected override void Interact(NovelVideoPlayer input)
        {
            input.Stop();
        }
    }
}
