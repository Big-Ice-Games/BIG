#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using Autofac;

namespace BIG
{
    /// <summary>
    /// Assembly module is used to register all types that are required to be registered in Dependency Injection container.
    /// Create such module.
    /// Define all types from this assembly.
    /// Create this module and push it into the <see cref="God"/> CreateWorld function.
    /// </summary>
    public abstract class AssemblyModule
    {
        public abstract void Register(ContainerBuilder containerBuilder);
    }
}
