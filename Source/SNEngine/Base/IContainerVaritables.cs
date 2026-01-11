using SiphoinUnityHelpers.XNodeExtensions;
using System.Collections.Generic;

namespace SNEngine.Graphs
{
    public interface IContainerVariables : IResetable
    {
        IDictionary<string, VariableNode> GlobalVariables { get; }
    }
}
