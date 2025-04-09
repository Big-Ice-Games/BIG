// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace BIG
{
    [InitializeOnLoad]
    public static class DefineInitializer
    {
        private const string BIG_WORKBOOK = "BIG_WORKBOOK";

        static DefineInitializer()
        {
            GameInitializer.AssertProjectStructure();

            try
            {
                AssetDatabase.Refresh();
                var settings = GameInitializer.GetSettings();
                if (settings == null || settings.UseWorkbook)
                    AddDefineIfMissing(BIG_WORKBOOK);
                else
                    RemoveDefineIfExists(BIG_WORKBOOK);
            }
            catch 
            {
                // Ignore.
            }
        }

        private static void AddDefineIfMissing(string define)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var nameBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            string defines = PlayerSettings.GetScriptingDefineSymbols(nameBuildTarget);

            if (!defines.Contains(define))
            {
                if (!string.IsNullOrEmpty(defines))
                    defines += ";" + define;
                else
                    defines = define;

                PlayerSettings.SetScriptingDefineSymbols(nameBuildTarget, defines);
                Debug.Log($"[BIG] Added scripting define symbol: {define}");
            }
        }
        
        private static void RemoveDefineIfExists(string define)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            string defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            var defineList = defines.Split(';').ToList();

            if (defineList.Contains(define))
            {
                defineList.Remove(define);
                string newDefines = string.Join(";", defineList);
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);

                Debug.Log($"[BIG] Removed scripting define symbol: {define}");
            }
        }
    }
    
    [JetBrains.Annotations.UsedImplicitly]
    internal sealed class GameInitializer
    {
        private const uint DEFAULT_STEAM_APP_ID = 440;
        private const string DEFAULT_DICTIONARY_EXAMPLE = "1rWbQgslF4K0RKB128MmoDhHlKUQbvL7MD08AdN2twAc";
        
        /// <summary>
        /// Create default <see cref="ILogger"/> implementation for Unity <see cref="UnityLogger"/>.
        /// Register all assembly modules defined in ResourcesModules folder.
        /// If we are building for Editor or Standalone platforms, register the standalone registration - all objects marked with [Register] attribute.
        /// Call <see cref="AfterDependenciesInitialization"/> at the end.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitDependencies()
        {
            // Because reflection extension caches assemblies also for Editor, we need to clear this cache when game is started.
            ReflectionExtension.ClearCache();
            #if UNITY_EDITOR
            AssertProjectStructure();
            #endif
            
            Application.quitting += OnQuit;
            
            var modules = LoadAllAssemblyModules();
            
            God.Ask()
                .WithLogger(new UnityLogger())
                .WithAssemblyModules(modules)
                #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
                .WithStandaloneRegistration()
                #endif
                .CreateWorld();

            AfterDependenciesInitialization();
        }

        /// <summary>
        /// It creates Resources/Modules folder if it doesn't exist.
        /// In this path it creates <see cref="Settings"/> and <see cref="BigAssemblyModule"/> objects if they don't exist.
        /// </summary>
        internal static void AssertProjectStructure()
        {
            string resourcesPath = "Assets/Resources";
            string modulesPath = Path.Combine(resourcesPath, "BIG");
            
            if (!AssetDatabase.IsValidFolder(resourcesPath))
                AssetDatabase.CreateFolder("Assets", "Resources");

            if (!AssetDatabase.IsValidFolder(modulesPath))
                AssetDatabase.CreateFolder("Assets/Resources", "BIG");
            
            string settingsPath = "Assets/Resources/BIG/Settings.asset";

            if (AssertFileInPath(settingsPath, out Settings settings))
            {
                settings.SteamAppId = DEFAULT_STEAM_APP_ID;
                settings.GoogleWorkbookDictionaryId = DEFAULT_DICTIONARY_EXAMPLE;
                settings.UseWorkbook = true;
                EditorUtility.SetDirty(settings);
            }
            
            string bigAssemblyModule = "Assets/Resources/BIG/BigAssemblyModule.asset";
            if (AssertFileInPath(bigAssemblyModule, out BigAssemblyModule assemblyModule))
            {
                assemblyModule.Settings = settings;
                EditorUtility.SetDirty(assemblyModule);
            }
            AssetDatabase.SaveAssets();
        }

        private static bool AssertFileInPath<T>(string path, out T instance) where T : ScriptableObject
        {
            if (!File.Exists(path))
            {
                instance = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();
                return true;
            }

            instance = null;
            return false;
        }
        
        /// <summary> 
        /// Load all Scriptable <see cref="IAssemblyModule"/> implementations from Resources/Modules folder.
        /// </summary>
        private static List<IAssemblyModule> LoadAllAssemblyModules()
        {
            var loaded = Resources.LoadAll<ScriptableObject>("BIG");
            var modules = loaded.OfType<IAssemblyModule>().ToList();
            return modules;
        }
        
        internal static ISettings GetSettings()
        {
            var loaded = Resources.LoadAll<ScriptableObject>("BIG");
            var settings = loaded.OfType<Settings>().FirstOrDefault();
            return settings;
        }

        /// <summary>
        /// Creates [BIG] GameObject with <see cref="MainThreadActionExecutor"/> component on it and don't destroy it on load.
        /// </summary>
        private static void AfterDependenciesInitialization()
        {
            var emptyGameObject = new GameObject("BIG");
            emptyGameObject.AddComponent<MainThreadActionExecutor>();
            GameObject.DontDestroyOnLoad(emptyGameObject);
        }
        
        private static void OnQuit()
        {
            Application.quitting -= OnQuit;
            God.Dispose();
        }
    }
}
