#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BIG.Editor.Toolbar
{
    /// <summary>
    /// Based on https://github.com/marijnz/unity-toolbar-extender.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        private const float SPACE = 8;
        private const float BUTTON_WIDTH = 32;
        private const float DROPDOWN_WIDTH = 80;
        private const float PLAY_PAUSE_STOP_WIDTH = 140;
        
        private static readonly int TOOL_COUNT;
        static GUIStyle _commandStyle = null;
        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();

        static ToolbarExtender()
        {
            Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            string fieldName = "k_ToolCount";
            FieldInfo toolIcons = toolbarType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            TOOL_COUNT = toolIcons != null ? ((int)toolIcons.GetValue(null)) : 8;
            ToolbarCallback.OnToolbarGUI = OnGUI;
            ToolbarCallback.OnToolbarGUILeft = GUILeft;
            ToolbarCallback.OnToolbarGUIRight = GUIRight;
        }
        
        static void OnGUI()
        {
            _commandStyle ??= new GUIStyle("CommandLeft");
            var screenWidth = EditorGUIUtility.currentViewWidth;

            // Following calculations match code reflected from Toolbar.OldOnGUI()
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - PLAY_PAUSE_STOP_WIDTH) / 2);

            Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += SPACE; // Spacing left
            leftRect.xMin += BUTTON_WIDTH * TOOL_COUNT; // Tool buttons
            leftRect.xMin += SPACE; // Spacing between tools and pivot
            leftRect.xMin += 64 * 2; // Pivot buttons
            leftRect.xMax = playButtonsPosition;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += _commandStyle.fixedWidth * 3; // Play buttons
            rightRect.xMax = screenWidth;
            rightRect.xMax -= SPACE; // Spacing right
            rightRect.xMax -= DROPDOWN_WIDTH; // Layout
            rightRect.xMax -= SPACE; // Spacing between layout and layers
            rightRect.xMax -= DROPDOWN_WIDTH; // Layers
            rightRect.xMax -= SPACE; // Spacing between layers and account
            rightRect.xMax -= DROPDOWN_WIDTH; // Account
            rightRect.xMax -= SPACE; // Spacing between account and cloud
            rightRect.xMax -= BUTTON_WIDTH; // Cloud
            rightRect.xMax -= SPACE; // Spacing between cloud and collab
            rightRect.xMax -= 78; // Colab

            // Add spacing around existing controls
            leftRect.xMin += SPACE;
            leftRect.xMax -= SPACE;
            rightRect.xMin += SPACE;
            rightRect.xMax -= SPACE;
            
            leftRect.y = 4;
            leftRect.height = 22;
            rightRect.y = 4;
            rightRect.height = 22;

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in LeftToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in RightToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        private static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in LeftToolbarGUI)
            {
                handler();
            }
            GUILayout.EndHorizontal();
        }

        private static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in RightToolbarGUI)
            {
                handler();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif