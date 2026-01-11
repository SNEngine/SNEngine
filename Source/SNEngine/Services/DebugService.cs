using SNEngine.Utils;
using UnityEngine;
using IngameDebugConsole;
using SNEngine.Debugging;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Debug Service")]
    public class DebugService : ServiceBase
    {
        private const string CONSOLE_PREFAB_PATH = "UI/DebugConsole";
        private DebugLogManager _consoleInstance;

        public override void Initialize()
        {
            if (Debug.isDebugBuild || Application.isEditor)
            {
                CreateConsole();
            }
        }

        private void CreateConsole()
        {
            var prefab = ResourceLoader.LoadCustomOrVanilla<DebugLogManager>(CONSOLE_PREFAB_PATH);

            if (prefab is null)
            {
               NovelGameDebug.LogError("DebugConsole prefab not found in Resources/UI/DebugConsole");
                return;
            }

            _consoleInstance = Instantiate(prefab);
            _consoleInstance.name = "Debug";

            DontDestroyOnLoad(_consoleInstance.gameObject);
        }
    }
}