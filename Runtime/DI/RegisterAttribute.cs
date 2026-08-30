using System;
using JetBrains.Annotations;

namespace BIG
{
    /// <summary>
    /// Marks a type for automatic registration in the DI container (instantiated by Autofac, not by user code).
    /// </summary>
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RegisterAttribute : Attribute
    {
        public bool Singleton { get; }

        public RegisterAttribute(bool singleton = false)
        {
            Singleton = singleton;
        }
    }
}
