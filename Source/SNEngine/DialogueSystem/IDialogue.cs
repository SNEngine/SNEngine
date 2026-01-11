using SiphoinUnityHelpers.XNodeExtensions;
using System;

namespace SNEngine.DialogSystem
{
    public interface IDialogue
    {
        object Name { get; }

        event Action OnStartExecute;
        event Action OnEndExecute;
        event Action<BaseNode> OnNextNode;

        void Execute();
        bool HasNextDialogueOnExit();
        void Pause();

        void Stop();
    }
}
