// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;

namespace BIG
{
    /// <summary>
    /// This attribute is used to generate clear information for end-user that something needs to be registered before other <see cref="AssemblyModule"/>s are registered.
    /// Usually the case for it are all the settings that have to be available for other classes to be injected into.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RegistrationRequiredAttribute : Attribute { }
}
