#if UNITY_EDITOR

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Linq;
using UnityEditor;

namespace BIG.Editor.Toolbar
{
    [InitializeOnLoad]
    public static class TopToolBar
    {
        static TopToolBar()
        {
            FindAndAttachToolbarElements();
        }

        private static void FindAndAttachToolbarElements()
        {
            var allElements = ReflectionExtension.FindStaticMethodsWithAttribute<ToolbarElementAttribute>().ToList();
            var rightElements = allElements
                .Where(s => s.Attribute.RightSide)
                .OrderBy(s => s.Attribute.Priority);
            
            var leftElements = allElements
                .Where(s => s.Attribute.RightSide == false)
                .OrderBy(s => s.Attribute.Priority);

            foreach (var rightElement in rightElements)
            {
                Action drawMethod = (Action)Delegate.CreateDelegate(typeof(Action), rightElement);
                ToolbarExtender.RightToolbarGUI.Add(drawMethod);
            }
           
            foreach (var rightElement in leftElements)
            {
                Action drawMethod = (Action)Delegate.CreateDelegate(typeof(Action), rightElement);
                ToolbarExtender.LeftToolbarGUI.Add(drawMethod);
            }
        }
    }
}
#endif