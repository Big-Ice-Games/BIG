// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace BIG
{
    /// <summary>
    /// This class is the only DI container holder to resolve all the registered dependencies.
    /// </summary>
    public sealed class God
    {
        private static God _instance;
        private static God Instance => _instance ??= new God();
        private readonly ContainerBuilder _builder;
        private IContainer _container;

        private int _modules;
        private int _automaticRegistration;
        private bool _logger;

        private God()
        {
            _builder = new ContainerBuilder();
        }

        public static God Ask() => Instance;

        public God WithStandaloneRegistration()
        {
            Instance.StandaloneRegistration();
            return _instance;
        }

        public God WithLogger(ILogger logger)
        {
            Instance._builder.Register(c => logger)
                .As<ILogger>()
                .Keyed<object>(typeof(ILogger).FullName!).SingleInstance();

            Logger.InitLogger(logger);
            Instance._logger = true;
            return _instance;
        }

        public God WithAssemblyModule(IAssemblyModule assemblyModule)
        {
            assemblyModule.Register(Instance._builder);
            Instance._modules++;
            return _instance;
        }

        public God WithAssemblyModules(IList<IAssemblyModule> assemblyModules)
        {
            assemblyModules.OrderBy(s => s.Priority).Each(s =>
            {
                Instance._modules++;
                s.Register(Instance._builder);
            });
            return _instance;
        }

        public God CreateWorld()
        {
            if (Instance._container != null)
            {
                throw new Exception("[GOD] World already created. You cannot create it again.");
            }

            Instance._container = Instance._builder.Build();

            try
            {
                // If there is no logger included then this will throw an exception.
                ILogger logger = PrayFor<ILogger>();
                Instance.Log($"World created:\n" +
                             (Instance._logger
                                 ? "<color=green>Logger Assigned</color>\n"
                                 : "<color=red>Logger Unassigned</color>\n") +
                             $"Modules registered: {Instance._modules}\n" +
                             $"Types registered automatically: {Instance._automaticRegistration}");
            }
            catch
            {
                // Ignore if user decide to create world without logger.
            }

            return Instance;
        }


        /// <summary>
        /// Generic types' resolver.
        /// </summary>
        /// <typeparam name="T">Type of registered object.</typeparam>
        /// <returns>Instance.</returns>
        public static T PrayFor<T>()
        {
            try
            {
                return Instance._container.Resolve<T>();
            }
            catch (Exception e)
            {
                var msg = $"[GOD] cannot resolve type: {typeof(T).FullName}\n{e.Message}\n{e.StackTrace}";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Generic types' resolver for non-static usage.
        /// </summary>
        /// <typeparam name="T">Type of registered object.</typeparam>
        /// <returns>Instance.</returns>
        public T Ask<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception e)
            {
                var msg = $"[GOD] cannot resolve type: {typeof(T).FullName}\n{e.Message}\n{e.StackTrace}";
                throw new Exception(msg);
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
                return Instance._container!.ResolveNamed<object>(type.FullName!);
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
            Instance._container.Dispose();
            _instance = null;
        }

        /// <summary>
        /// Fully automated registration. IDisposable is added manually because sometimes this interface is ignored by AsImplementedInterfaces function.
        /// This registration is not supported on mobile devices because resolving such instance is done by dynamic constructor compilation.
        /// </summary>
        private void StandaloneRegistration()
        {
            foreach (TypeWithAttribute<RegisterAttribute> t in ReflectionExtension.GetAllTypesByAttribute<RegisterAttribute>())
            {
                _automaticRegistration++;
                bool singletone = t.Attribute.Singletone;
                if (singletone)
                {
                    if (t.Type.GetInterfaces().Contains(typeof(IDisposable)))
                    {
                        var build = _builder.RegisterType(t)
                            .AsSelf()
                            .AsImplementedInterfaces()
                            .As<IDisposable>() // I notice that sometimes this interface is ignored by AsImplementedInterfaces function.
                            .Keyed<object>(t.Type.FullName!)
                            .SingleInstance();

                        // Use every interface as a key too.
                        t.Type.GetInterfaces().Each(s => build = build.Keyed<object>(s.FullName!));
                    }
                    else
                    {
                        var build = _builder.RegisterType(t)
                            .AsSelf()
                            .AsImplementedInterfaces()
                            .Keyed<object>(t.Type.FullName!)
                            .SingleInstance();

                        // Use every interface as a key too.
                        t.Type.GetInterfaces().Each(s => build = build.Keyed<object>(s.FullName!));
                    }
                }
                else
                {
                    if (t.Type.GetInterfaces().Contains(typeof(IDisposable)))
                    {
                        var build = _builder.RegisterType(t)
                            .AsSelf()
                            .AsImplementedInterfaces()
                            .As<IDisposable>() // I notice that sometimes this interface is ignored by AsImplementedInterfaces function.
                            .Keyed<object>(t.Type.FullName!);

                        // Use every interface as a key too.
                        t.Type.GetInterfaces().Each(s => build = build.Keyed<object>(s.FullName!));
                    }
                    else
                    {
                        var build = _builder.RegisterType(t)
                            .AsSelf()
                            .AsImplementedInterfaces()
                            .Keyed<object>(t.Type.FullName!);

                        // Use every interface as a key too.
                        t.Type.GetInterfaces().Each(s => build = build.Keyed<object>(s.FullName!));
                    }
                }
            }
        }
    }
}