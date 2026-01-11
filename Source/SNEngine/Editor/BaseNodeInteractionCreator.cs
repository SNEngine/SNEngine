using UnityEditor;
namespace SNEngine.Editor
{
    public static class BaseNodeInteractionCreator
    {
        private const string BaseNodeInteractionTemplate =
    @"using SiphoinUnityHelpers.XNodeExtensions;

public class #SCRIPTNAME# : BaseNodeInteraction
{
    // This is a base node that can interact with flow controls like loops or ifs.

    public override void Execute()
    {
        // Add your immediate execution logic here
    }
    
    // public override object GetValue(NodePort port) 
    // {
    //     // Implement data output logic here
    //     return base.GetValue(port);
    // }
}";

        [MenuItem("Assets/Create/SNEngine/New Base Interaction Node", false, 182)]
        public static void CreateNewBaseNodeInteractionScript()
        {
            BaseCreator.CreateScript(BaseNodeInteractionTemplate, "/NewBaseNodeInteraction.cs");
        }
    }
}