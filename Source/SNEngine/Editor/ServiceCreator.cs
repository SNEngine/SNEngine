using UnityEditor;

namespace SNEngine.Editor
{
    public static class ServiceCreator
    {
        private const string ServiceTemplate =
    @"using UnityEngine;
using SNEngine.Services;
[CreateAssetMenu(menuName = ""SNEngine/Custom/Services/New #SCRIPTNAME#"")] 
public class #SCRIPTNAME# : ServiceBase
{

    public override void Initialize()
    {
        // Called when the ServiceContainer starts up (usually once at game launch)
    }

    public override void ResetState()
    {
        // Called after a clear screen, or when starting a new dialogue/game state
    }
}";

        [MenuItem("Assets/Create/SNEngine/New C# Service", false, 82)]
        public static void CreateNewServiceScript()
        {
            // BaseCreator заменит оба вхождения #SCRIPTNAME# на имя файла
            BaseCreator.CreateScript(ServiceTemplate, "/NewService.cs");
        }
    }
}