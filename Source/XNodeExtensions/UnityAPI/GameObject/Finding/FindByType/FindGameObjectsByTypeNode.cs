using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.FindByType
{
    public class FindGameObjectsByTypeNode : FindWithTypeNode<GameObject[]>
    {
        protected override GameObject[] ReturnResult(Type type)
        {
            return GameObject.FindObjectsOfType(type) as GameObject[];
        }
    }
}
