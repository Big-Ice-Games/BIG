using System;
using JetBrains.Annotations;

namespace BIG
{
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Struct |
                    AttributeTargets.Method |
                    AttributeTargets.Property |
                    AttributeTargets.Field |
                    AttributeTargets.Constructor)]
    public sealed class PreserveAttribute : Attribute {}
}
