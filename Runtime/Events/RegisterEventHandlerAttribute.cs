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
        public int Priority { get; }
        public Type EventType { get; }

        public RegisterEventHandlerAttribute(int priority, Type eventType)
        {
            Priority = priority;
            EventType = eventType;
        }
    }
}
