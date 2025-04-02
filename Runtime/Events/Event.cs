#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;

#pragma warning disable CS8631
#pragma warning disable CS8625

namespace BIG.Events
{
    public abstract class Event : EventArgs
    {
        public string EventIdentifier => GetType()?.FullName ?? string.Empty;
        public bool Consumed { get; set; }
        public void Publish<T>(Type sender = null) where T : Event => Events.Publish(sender, this as T);
    }
}