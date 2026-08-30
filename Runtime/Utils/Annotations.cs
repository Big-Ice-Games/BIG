using System;

// Minimal embedded copy of JetBrains.Annotations used by BIG attributes.
// Rider/ReSharper match these attributes by their fully qualified name only — visibility does not matter —
// so they stay internal and never conflict with Unity's or NuGet's copies of JetBrains.Annotations.
// Thanks to [MeansImplicitUse] on BIG attributes, Rider knows that e.g. [Inject] fields are assigned
// by reflection and stops suggesting "make readonly" / "field is never assigned".
namespace JetBrains.Annotations
{
    [Flags]
    internal enum ImplicitUseKindFlags
    {
        Default = Access | Assign | InstantiatedWithFixedConstructorSignature,
        /// <summary> Only entity marked with attribute considered used. </summary>
        Access = 1,
        /// <summary> Indicates implicit assignment to a member. </summary>
        Assign = 2,
        /// <summary> Indicates implicit instantiation of a type with fixed constructor signature. </summary>
        InstantiatedWithFixedConstructorSignature = 4,
        /// <summary> Indicates implicit instantiation of a type. </summary>
        InstantiatedNoFixedConstructorSignature = 8,
    }

    [Flags]
    internal enum ImplicitUseTargetFlags
    {
        Default = Itself,
        Itself = 1,
        /// <summary> Members of entity marked with attribute are considered used. </summary>
        Members = 2,
        /// <summary> Inherited entities are considered used. </summary>
        WithInheritors = 4,
        /// <summary> Entity marked with attribute and all its members considered used. </summary>
        WithMembers = Itself | Members,
    }

    /// <summary>
    /// Should be used on attributes and causes ReSharper/Rider to not mark symbols marked with such attributes
    /// as unused (as well as by other usage inspections).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class MeansImplicitUseAttribute : Attribute
    {
        public MeansImplicitUseAttribute()
            : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
            : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

        public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
            : this(ImplicitUseKindFlags.Default, targetFlags) { }

        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
        {
            UseKindFlags = useKindFlags;
            TargetFlags = targetFlags;
        }

        public ImplicitUseKindFlags UseKindFlags { get; }
        public ImplicitUseTargetFlags TargetFlags { get; }
    }
}
