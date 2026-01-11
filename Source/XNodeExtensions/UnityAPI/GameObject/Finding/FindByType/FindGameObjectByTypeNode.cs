using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.FindByType
{
    public class FindGameObjectByTypeNode : FindWithTypeNode<GameObject>
    {
        protected override GameObject ReturnResult(Type type)
        {
            return GameObject.FindObjectOfType(type) as GameObject;
        }
    }
}
