using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.Language
{
    public abstract class LanguageEditorWorker : ScriptableObject
    {
        public abstract UniTask<LanguageWorkerResult> Work();
    }
}