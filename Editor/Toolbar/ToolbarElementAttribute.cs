#if UNITY_EDITOR
// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;

namespace BIG.Editor.Toolbar
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ToolbarElementAttribute : Attribute
    {
        public ToolbarElementAttribute(bool rightSide, int priority = 10)
        {
            RightSide = rightSide;
            Priority = priority;
        }

        /// <summary>
        /// If true the element will be added to the right toolbar.
        /// Otherwise, it will be added to the left toolbar.
        /// </summary>
        public bool RightSide { get; }
        
        /// <summary>
        /// Drawing elements we sort them by priority.
        /// On the right side from left to right.
        /// On the left side from right to left.
        /// </summary>
        public int Priority { get; }
    }
}
#endif