using System;

namespace BIG
{
    /// <summary>
    /// Use this attribute to register type for automated <see cref="God.WorldCreation"/> without defining <see cref="AssemblyModule"/>s.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class RegisterAttribute : Attribute
    {
        public bool Singletone { get; }

        public RegisterAttribute(bool singletone = false)
        {
            Singletone = singletone;
        }
    }
}
