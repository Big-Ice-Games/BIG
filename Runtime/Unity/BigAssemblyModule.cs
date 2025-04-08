// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using Autofac;
using UnityEngine;

namespace BIG
{
    [CreateAssetMenu(fileName = "BigAssemblyModule", menuName = "BIG/BigAssemblyModule")]
    public sealed class BigAssemblyModule : ScriptableObject, IAssemblyModule
    {
        [field: SerializeField] public int Priority { get; }
        public Settings Settings;
        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(
                    c => new MainThreadActionsQueue())
                .As<MainThreadActionsQueue>()
                .Keyed<object>(typeof(MainThreadActionsQueue).FullName)
                .SingleInstance();
            
            containerBuilder.Register(s => Settings)
                .As<ISettings>()
                .As<Settings>()
                .Keyed<object>(typeof(ISettings).FullName)
                .Keyed<object>(typeof(Settings).FullName)
                .SingleInstance();
        }
    }
}
