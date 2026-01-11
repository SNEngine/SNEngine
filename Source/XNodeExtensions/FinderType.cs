using System;
using System.Reflection;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public static class FinderType
    {
        public static Type Find (string typeName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type type = null;

            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type item in types)
                {
                    if (item.Name == typeName)
                    {
                        type = item;

                        break;
                    }
                }

            }

            if (type is null)
            {
                throw new NullReferenceException($"type {typeName} not found");
            }

            return type;
        }
    }
}
