using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.ResourceFinder
{
    public static class SNILResourceFinder
    {
        private static Dictionary<string, List<string>> _resourceCache = new Dictionary<string, List<string>>();
        private static bool _cacheInitialized = false;

        public static void InitializeCache()
        {
            if (_cacheInitialized) return;

            _resourceCache.Clear();

            // Ищем все ассеты в проекте (кроме сцен и скриптов)
            string[] allAssetGUIDs = AssetDatabase.FindAssets("t:Object");
            
            foreach (string guid in allAssetGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                
                // Исключаем .cs файлы и сцены
                if (path.EndsWith(".cs") || path.EndsWith(".unity")) continue;
                
                if (!_resourceCache.ContainsKey(fileName))
                {
                    _resourceCache[fileName] = new List<string>();
                }
                
                _resourceCache[fileName].Add(path);
            }

            _cacheInitialized = true;
        }

        public static string FindResourcePath(string resourceName, System.Type resourceType = null)
        {
            InitializeCache();

            // Если имя содержит путь, ищем точное совпадение
            if (resourceName.Contains("/") || resourceName.Contains("\\"))
            {
                string[] possiblePaths = {
                    resourceName,                         // Прямой путь
                    "Assets/" + resourceName,             // С добавлением Assets/
                    resourceName.Replace("Assets/", "")   // Без Assets/
                };

                foreach (string path in possiblePaths)
                {
                    string fullPath = path.Replace("\\", "/");
                    if (AssetDatabase.GUIDFromAssetPath(fullPath) != new GUID(""))
                    {
                        // Проверяем тип, если он указан
                        if (resourceType != null)
                        {
                            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(fullPath, resourceType);
                            if (asset != null)
                            {
                                return fullPath;
                            }
                        }
                        else
                        {
                            return fullPath;
                        }
                    }
                }
            }
            else
            {
                // Ищем по имени файла
                if (_resourceCache.ContainsKey(resourceName))
                {
                    var paths = _resourceCache[resourceName];
                    
                    if (paths.Count == 1)
                    {
                        // Если тип указан, проверяем совместимость
                        if (resourceType != null)
                        {
                            foreach (string path in paths)
                            {
                                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, resourceType);
                                if (asset != null)
                                {
                                    return path;
                                }
                            }
                            // Если не нашли подходящий по типу, возвращаем null
                            return null;
                        }
                        
                        return paths[0]; // Только один файл с таким именем
                    }
                    else if (paths.Count > 1)
                    {
                        // Несколько файлов с одинаковым именем - проверяем типы
                        if (resourceType != null)
                        {
                            List<string> compatiblePaths = new List<string>();
                            foreach (string path in paths)
                            {
                                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, resourceType);
                                if (asset != null)
                                {
                                    compatiblePaths.Add(path);
                                }
                            }
                            
                            if (compatiblePaths.Count == 1)
                            {
                                return compatiblePaths[0];
                            }
                            else if (compatiblePaths.Count > 1)
                            {
                                string compatiblePathsStr = string.Join(", ", compatiblePaths);
                                SNILDebug.LogError($"Multiple {resourceType.Name} assets found with name '{resourceName}': {compatiblePathsStr}. Please specify the full path.");
                                return null;
                            }
                        }
                        
                        // Если тип не указан или не нашли подходящие по типу
                        string allPathsStr = string.Join(", ", paths);
                        SNILDebug.LogError($"Multiple assets found with name '{resourceName}': {allPathsStr}. Please specify the full path.");
                        return null;
                    }
                }
            }

            // Если ничего не найдено
            string typeStr = resourceType != null ? resourceType.Name : "asset";
            SNILDebug.LogWarning($"{typeStr} with name or path '{resourceName}' not found.");
            return null;
        }
    }
}