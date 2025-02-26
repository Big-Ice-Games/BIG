#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;

namespace BIG.Events
{
    /// <summary>
    /// Use this attribute to request event handler registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RegisterEventHandlerAttribute : Attribute
    {
        public Type EventType { get; }

        public RegisterEventHandlerAttribute(Type eventType)
        {
            EventType = eventType;
        }
    }
}
