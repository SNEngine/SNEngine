using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Attributes
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public ReadOnlyMode Mode { get; private set; }

        public Predicate<bool> Condition { get; private set; }
        

        public ReadOnlyAttribute(ReadOnlyMode mode = ReadOnlyMode.Always)
        {
            Mode = mode;
        }

        public ReadOnlyAttribute(ReadOnlyMode mode, Predicate<bool> predicate)
        {
            Mode = mode;

            Condition = predicate;
        }
    }
}
