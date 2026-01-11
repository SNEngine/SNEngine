using UnityEditor;

namespace SNEngine.Editor
{
    [InitializeOnLoad]
    public class ExecutionOrderManager
    {
        static ExecutionOrderManager()
        {
            SetExecutionOrder();
        }

        public static void SetExecutionOrder()
        {
            string scriptName = typeof(NovelGame).Name;
            int priority = -100;

            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (monoScript.name == scriptName)
                {
                    if (MonoImporter.GetExecutionOrder(monoScript) != priority)
                    {
                        MonoImporter.SetExecutionOrder(monoScript, priority);
                    }
                    break;
                }
            }
        }
    }
}