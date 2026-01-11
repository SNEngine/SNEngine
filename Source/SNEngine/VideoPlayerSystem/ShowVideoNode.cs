namespace SNEngine.VideoPlayerSystem
{
    public class ShowVideoNode : VideoInteractionNode
    {
        protected override void Interact(NovelVideoPlayer input)
        {
            input.Show();
        }
    }
}
