using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BIG
{
    [JetBrains.Annotations.UsedImplicitly]
    internal sealed class GameInitializer
    {
        /// <summary>
        /// Create default <see cref="ILogger"/> implementation for Unity <see cref="UnityLogger"/>.
        /// Register all assembly modules defined in Resources/Modules folder.
        /// If we are building for Editor or Standalone platforms, register the standalone registration - all objects marked with [Register] attribute.
        /// Call <see cref="AfterDependenciesInitialization"/> at the end.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitDependencies()
        {
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
        /// Load all Scriptable <see cref="IAssemblyModule"/> implementations from Resources/Modules folder.
        /// </summary>
        private static List<IAssemblyModule> LoadAllAssemblyModules()
        {
            var loaded = Resources.LoadAll<ScriptableObject>("Modules");
            var modules = loaded.OfType<IAssemblyModule>().ToList();
            return modules;
        }

        /// <summary>
        /// Creates [BIG] GameObject with MainThreadActionExecutor component on it and don't destroy it on load.
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
