#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using Autofac;
using UnityEngine;

namespace BIG
{
    [CreateAssetMenu(fileName = "BigAssemblyModule", menuName = "BIG/BigAssemblyModule")]
    public sealed class BigAssemblyModule : ScriptableObject, IAssemblyModule
    {
        [field: SerializeField] public int Priority { get; }
        [field: SerializeField] private Settings _settings;
        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(
                    c => new MainThreadActionsQueue())
                .As<MainThreadActionsQueue>()
                .Keyed<object>(typeof(MainThreadActionsQueue).FullName)
                .SingleInstance();

            containerBuilder.Register(s => _settings)
                .As<ISettings>()
                .As<Settings>()
                .Keyed<object>(typeof(ISettings).FullName)
                .Keyed<object>(typeof(Settings).FullName)
                .SingleInstance();
        }
    }
}
