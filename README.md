# BIG

Base Library used among all Big Ice Games projects.

It provides a set of useful functions that can speed up your Unity project in many ways. It is a base library that will be used by subsequent - more specific - plugins [TBA] and will be used in the example game [TBA].

* Dependency Injection features, based on <a href="https://github.com/autofac/Autofac" target="_blank">`Autofac`</a>. Built to work with Unity, but can also be used in other application layers such as server, web service, etc.
* Well optimized, struct based Events that you can Raise and Subscribe to. Fast, optimized and extreamly easy to use.
* Basic behavior designed for convenient use of DI and Events.
* Flexible Dependency Registration. You can define your own assembly modules, scene injectors or use registration attribute for non-mobile devices.
* A set of useful extensions that can be used to solve common problems in collections, networking, reflection, and more.
* Workbook that can map Spreadsheet files into optimized runtime structure that can be used to import excel files into your project to build scriptable objects from them.
* Translator created as an example of Workbook usage.
* Editor toolbar that you can extend with your own toolbar elements.

## Table of Contents
- [Installation](#installation)
- [Dependency Injection](#dependency-injection)
- [Events](#events)
- [Toolbar](#toolbar)

Installation
---
Go to Window > Package Manager > Click plus `+` sign button in the left upper corner and select `Install Package from git URL`.
![PackageInstallation_1](https://github.com/user-attachments/assets/98843125-4a81-487c-8911-cd711ff6dcd4)
Use this repository git url to download it : https://github.com/Big-Ice-Games/BIG.git.
![PackageInstallation_2](https://github.com/user-attachments/assets/809eb453-f3dc-4fd3-b47d-6f20569069cd)


Dependency Injection
---
<a href="https://github.com/Big-Ice-Games/BIG/tree/main/Runtime/DI" target="_blank">`DI`</a> is the backbone of Big Ice Games projects.

The best way to start is to just run the project for the first time.
<a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Unity/GameInitializer.cs" target="_blank">`Game Initializer`</a> should accomplish couple of easy steps.
* Assert path Assets/Resources/BIG.
* Create <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Unity/Settings.cs" target="_blank">`Settings`</a> asset in this path.
* Create <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Unity/BigAssemblyModule.cs" target="_blank">`Big Assembly Module`</a> asset in this path.
* Register <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Unity/UnityLogger.cs" target="_blank">`Unity Logger`</a>
* Register all <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/IAssemblyModule.cs" target="_blank">`Assembly modules`</a> placed in Assets/Resources/BIG/ in a same way like Big Assembly Module.
* Register all <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Unity/ISceneInjector.cs" target="_blank">`Scene Injectors`</a> that you can use to register objects that are already on your starting scene.
* Register all <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/RegisterAttribute.cs" target="_blank">`Register Attribute`</a> decorated classes and structures that you have in your project. [ONLY FOR NON-MOBILE BUILDS]
* Spawn new game object with <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Unity/MainThreadActionsExecutor.cs" target="_blank">`Main Thread Actions Executor`</a> and DontDestroyOnLoad option.

Based on that flow you can define your own registered types, assemblies, scene injectors etc.

$${\color{red}IMPORTANT}$$ - dynamic compilation does not work on some platforms. Especially mobile platforms.

You can check how <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/God.cs" target="_blank">`God`</a> handles registration of UnityLogger in `WithLogger` function or check example belowe.

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
or use a base class like this:

```
public class Entity : BaseBehaviour
{
    /// <summary>
    /// Will be available after Awake function.
    /// </summary>
    [Inject] private ISettings _settings;
}
```

Events
---

<a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Unity/BaseBehaviour.cs" target="_blank">`BaseBehaviour`</a> also handle <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Events/SubscribeAttribute.cs" target="_blank">`[Subscribe]`</a>
automatically using extension methods. You can build your own version easly or use these extensions as you like.

```
this.ResolveMyDependencies(); // handle [Inject] attributes.
protected virtual void OnEnable() => this.Subscribe(); // Handle [Subscribe] attributes.
protected virtual void OnDisable() => this.Unsubscribe(); // Handle [Subscribe] attributes.
```

Subscribe attribute take advantage of a simple <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Events/Events.cs" target="_blank">`Events`</a> bus implementation without dynamic allocations.

Raise event example: 
```
public readonly struct EventDataExample
{
    public EventDataExample(Transform referenceTypeExample, int valueTypeExample)
    {
        ReferenceTypeExample = referenceTypeExample;
        ValueTypeExample = valueTypeExample;
    }
    
    public readonly Transform ReferenceTypeExample;
    public readonly int ValueTypeExample;
}
public class EventPublisher : BaseBehaviour
{
    private void Update()
    {
        Events.Raise(new EventDataExample(transform, 44));
    }
}
```
Subscriber example:

```
public class EventSubscriber : BaseBehaviour
{
    [Subscribe]
    private void OnEventDataExample(EventDataExample e)
    {
        this.Log(e.ValueTypeExample.ToString());
    }
}
```


## <a href="https://github.com/Big-Ice-Games/BIG/tree/main/Runtime/Utils" target="_blank">`Utils`</a>
Avoiding code duplication, we keep extensions with broad applications here and use them in other projects. Useful things that are independent of a specific application are found here.

## Workbook

The workbook is designed to convert spreadsheets into a runtime framework that can be used to create scriptable assets or other runtime features.

It is used for game localization. To try it out, you need to define BIG_WORKBOOK in Project Settings > Player > Scripts Define Symbols.

BIG_WORKBOOK should work by default. To disable it, go to Assets/Resources/BIG/Settings and set Use Workbook to false - otherwise it will be defined every time the project loads scripts.

Now you can load the default dictionary into your project. Go to BIG > Workbook - Reload Dictionary.
You can check how the dictionary is defined. To use your own dictionary for this feature, go to Resources/Resources/BIG/Settings and set your own Google Workbook dictionary ID.

It must be publicly available and you can find this ID in your own link like this:

Full link example: https://docs.google.com/spreadsheets/d/1rWbQgslF4K0RKB128MmoDhHlKUQbvL7MD08AdN2twAc/edit?gid=0#gid=0
Id: 1rWbQgslF4K0RKB128MmoDhHlKUQbvL7MD08AdN2twAc

Now you can use <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Workbook/Localization/Translator.cs" target="_blank">`Translator`</a> class or try to add <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/Workbook/Localization/TranslatedText.cs" target="_blank">`TranslatedText`</a> component into your Text Mesh Pro Lable.

Toolbar
---
![Toolbar](https://github.com/user-attachments/assets/222b5fda-ea5c-4efc-af25-e291e6ada4ae)

Based on <a href="https://github.com/marijnz/unity-toolbar-extender" target="_blank">`Unity Toolbar Extender`</a> I prepared flexible solution that you can extend.
There are two examples 
* <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Editor/Toolbar/ToolbarElements/ScenesDropdownToolbar.cs" target="_blank">`Scene Selection Dropdown`</a>
* <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Editor/Toolbar/ToolbarElements/SettingsShortcutToolbar.cs" target="_blank">`Settings Shortcut`</a>.

Based on that, using <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Editor/Toolbar/ToolbarElementAttribute.cs" target="_blank">`Toolbar Element Attribute`</a> you can prepare your own extensions.

