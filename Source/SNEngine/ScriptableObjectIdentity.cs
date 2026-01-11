using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using System;
using UnityEngine;

namespace SNEngine
{
    public class ScriptableObjectIdentity : ScriptableObject, Iidentity
    {
        [SerializeField, ReadOnly] private string _guidSO = Guid.NewGuid().ToShortGUID();

        public string GUID => _guidSO;
    }
}
