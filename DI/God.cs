using System;
using System.Collections.Generic;
using Autofac;

namespace BIG
{
    /// <summary>
    /// This class is the only DI container holder to resolve all the registered dependencies.
    /// </summary>
    public class God
    {
        private static IContainer? _container;

        public static void WorldCreation(
            ILogger logger,
            List<AssemblyModule> modules)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.Register(c => logger)
                .As<ILogger>()
                .Keyed<object>(typeof(ILogger).FullName).SingleInstance();

            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Register(builder);
            }

            _container = builder.Build();

            Logger.InitLogger(God.PrayFor<ILogger>());
            typeof(God).Log($"World created - {modules.Count} modules registered.", Category.Default, LogLevel.Editor);
        }

        /// <summary>
        /// Generic types resolver.
        /// </summary>
        /// <typeparam name="T">Type of registered object.</typeparam>
        /// <returns>Instance.</returns>
        public static T PrayFor<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception e)
            {
                _container?.Log($"[GOD] cannot resolve type: {typeof(T).FullName}\n{e.Message}\n{e.StackTrace}", Category.Default, LogLevel.Error);
                return default;
            }
        }

        /// <summary>
        /// Gets registered object instance by type.
        /// This function is used by internal dependency injection automation for game objects. 
        /// For normal case use <see cref="PrayFor{T}"/>.
        /// </summary>
        /// <param name="type">Requested type.</param>
        /// <returns>Instance.</returns>
        public static object PrayFor(Type type)
        {
            try
            {
                return _container.ResolveNamed<object>(type.FullName);
            }
            catch (Exception e)
            {
                type.Log($"[GOD] cannot resolve type: {type.FullName}\n{e.Message}\n{e.StackTrace}", Category.Default, LogLevel.Error);
                return null;
            }
        }

        /// <summary>
        /// Dispose all types registered as IDisposable.
        /// </summary>
        public static void Dispose()
        {
            PrayFor<IList<IDisposable>>().Each(c => c?.Dispose());
        }

    }
}