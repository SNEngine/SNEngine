using UnityEditor;
namespace SNEngine.Editor
{
    public static class RepositoryCreator
    {
        private const string RepositoryTemplate =
    @"using UnityEngine;
using SNEngine.Repositories;
[CreateAssetMenu(menuName = ""SNEngine/Custom/Repositories/#SCRIPTNAME#"")]
public class #SCRIPTNAME# : RepositoryBase
{
    // Place fields and properties specific to this repository here

    public override void Initialize()
    {
        // Called when the RepositoryContainer starts up (usually once at game launch)
    }
}";

        [MenuItem("Assets/Create/SNEngine/New C# Repository", false, 83)]
        public static void CreateNewRepositoryScript()
        {
            BaseCreator.CreateScript(RepositoryTemplate, "/NewRepository.cs");
        }
    }
}