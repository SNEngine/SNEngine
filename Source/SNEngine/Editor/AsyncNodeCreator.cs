using UnityEditor;
namespace SNEngine.Editor
{
    public static class AsyncNodeCreator
    {
        private const string AsyncNodeTemplate =
    @"using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using Cysharp.Threading.Tasks;

public class #SCRIPTNAME# : AsyncNode
{
    // This node supports asynchronous waiting and cancellation.

    public override async void Execute()
    {
        base.Execute();
        // Start your async task here, using TokenSource.Token
        
        // Example: 
        // UniTask.Delay(1000, cancellationToken: TokenSource.Token)
        //     .ContinueWith(() => StopTask());
    }
    
    // public override object GetValue(NodePort port) 
    // {
    //     // Implement data output logic here
    //     return base.GetValue(port);
    // }
}";

        [MenuItem("Assets/Create/SNEngine/New Async Node", false, 183)]
        public static void CreateNewAsyncNodeScript()
        {
            BaseCreator.CreateScript(AsyncNodeTemplate, "/NewAsyncNode.cs");
        }
    }
}