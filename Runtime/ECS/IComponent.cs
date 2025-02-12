#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion
// ReSharper disable InvalidXmlDocComment
namespace BIG
{
    /// <summary>
    /// Abstract component. Developer can design up to 64 component types.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Component type have to be ulong enum value.
        /// <example>
        /// <code>
        /// [Flags]
        /// public enum ComponentType : ulong
        /// {
        ///    None = 0,
        ///    Transform = 1UL << 0,  // 1
        ///    Input = 1UL << 1,  // 2
        ///    Health = 1UL << 2,  // 4
        ///    Connection = 1UL << 3,  // 8
        ///    Chunk = 1UL << 4,  // 16
        ///    ...
        ///    Inventory = 1UL << 62, // 2^62
        ///    Target = 1UL << 63  // 2^63
        /// }
        /// </code>
        /// </example>
        /// </summary>
        ulong ComponentType { get; }
    }

    public static class ComponentsUtils
    {
        public static ulong ToFlag(this IComponent[] components)
        {
            ulong flag = 0;

            for (int i = 0; i < components.Length; i++)
            {
                flag |= components[i].ComponentType;
            }

            return flag;
        }

        public static bool Have(this ulong components, IComponent component) => (components & component.ComponentType) == component.ComponentType;
    }
}