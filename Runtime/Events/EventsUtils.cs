#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace BIG
{
    public static class EventsUtils
    {
        private static readonly Dictionary<object, List<(string key, Action<object> handler)>> _subscriptions = new();

        public static void Subscribe<T>(this T obj)
        {
            var methods = obj.GetMethodsWithAttribute<T, SubscribeAttribute>();

            if (!_subscriptions.TryGetValue(obj, out var cache))
            {
                cache = new List<(string key, Action<object> handler)>(methods.Length);
                _subscriptions[obj] = cache;
            }

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<SubscribeAttribute>();
                var param = method.GetParameters();
                if (param.Length != 1)
                    throw new InvalidOperationException($"Method {method.Name} must have exactly one parameter.");

                var eventParamType = param[0].ParameterType;
                var key = eventParamType.FullName;
                
                Action<object> handler = (evt) =>
                {
                    if (evt == null || !eventParamType.IsInstanceOfType(evt)) return;
                    method.Invoke(obj, new object[] { evt });
                };

                Events.Subscribe(key, attr.Priority, handler);
                cache.Add((key, handler));
            }
        }

        public static void Unsubscribe<T>(this T obj)
        {
            if (!_subscriptions.TryGetValue(obj, out var cache))
                return;

            foreach (var (key, handler) in cache)
            {
                Events.Unsubscribe(key, handler);
            }

            _subscriptions.Remove(obj);
        }
    }

}
