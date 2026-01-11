using Cysharp.Threading.Tasks;
using SNEngine.DialogSystem;
using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Render Old Dialogue Service")]
    public class RenderOldDialogueService : ServiceBase, IService, IOldRenderDialogue
    {
        private IOldRenderDialogue _renderDialogue;
        private const int TIME_OUT_WAIT_TO_NEW_RENDERER = 35;

        public override void Initialize()
        {
            var render = Resources.Load<OldRenderDialogue>("Render/OldRenderDialogue");

            var prefab = Object.Instantiate(render);

            prefab.name = render.name;

            Object.DontDestroyOnLoad(prefab);

            _renderDialogue = prefab;
        }

        public Texture2D UpdateRender()
        {
            return _renderDialogue.UpdateRender();
        }

        public void DisplayFrame(Texture2D frameTexture)
        {
            _renderDialogue.DisplayFrame(frameTexture);
        }

        public void HideFrame()
        {
           _renderDialogue.HideFrame();
        }

        public override async void ResetState()
        {
            await UniTask.Delay(TIME_OUT_WAIT_TO_NEW_RENDERER);
            await UniTask.WaitForEndOfFrame();
            _renderDialogue.ResetState();
        }
    }
}
