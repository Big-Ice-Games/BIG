#if UNITY_EDITOR

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using UnityEditor;
using UnityEngine;

namespace BIG.Editor.Toolbar.ToolbarElements
{
    /// <summary>
    /// This class provides a quick shortcut toolbar element for selecting settings in the project.
    /// It's selecting Assets/Resources/BIG/Settings.asset file and focus Project Browser and Inspector Window.
    /// It's a great example of how to extend the Unity editor's functionality using custom toolbar elements.
    /// These elements are drawn by <see cref="TopToolBar"/> class.
    /// </summary>
    public static class SettingsShortcutToolbar
    {
        [ToolbarElement(true, 1)]
        private static void Draw()
        {
            var tex = EditorGUIUtility.IconContent(@"Audio Mixer@2x").image;
            GUI.changed = false;
            GUILayout.Button(
                new GUIContent(null, tex, "Focus settings"),
                new GUIStyle()
                {
                    fixedHeight = 20,
                    fixedWidth = 20
                }
            );

            if (GUI.changed)
            {
                // Set focus on ProjectBrowser tab.
                System.Type projectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
                if (projectBrowserType != null)
                {
                    EditorWindow projectWindow = EditorWindow.GetWindow(projectBrowserType);
                    projectWindow.Focus();
                }

                // Ping settings object.
                var obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/BIG/Settings.asset", typeof(Settings));
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;

                // Set focus on InspectorBrowser tab.
                System.Type inspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
                if (inspectorWindowType != null)
                {
                    EditorWindow inspectorWindow = EditorWindow.GetWindow(inspectorWindowType);
                    inspectorWindow.Focus();
                }
            }
        }
    }
}
#endif