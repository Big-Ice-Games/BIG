#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BIG.Editor.Toolbar
{
    /// <summary>
    /// Based on https://github.com/marijnz/unity-toolbar-extender
    /// </summary>
    public static class ToolbarCallback
    {
        private static readonly Type TOOLBAR_TYPE = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject _currentToolbar;
        
        public static Action OnToolbarGUI;
        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (_currentToolbar == null)
            {
                // Find toolbar
                var toolbars = Resources.FindObjectsOfTypeAll(TOOLBAR_TYPE);
                _currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (_currentToolbar != null)
                {
                    var root = _currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(_currentToolbar);
                    var mRoot = rawRoot as VisualElement;
                    RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                    RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);

                    void RegisterCallback(string root, Action cb)
                    {
                        var toolbarZone = mRoot.Q(root);

                        var parent = new VisualElement()
                        {
                            style = {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        var container = new IMGUIContainer();
                        container.style.flexGrow = 1;
                        container.onGUIHandler += () => {
                            cb?.Invoke();
                        };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
                }
            }
        }

        private static void OnGUI()
        {
            var handler = OnToolbarGUI;
            if (handler != null) handler();
        }
    }
}
#endif