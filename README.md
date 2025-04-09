# BIG

Base Library used among all Big Ice Games projects.

## Installation

Go to Window > Package Manager > Click plus `+` sign button in the left upper corner and select `Install Package from git URL`.
Use this repository git url to download it : https://github.com/Big-Ice-Games/BIG.git.

## <a href="https://github.com/Big-Ice-Games/BIG/tree/main/Runtime/DI" target="_blank">`DI`</a>
Dependency injection is the backbone of Big Ice Games projects.

First you are starting with <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/AssemblyModule.cs" target="_blank">`AssemblyModule`</a> definition.
For every assembly you can define one or more modules that will register injectable classes into your application.

To make sure we avoid dynamic compilation that can occure with <a href="https://github.com/autofac/Autofac" target="_blank">`Autofac`</a>, we use a 
"manual" registration approach. 

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

Application entry happening when you invoke <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Runtime/DI/God.cs" target="_blank">`CreateWorld`</a> in a God class.

You can use flexible chain creation like this:

```
using Autofac;
using BIG;
using UnityEngine;

public sealed class Initialization : MonoBehaviour
{
    [SerializeField] private Settings _settings;
    private void Awake()
    {
        God.Ask()
            .WithLogger(new UnityLogger())
            .WithAssemblyModule(new MyCustomAssemblyModule(_settings))
            .WithStandaloneRegistration().CreateWorld();

        // Here you have your settings resolved from the container
        var usageExample = God.PrayFor<ISettings>();
    }
    
    private sealed class MyCustomAssemblyModule : AssemblyModule
    {
        private readonly ISettings _settings;
        internal MyCustomAssemblyModule(ISettings settings)
        {
            _settings = settings;
        }
        public override void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c => _settings)
                .As<ISettings>()
                .Keyed<object>(typeof(ISettings).FullName)
                .SingleInstance();
        }
    }
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

## Toolbar
Based on <a href="https://github.com/marijnz/unity-toolbar-extender" target="_blank">`Unity Toolbar Extender`</a> I prepared flexible solution that you can extend.
There are two examples <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Editor/Toolbar/ToolbarElements/ScenesDropdownToolbar.cs" target="_blank">`Scene Selection Dropdown`</a> and <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Editor/Toolbar/ToolbarElements/SettingsShortcutToolbar.cs" target="_blank">`Settings Shortcut`</a>.

Based on that, using <a href="https://github.com/Big-Ice-Games/BIG/blob/main/Editor/Toolbar/ToolbarElementAttribute.cs" target="_blank">`Toolbar Element Attribute`</a> you can prepare your own extensions.

