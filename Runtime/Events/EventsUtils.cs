#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Reflection;

namespace BIG.Events
{
    public static class EventsUtils
    {
        public static void SubscribeMyEventHandlers<T>(this T obj)
        {
            var methods = obj.GetMethodsWithAttribute<T, RegisterEventHandlerAttribute>();

            for (int i = 0; i < methods.Length; i++)
            {
                try
                {
                    var attribute = methods[i].GetCustomAttribute<RegisterEventHandlerAttribute>();

                    EventHandler handler = (EventHandler)Delegate.CreateDelegate(
                        typeof(EventHandler), obj, methods[i]);

                    GameEventsManager.Subscribe(attribute.EventType, handler);
                }
                catch (Exception e)
                {
                    throw new Exception($"Cannot subscribe event handler for object {typeof(T).FullName} for method {methods[i].Name}. \n {e}");
                }
            }
        }

        public static void UnSubscribeMyEventHandlers<T>(this T obj)
        {
            var methods = obj.GetMethodsWithAttribute<T, RegisterEventHandlerAttribute>();

            for (int i = 0; i < methods.Length; i++)
            {
                var attribute = methods[i].GetCustomAttribute<RegisterEventHandlerAttribute>();

                EventHandler handler = (EventHandler)Delegate.CreateDelegate(
                    typeof(EventHandler), obj, methods[i]);

                GameEventsManager.Unsubscribe(attribute.EventType, handler);
            }
        }
    }
}