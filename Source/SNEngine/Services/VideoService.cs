using SNEngine.Utils;
using SNEngine.VideoPlayerSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Video Service")]
    public class VideoService : ServiceBase, IShowable, IHidden
    {
        public INovelVideoPlayer VideoPlayer { get; private set; }
        private const string VIDEO_PLAYER_VANILLA_PATH = "UI/VideoPlayer";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<NovelVideoPlayer>(VIDEO_PLAYER_VANILLA_PATH);

            if (input == null)
            {
                return;
            }

            var prefab = Instantiate(input);

            prefab.name = input.name;

            VideoPlayer = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            prefab.gameObject.SetActive(false);
        }

        public void Show()
        {
            VideoPlayer.Show();
        }

        public void Hide()
        {
            VideoPlayer.Hide();
            VideoPlayer.Stop();
        }

        public override void ResetState()
        {
            VideoPlayer.ResetState();
        }
    }
}