#if UNITY_EDITOR

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BIG.Editor.Toolbar.ToolbarElements
{
    /// <summary>
    /// This class provides a dropdown toolbar element for switching between scenes in the Unity editor.
    /// It displays the name of the currently active scene and allows the user to select from a list of available scenes that are enabled in the build settings.
    /// It's a great example of how to extend the Unity editor's functionality using custom toolbar elements.
    /// These elements are drawn by <see cref="TopToolBar"/> class.
    /// </summary>
    public static class ScenesDropdownToolbar
    {
        [ToolbarElement(true, 0)]
        public static void Draw()
        {
            GUI.changed = false;

            if (GUILayout.Button(new GUIContent(SceneManager.GetActiveScene().name, "Change Scene"),
                    EditorStyles.toolbarDropDown,
                    GUILayout.Width(100)))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (scene.enabled)
                    {
                        string scenePath = scene.path;
                        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                        menu.AddItem(new GUIContent(sceneName), false, () =>
                        {
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                EditorSceneManager.OpenScene(scenePath);
                            }
                        });
                    }
                }
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Cancel"), false, () => {});
                menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
            }
        }
    }
}
#endif