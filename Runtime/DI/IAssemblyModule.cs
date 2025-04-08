// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using Autofac;

namespace BIG
{
    /// <summary>
    /// Assembly module is used to register all types that are required to be registered in Dependency Injection container.
    /// Create such module as SCRIPTABLE OBJECT.
    /// Put this object into Resources/Modules folder.
    /// Set Priority (lower number is higher priority).
    /// Inside Register method register all types that are required to be registered.
    /// This module will be automatically registered by God class in <see cref="GameInitializer"/>.
    /// </summary>
    public interface IAssemblyModule
    {
        public int Priority { get; }
        public void Register(ContainerBuilder containerBuilder);
    }
}
