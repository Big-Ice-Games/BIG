# BIG
Base Library used among all Big Ice Games projects.

## <a href="https://github.com/Big-Ice-Games/BIG/tree/main/Runtime/DI" target="_blank">`DI`</a>
Dependency injection is the backbone of Big Ice Games projects.

First you are starting with <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/AssemblyModule.cs" target="_blank">`AssemblyModule`</a> definition.
For every assembly you can define one or more modules that will register injectable classes into your application.

To make sure we avoid dynamic compilation that can occure with <a href="https://github.com/autofac/Autofac" target="_blank">`Autofac`</a>, we use a 
"manual" registration approach. 

$${\color{red}IMPORTANT}$$ - dynamic compilation does not work on some platforms. Especially mobile platforms.

You can check how <a href="https://github.com/Big-Ice-Games/SpaceSmuggler/blob/main/Runtime/SpaceSmugglerAssemblyModule.cs" target="_blank">`Space Smuggler`</a> handles registration.

```
public override void Register(ContainerBuilder containerBuilder)
{
    containerBuilder.Register(c => new MainThreadActionQueue())
        .As<MainThreadActionQueue>()
        .Keyed<object>(typeof(MainThreadActionQueue).FullName)
        .SingleInstance();

   ...
}
```

Application entry happening when you invoke <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/God.cs" target="_blank">`WorldCreation`</a> in a God class.
It require <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/ILogger.cs" target="_blank">`ILogger`</a> implementation and a set of <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/AssemblyModule.cs" target="_blank">`AssemblyModules`</a> that you would like to register.

Additionally, you can mark your class with the <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/RegistrationRequiredAttribute.cs" target="_blank">`[RegistrationRequiredAttribute]`</a> to ensure that the developer receives the appropriate information about which types require registration but the application is unable to resolve them.

You can inject your dependencies into Unity Game Objects through <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/InjectAttribute.cs" target="_blank">`[InjectAttribute]`</a> and <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/RuntimeDependencyProvider.cs" target="_blank">`RuntimeDependencyProvider`</a> like this

```
public class ExampleClass : MonoBehaviour
{
  [Inject] private MainThreadActionsQueue _mainThreadActionsQueue;

  private void Awake()
  {
    this.ResolveMyDependencies();
  }
}
```
or hide it in a base class like this

```
public abstract class BaseBehaviour : MonoBehaviour
{
   private bool _initiated = false;
   protected void Awake()
   {
       if (_initiated) return;
       _initiated = true;
       this.ResolveMyDependencies();
       OnAwake();
   }

   protected virtual void OnAwake()
   {
   }
}

public class ExampleClass : BaseBehaviour
{
  [Inject] private MainThreadActionsQueue _mainThreadActionsQueue;
}
```

## <a href="https://github.com/Big-Ice-Games/BIG/tree/main/Runtime/Utils" target="_blank">`Utils`</a>
Avoiding code duplication, we keep extensions with broad applications here and use them in other projects. Useful things that are independent of a specific application are found here.

## <a href="https://github.com/Big-Ice-Games/BIG/tree/main/Runtime/Types" target="_blank">`Types`</a>
To maintain consistency of base types across applications, we build a repository of useful classes such as vectors, colors, etc.

## DLL 
files ready to use in Unity projects are available at the following URLs:

https://github.com/Big-Ice-Games/BIG/raw/refs/heads/main/bin/Release/netstandard2.1/Autofac.dll</br>
https://github.com/Big-Ice-Games/BIG/raw/refs/heads/main/bin/Release/netstandard2.1/BIG.dll

bin/debug and bin/release are not a part of the .gitignore because of that.
The latest version of BIG should be available ready to use without having to download the entire repository and build it yourself.

The process of using and updating can be automated in a project with a class like this:

```

using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

namespace Editor
{
    public class BigIceGamesRepositoryProvider : EditorWindow
    {
        private const string AutofacUrl = "https://github.com/Big-Ice-Games/BIG/raw/refs/heads/main/bin/Release/netstandard2.1/Autofac.dll";
        private const string BigUrl = "https://github.com/Big-Ice-Games/BIG/raw/refs/heads/main/bin/Release/netstandard2.1/BIG.dll";
        private const string AutofacDestinationPath = "Assets/Plugins/BigIceGames/Autofac.dll";
        private const string BigDestinationPath = "Assets/Plugins/BigIceGames/BIG.dll";

        [MenuItem("BIG/Update BIG dlls")]
        public static void UpdateBigDlls()
        {
            DownloadFile(AutofacUrl, AutofacDestinationPath);
            DownloadFile(BigUrl, BigDestinationPath);
        }

        public static void DownloadFile(string fileUrl, string destinationPath)
        {
            var directory = Path.GetDirectoryName(destinationPath);
            var file = Path.GetFileName(destinationPath);

            EditorUtility.DisplayProgressBar("Downloading File", $"Fetching {file}...", 0.5f);

            var webRequest = UnityWebRequest.Get(fileUrl);
            var operation = webRequest.SendWebRequest();

            operation.completed += _ =>
            {
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error downloading file: {webRequest.error}");
                }
                else
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllBytes(destinationPath, webRequest.downloadHandler.data);
                    Debug.Log($"File downloaded successfully to: {destinationPath}");
                }

                EditorUtility.ClearProgressBar();
            };
        }
    }
}

```
