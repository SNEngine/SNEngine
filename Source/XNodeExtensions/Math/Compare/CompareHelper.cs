using System;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Compare
{
    public static class CompareHelper
    {



        private static bool TryParseComparingObjects (object first, object second, out double numberFirst, out double numberSecond)
        {
            numberFirst = double.NaN;

            numberSecond = double.NaN;

            return double.TryParse(first.ToString(), out numberFirst) && double.TryParse(second.ToString(), out numberSecond);
        }

        public static bool Compare(object first, object second, CompareType computeType)
        {
            double numberFirst = double.NaN;

            double numberSecond = double.NaN;

            switch (computeType)
            {
                case CompareType.Equals:
                    return first.Equals(second);
                case CompareType.NotEquals:
                    return !first.Equals(second);

                case CompareType.More:

                    if (TryParseComparingObjects(first, second, out numberFirst, out numberSecond))
                    {
                        return numberFirst > numberSecond;
                    }

                    break;

                case CompareType.Lesser:

                    if (TryParseComparingObjects(first, second, out numberFirst, out numberSecond))
                    {
                        return numberFirst < numberSecond;
                    }

                    else if (first.GetType() == typeof(string) && second.GetType() == typeof(string))
                    {
                        string firstString = first.ToString();

                        string secondString = second.ToString();

                        return firstString.Length < secondString.Length;
                    }

                    break;


                case CompareType.LesserOrEquals:

                    if (TryParseComparingObjects(first, second, out numberFirst, out numberSecond))
                    {
                        return numberFirst <= numberSecond;
                    }
                    break;
                case CompareType.MoreOrEquals:

                    if (TryParseComparingObjects(first, second, out numberFirst, out numberSecond))
                    {
                        return numberFirst >= numberSecond;
                    }
                    break;
            }

            return false;
        }

        public static bool Compare(object a, object b, CompareObjectsType type)
        {
            switch (type)
            {
                case CompareObjectsType.Equals:
                    return a == b;
                case CompareObjectsType.NotEquals:
                    return a != b;
            }

            return false;
        }

        public static bool Compare(bool a, bool b, CompareBoolsType type)
        {
            switch (type)
            {
                case CompareBoolsType.Equals:
                    return a == b;
                case CompareBoolsType.NotEquals:
                    return a != b;
            }

            return false;
        }

        public static bool Compare(string a, string b, CompareStringsType type)
        {
            switch (type)
            {
                case CompareStringsType.Equals:
                    return a == b;
                case CompareStringsType.NotEquals:
                    return a != b;
            }

            return false;
        }
    }
}
