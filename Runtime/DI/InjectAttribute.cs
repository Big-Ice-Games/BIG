using System;

namespace BIG
{
    /// <summary>
    /// Attribute to decorate fields and properties that can be injected directed without constructor.
    /// It's designed for Unity's game objects and <see cref="RuntimeDependencyProvider"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute { }
}
