using SiphoinUnityHelpers.XNodeExtensions;
using System.Collections.Generic;
using UnityEngine;

namespace SNEngine.Graphs
{
    [CreateAssetMenu(menuName = "SNEngine/VariableContainerGraph")]
    public class VariableContainerGraph : BaseGraph, IContainerVariables
    {
        public IDictionary<string, VariableNode> GlobalVariables => Variables;

        public override void Execute()
        {
            BuidVariableNodes();
        }

        public void ResetState()
        {
            foreach (var Variable in Variables.Values)
            {
                Variable.ResetValue();
            }
        }

        public override string GetWindowTitle()
        {
            return "Global Variables";
        }
    }
}
