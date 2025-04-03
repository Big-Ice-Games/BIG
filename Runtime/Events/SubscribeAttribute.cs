#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;

namespace BIG
{
    /// <summary>
    /// Use this attribute to request event handler registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SubscribeAttribute : Attribute
    {
        private const int DEFAULT_PRIORITY = 0;
        public int Priority { get; }

        public SubscribeAttribute(int priority = DEFAULT_PRIORITY)
        {
            Priority = priority;
        }
    }
}
