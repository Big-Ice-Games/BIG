namespace BIG
{
    /// <summary>
    /// Assembly module is used to register all types that are required to be registered in Dependency Injection container.
    /// Create such module as SCRIPTABLE OBJECT.
    /// Put this object into Resources/Modules folder.
    /// Set Priority — HIGHER priority registers EARLIER (BIG-wide convention, default 0).
    /// Inside Register method register all types that are required to be registered.
    /// </summary>
    public interface IAssemblyModule
    {
        public int Priority { get; }
        public void Register(Autofac.ContainerBuilder containerBuilder);
    }
}
