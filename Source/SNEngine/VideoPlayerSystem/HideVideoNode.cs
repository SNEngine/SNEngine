namespace SNEngine.VideoPlayerSystem
{
    public class HideVideoNode : VideoInteractionNode
    {
        protected override void Interact(NovelVideoPlayer input)
        {
            input.Hide();
        }
    }
}
