// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BIG
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    public static class DefineInitializer
    {
        private const string BIG_WORKBOOK = "BIG_WORKBOOK";

        static DefineInitializer()
        {
            GameInitializer.AssertProjectStructure();

            try
            {
                UnityEditor.AssetDatabase.Refresh();
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
            var targetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            var nameBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            string defines = UnityEditor.PlayerSettings.GetScriptingDefineSymbols(nameBuildTarget);

            if (!defines.Contains(define))
            {
                if (!string.IsNullOrEmpty(defines))
                    defines += ";" + define;
                else
                    defines = define;

                UnityEditor.PlayerSettings.SetScriptingDefineSymbols(nameBuildTarget, defines);
                Debug.Log($"[BIG] Added scripting define symbol: {define}");
            }
        }
        
        private static void RemoveDefineIfExists(string define)
        {
            var targetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            string defines = UnityEditor.PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            var defineList = defines.Split(';').ToList();

            if (defineList.Contains(define))
            {
                defineList.Remove(define);
                string newDefines = string.Join(";", defineList);
                UnityEditor.PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);

                Debug.Log($"[BIG] Removed scripting define symbol: {define}");
            }
        }
    }
    #endif
    
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
            var sceneInjectors = LoadAllSceneInjectors();
            
            God.Ask()
                .WithLogger(new UnityLogger())
                .WithAssemblyModules(modules)
                .WithSceneInjectors(sceneInjectors)
                #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
                .WithStandaloneRegistration()
                #endif
                .CreateWorld();

            AfterDependenciesInitialization();
        }

        #if UNITY_EDITOR
        /// <summary>
        /// It creates Resources/Modules folder if it doesn't exist.
        /// In this path it creates <see cref="Settings"/> and <see cref="BigAssemblyModule"/> objects if they don't exist.
        /// </summary>
        internal static void AssertProjectStructure()
        {
            string resourcesPath = "Assets/Resources";
            string modulesPath = Path.Combine(resourcesPath, "BIG");
            
            if (!UnityEditor.AssetDatabase.IsValidFolder(resourcesPath))
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

            if (!UnityEditor.AssetDatabase.IsValidFolder(modulesPath))
                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "BIG");
            
            string settingsPath = "Assets/Resources/BIG/Settings.asset";

            if (AssertFileInPath(settingsPath, out Settings settings))
            {
                settings.SteamAppId = DEFAULT_STEAM_APP_ID;
                settings.GoogleWorkbookDictionaryId = DEFAULT_DICTIONARY_EXAMPLE;
                settings.UseWorkbook = true;
                UnityEditor.EditorUtility.SetDirty(settings);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(settings);
            }
            
            string bigAssemblyModule = "Assets/Resources/BIG/BigAssemblyModule.asset";
            if (AssertFileInPath(bigAssemblyModule, out BigAssemblyModule assemblyModule))
            {
                assemblyModule.Settings = settings;
                UnityEditor.EditorUtility.SetDirty(assemblyModule);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(assemblyModule);
            }
        }

        private static bool AssertFileInPath<T>(string path, out T instance) where T : ScriptableObject
        {
            if (!File.Exists(path))
            {
                instance = ScriptableObject.CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(instance, path);
                UnityEditor.AssetDatabase.SaveAssets();
                return true;
            }

            instance = null;
            return false;
        }
        #endif
        
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
        
        private static List<ISceneInjector> LoadAllSceneInjectors()
        {
            var loaded = Resources.FindObjectsOfTypeAll<Object>();
            var sceneInjectors = loaded.OfType<ISceneInjector>().ToList();
            return sceneInjectors;
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
