using System;
using System.Linq;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILTypeResolver
    {
        public static Type GetNodeType(string name)
        {
            // Handle aliases for common node types
            string actualNodeName = GetActualNodeName(name);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var type = assembly.GetTypes().FirstOrDefault(t =>
                        t.Name == actualNodeName && typeof(BaseNode).IsAssignableFrom(t));
                    if (type != null) return type;
                }
                catch { continue; }
            }

            return null;
        }

        private static string GetActualNodeName(string nodeName)
        {
            // Map aliases to actual node types
            switch (nodeName.ToLower())
            {
                default:
                    return nodeName;
            }
        }
    }
}