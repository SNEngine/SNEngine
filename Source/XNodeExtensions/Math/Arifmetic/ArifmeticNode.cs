using System;
using UnityEngine;
namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    public abstract class ArifmeticNode<T> : OperationABNode<T, T>
    {
        [SerializeField] private ArifmeticType _type;
        protected override T Operation(T a, T b)
        {
            switch (_type)
            {
                case ArifmeticType.Increment:
                    return Increment(a, b);
                case ArifmeticType.Decrement:
                    return Decrement(a, b);
                case ArifmeticType.Divide:
                    return Divide(a, b);
                case ArifmeticType.Multiply:
                    return Multiply(a, b);
                case ArifmeticType.Percent:
                    return Percent(a, b);
            }

            throw new InvalidOperationException("invalid operation type");
        }

        protected abstract T Increment(T a, T b);

        protected abstract T Decrement(T a, T b);

        protected abstract T Divide(T a, T b);

        protected abstract T Multiply(T a, T b);

        protected abstract T Percent(T a, T b);
    }
}
