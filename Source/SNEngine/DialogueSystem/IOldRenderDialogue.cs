using UnityEngine;

namespace SNEngine.DialogSystem
{
    public interface IOldRenderDialogue : IResetable
    {
        Texture2D UpdateRender();

        void DisplayFrame(Texture2D frameTexture);
        void HideFrame();
    }
}