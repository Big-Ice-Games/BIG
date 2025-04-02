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
        /// <summary>
        /// Register all event handlers from object that has RegisterEventHandlerAttribute.
        /// </summary>
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

                    Events.Subscribe(attribute.EventType, attribute.Priority, handler);
                }
                catch (Exception e)
                {
                    throw new Exception($"Cannot subscribe event handler for object {typeof(T).FullName} for method {methods[i].Name}. \n {e}");
                }
            }
        }

        /// <summary>
        /// Unsubscribe all event handlers from object that has RegisterEventHandlerAttribute.
        /// </summary>
        public static void UnSubscribeMyEventHandlers<T>(this T obj)
        {
            var methods = obj.GetMethodsWithAttribute<T, RegisterEventHandlerAttribute>();

            for (int i = 0; i < methods.Length; i++)
            {
                var attribute = methods[i].GetCustomAttribute<RegisterEventHandlerAttribute>();

                EventHandler handler = (EventHandler)Delegate.CreateDelegate(
                    typeof(EventHandler), obj, methods[i]);

                Events.Unsubscribe(attribute.EventType, handler);
            }
        }
    }
}