using SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic;
using SiphoinUnityHelpers.XNodeExtensions.Math.Compare;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Loop
{
    [NodeWidth(250)]
    public class ForNode : LoopNode
    {
        [Input, SerializeField] private int _n = 2;

        [Input, SerializeField] private int _startValue;

        [SerializeField] private CompareType _computeType = CompareType.Lesser;

        [SerializeField] private ArifmeticType _arifmeticType = ArifmeticType.Increment;

        [Output(ShowBackingValue.Never), SerializeField] private int _currentIndex;


        private int _i;

        public override void Execute()
        {
            var nPort = GetInputPort(nameof(_n));

            var n = _n;

            var startValue = _startValue;

            var startValuePort = GetInputPort(nameof(_startValue));

            if (nPort.Connection != null)
            {
                n = (int)nPort.Connection.GetOutputValue();
            }

            if (startValuePort.Connection != null)
            {
                startValue = (int)startValuePort.Connection.GetOutputValue();
            }

            switch (_computeType)
            {
                case CompareType.Equals:
                    switch (_arifmeticType)
                    {
                        case ArifmeticType.Increment:
                            for (_i = startValue; _i == n; _i++)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Decrement:
                            for (_i = startValue; _i == n; _i--)
                            {
                                ExecutesNodesInFor();
                            }
                            break;

                        case ArifmeticType.Divide:
                            for (_i = startValue; _i == n; _i /= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Multiply:
                            for (_i = startValue; _i == n; _i *= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Percent:
                            for (_i = startValue; _i == n; _i %= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                    }
                    break;
                case CompareType.NotEquals:
                    switch (_arifmeticType)
                    {
                        case ArifmeticType.Increment:
                            for (_i = startValue; _i != n; _i++)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Decrement:
                            for (_i = startValue; _i != n; _i--)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Divide:
                            for (_i = startValue; _i != n; _i /= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Multiply:
                            for (_i = startValue; _i != n; _i *= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Percent:
                            for (_i = startValue; _i != n; _i %= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                    }
                    break;
                case CompareType.More:
                    switch (_arifmeticType)
                    {
                        case ArifmeticType.Increment:
                            for (_i = startValue; _i > n; _i--)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Decrement:
                            for (_i = startValue; _i > n; _i--)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Divide:
                            for (_i = startValue; _i > n; _i /= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Multiply:
                            for (_i = startValue; _i > n; _i *= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Percent:
                            for (_i = startValue; _i > n; _i %= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case CompareType.Lesser:
                    switch (_arifmeticType)
                    {
                        case ArifmeticType.Increment:
                            for (_i = startValue; _i < n; _i++)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Decrement:
                            for (_i = startValue; _i < n; _i++)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Divide:
                            for (_i = startValue; _i < n; _i /= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Multiply:
                            for (_i = startValue; _i < n; _i *= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Percent:
                            for (_i = startValue; _i < n; _i %= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                    }
                    break;
                case CompareType.LesserOrEquals:
                    switch (_arifmeticType)
                    {
                        case ArifmeticType.Increment:
                            for (_i = startValue; _i <= n; _i++)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Decrement:
                            for (_i = startValue; _i <= n; _i++)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Divide:
                            for (_i = startValue; _i <= n; _i /= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Multiply:
                            for (_i = startValue; _i <= n; _i *= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Percent:
                            for (_i = startValue; _i <= n; _i %= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                    }
                    break;
                case CompareType.MoreOrEquals:
                    switch (_arifmeticType)
                    {
                        case ArifmeticType.Increment:
                            for (_i = startValue; _i >= n; _i--)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Decrement:
                            for (_i = startValue; _i >= n; _i--)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Divide:
                            for (_i = startValue; _i >= n; _i /= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Multiply:
                            for (_i = startValue; _i >= n; _i *= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                        case ArifmeticType.Percent:
                            for (_i = startValue; _i >= n; _i %= n)
                            {
                                ExecutesNodesInFor();
                            }
                            break;
                    }
                    break;
            }
        }

        private void ExecutesNodesInFor()
        {
            CallLoop();
        }

        public override object GetValue(NodePort port)
        {
            return _i;
        }
    }

}