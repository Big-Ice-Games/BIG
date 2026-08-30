# BIG

Base .NET Standard 2.1 library used among all Big Ice Games projects.

It is engine-independent: the same code runs inside Unity (through the [BIG.Unity](https://github.com/Big-Ice-Games/BIG.Unity) plugin), on game servers and in tools. It is the foundation for subsequent, more specific libraries (BIG.Deterministic, BIG.Client, BIG.Server) and plugins.

* **Dependency Injection** based on [Autofac](https://github.com/autofac/Autofac) — fluent bootstrap, assembly modules, attribute-based registration and field injection designed to work with Unity game objects, but usable in any application layer (server, web service, tools).
* **Struct-based Events** with priorities — raise and subscribe with zero dynamic allocations on the hot path.
* **Deterministic binary serialization** — `Span<byte>`-based, explicit little-endian, zero-allocation writer/reader shared by server and client.
* **Game math types** — vectors (float/int/byte), quaternion — all structs with serialization built in.
* **JSON serialization** for configs and saves, with culture-invariant converters for all BIG types.
* **Central cache registry** — one `Cache.Clear()` to reset all static metadata caches (essential for Unity with domain reload disabled).
* A set of useful extensions for collections, enums, reflection, randomization and more.

## Table of Contents
- [Installation](#installation)
- [Dependency Injection](#dependency-injection)
- [Events](#events)
- [Binary Serialization](#binary-serialization)
- [JSON](#json)
- [Types](#types)
- [Cache](#cache)
- [Logging](#logging)
- [License](#license)

Installation
---
* **Unity** — use the [BIG.Unity](https://github.com/Big-Ice-Games/BIG.Unity) plugin, which ships `BIG.dll` together with the engine-side bootstrap.
* **Server / tools / other .NET** — reference the project or the built `BIG.dll` directly. Build with `dotnet build -c Release`.

Dependency Injection
---
The container is bootstrapped once, through the fluent [`God`](Runtime/DI/God.cs) API:

```csharp
God.Ask()
    .WithLogger(new MyLogger())          // Your ILogger implementation.
    .WithAssemblyModules(modules)        // Modules with manual registrations.
    .WithStandaloneRegistration()        // Auto-register all [Register] types (not supported on mobile IL2CPP).
    .CreateWorld();
```

Types can be registered in three ways:

* [`[Register]`](Runtime/DI/RegisterAttribute.cs) attribute — automatic, reflection-based (desktop/editor only):

```csharp
[Register(singleton: true)]
public sealed class MyService : IMyService { }
```

* [`IAssemblyModule`](Runtime/DI/IAssemblyModule.cs) — manual registration, works on every platform (HIGHER priority registers EARLIER, default 0):

```csharp
public sealed class MyModule : IAssemblyModule
{
    public int Priority => 0;

    public void Register(ContainerBuilder containerBuilder)
    {
        containerBuilder.Register(c => new MyService())
            .As<IMyService>()
            .Keyed<object>(typeof(MyService).FullName) // Required for [Inject] field injection.
            .SingleInstance();
    }
}
```

* Directly on the builder inside any module.

Resolve through `God.PrayFor<T>()`, or inject into fields and properties with [`[Inject]`](Runtime/DI/InjectAttribute.cs) + [`RuntimeDependencyProvider`](Runtime/DI/RuntimeDependencyProvider.cs):

```csharp
public class Example
{
    [Inject] private IMyService _service;

    public Example() => this.ResolveMyDependencies();
}
```

Reflection metadata is cached per type, so injecting hundreds of instances of the same type is cheap.

Events
---
[`Events`](Runtime/Events.cs) is a simple struct-based event bus. Convention used across the whole BIG ecosystem: **HIGHER priority executes EARLIER, default is 0**.

```csharp
public readonly struct DamageDealt
{
    public DamageDealt(int amount) => Amount = amount;
    public readonly int Amount;
}

// Publisher:
Events.Raise(new DamageDealt(10));

// Manual subscription:
Events.Subscribe<DamageDealt>(priority: 0, OnDamageDealt);
Events.Unsubscribe<DamageDealt>(OnDamageDealt);
```

Or subscribe declaratively with the [`[Subscribe]`](Runtime/Events.cs) attribute — `obj.Subscribe()` / `obj.Unsubscribe()` extension methods handle all decorated methods (method metadata is cached per type):

```csharp
public class HealthView
{
    [Subscribe(priority: 5)]
    private void OnDamageDealt(DamageDealt e) { /* runs before default-priority handlers */ }
}
```

Binary Serialization
---
Deterministic, allocation-free serialization for networking — the same bytes on every platform (explicit little-endian, floats written as raw IEEE 754 bits). Implement [`ISerializable`](Runtime/Serialization/ISerializable.cs) directly on your structs:

```csharp
public partial struct PlayerState : ISerializable
{
    public FloatVector3 Position;
    public FloatQuaternion Rotation;

    public int SerializedSize => FloatVector3.SERIALIZED_SIZE + FloatQuaternion.SERIALIZED_SIZE;

    public void Serialize(ref ByteWriter writer)
    {
        Position.Serialize(ref writer);
        Rotation.Serialize(ref writer);
    }

    public void Deserialize(ref ByteReader reader)
    {
        Position.Deserialize(ref reader);
        Rotation.Deserialize(ref reader);
    }
}

// Usage — no boxing, no allocation:
Span<byte> buffer = stackalloc byte[state.SerializedSize];
int written = Serializer.Serialize(state, buffer);
var restored = Serializer.Deserialize<PlayerState>(buffer);
```

All BIG types (vectors, quaternion) implement `ISerializable` out of the box. [`ByteWriter`](Runtime/Serialization/ByteWriter.cs)/[`ByteReader`](Runtime/Serialization/ByteReader.cs) also handle primitives, UTF-8 strings and raw byte spans.

JSON
---
For configs, saves and other non-realtime data use [`Json`](Runtime/Serialization/Json.cs):

```csharp
string json = myConfig.SerializeJson();
var config = json.DeserializeJson<MyConfig>();
```

Every BIG type serializes to a compact, culture-invariant string (`"{1.5, -2.25}"`). Each type carries its own converter as a nested `JsonConverter` class discovered through the `[JsonConverter]` attribute — libraries built on BIG add their own types the same way, with zero registration. For third-party types that cannot be decorated, register a converter once at bootstrap with `Json.RegisterConverter(...)`.

Types
---
All types are structs with `StructLayout.Sequential`, `IEquatable<T>`, math operations and built-in serialization:

* [`FloatVector2`](Runtime/Types/Vectors/FloatVector2.cs), [`FloatVector3`](Runtime/Types/Vectors/FloatVector3.cs) — Dot, Cross, Distance, Lerp, safe Normalized...
* [`IntVector2`](Runtime/Types/Vectors/IntVector2.cs), [`IntVector3`](Runtime/Types/Vectors/IntVector3.cs)
* [`ByteVector2`](Runtime/Types/Vectors/ByteVector2.cs), [`ByteVector3`](Runtime/Types/Vectors/ByteVector3.cs), [`ByteVector4`](Runtime/Types/Vectors/ByteVector4.cs) — usable as colors (R/G/B/A aliases, color Lerp).
* [`FloatQuaternion`](Runtime/Types/FloatQuaternion.cs) — radians, Unity-compatible euler order (Z→X→Y), Slerp/Nlerp, vector rotation.
* [`Direction`](Runtime/Types/Direction.cs) — 4-directional grid helper.

Cache
---
Static metadata caches (reflection, enums) across the whole ecosystem register themselves in [`Cache`](Runtime/Utils/Cache.cs):

```csharp
Cache.Clear();              // One call resets all registered caches — call it on game start.
Cache.Register(name, action); // Register your own cache from any library built on BIG.
Cache.ValidateCoverage();   // Dev-time check: finds ClearCache() methods that were never registered.
```

`Cache.Clear()` is safe to call at any time (caches repopulate lazily). It deliberately does NOT touch state — resetting event subscriptions is a separate, explicit bootstrap step (`Events.Clear()`).

Logging
---
Implement [`ILogger`](Runtime/ILogger.cs) once, register it via `God.WithLogger(...)`, and log from anywhere through extension methods:

```csharp
this.Log("Hello");
this.LogWarning("Careful");
this.LogError("Boom", withStackTrace: true);
```

License
---
MIT — see [LICENSE](LICENSE). Redistributed third-party libraries (Autofac, Newtonsoft.Json — both MIT) are covered in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
