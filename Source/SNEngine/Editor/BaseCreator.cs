using SNEngine.IO;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

public static class BaseCreator
{
    public static void CreateScript(string template, string defaultFileName)
    {
        string path = GetSelectedPathOrFallback();

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<DoCreateNewScriptAction>(),
            path + defaultFileName,
            null,
            template
        );
    }

    private class DoCreateNewScriptAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string content = resourceFile;
            string className = Path.GetFileNameWithoutExtension(pathName).Replace(" ", string.Empty);
            content = content.Replace("#SCRIPTNAME#", className);

            NovelFile.WriteAllText(pathName, content);

            AssetDatabase.ImportAsset(pathName);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (Object obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && NovelFile.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}