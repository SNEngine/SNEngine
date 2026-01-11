using System.Collections.Generic;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Collection.Operations
{
    public class GetCountOfElementsNode : CollectionOperationNode<int>
    {

        public override int Operation(List<object> list)
        {
            return list.Count;
        }
    }
}
