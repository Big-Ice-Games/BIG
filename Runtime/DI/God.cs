#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            ILogger logger)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.Register(c => logger)
                .As<ILogger>()
                .Keyed<object>(typeof(ILogger).FullName).SingleInstance();

            StandaloneRegistration(builder);

            _container = builder.Build();

            Logger.InitLogger(God.PrayFor<ILogger>());
            typeof(God).Log("World created.", LogLevel.Editor);
        }

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
            AssertRequiredRegistrations();

            // Initiate static Logger with ILogger implementation provided by end user.
            Logger.InitLogger(God.PrayFor<ILogger>());
            typeof(God).Log($"World created - {modules.Count} modules registered.", LogLevel.Editor);
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
                _container?.Log($"[GOD] cannot resolve type: {typeof(T).FullName}\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
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
                type.Log($"[GOD] cannot resolve type: {type.FullName}\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
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

        /// <summary>
        /// Make sure that all the classes marked with <see cref="RegistrationRequiredAttribute"/> are registered.
        /// </summary>
        private static void AssertRequiredRegistrations()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> typesToRegister;
                try
                {
                    typesToRegister = assembly.GetTypes().Where(t => t.IsDefined(typeof(RegistrationRequiredAttribute)));
                }
                catch
                {
                    // Ignore types loading exception
                    continue;
                }

                foreach (Type t in typesToRegister)
                {
                    try
                    {
                        var instance = PrayFor(t);
                        if (instance == null)
                            throw new Exception("Type not registered. Resolved null instance.");
                    }
                    catch (Exception e)
                    {
                        _container!.Log($"{t.FullName} not implemented. You need to register this type before other dependencies are resolved.", LogLevel.Info);
                        _container!.Log($"{t.FullName} can't be resolved.\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Fully automated registration. IDisposable is added manually because sometimes this interface is ignored by AsImplementedInterfaces function.
        /// This registration is not supported on mobile devices because resolving such instance is done by dynamic constructor compilation.
        /// </summary>
        private static void StandaloneRegistration(ContainerBuilder builder)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> typesToRegister;
                try
                {
                    typesToRegister = assembly.GetTypes().Where(t => t.IsDefined(typeof(RegisterAttribute)));
                }
                catch
                {
                    // Ignore types loading exception
                    continue;
                }

                foreach (Type t in typesToRegister)
                {
                    bool singletone = t.GetCustomAttribute<RegisterAttribute>(true).Singletone;
                    if (singletone)
                    {
                        if (t.GetInterfaces().Contains(typeof(IDisposable)))
                        {
                            builder.RegisterType(t)
                                .AsSelf()
                                .AsImplementedInterfaces()
                                .As<IDisposable>()
                                .Keyed<object>(t.FullName)
                                .SingleInstance();
                        }
                        else
                        {
                            builder.RegisterType(t)
                                .AsSelf()
                                .AsImplementedInterfaces()
                                .Keyed<object>(t.FullName)
                                .SingleInstance();
                        }
                    }
                    else
                    {
                        if (t.GetInterfaces().Contains(typeof(IDisposable)))
                        {
                            builder.RegisterType(t)
                                .AsSelf()
                                .AsImplementedInterfaces()
                                .As<IDisposable>()
                                .Keyed<object>(t.FullName);
                        }
                        else
                        {
                            builder.RegisterType(t)
                                .AsSelf()
                                .AsImplementedInterfaces()
                                .Keyed<object>(t.FullName);
                        }
                    }
                }
            }
        }
    }
}