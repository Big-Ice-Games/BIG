using System;
using System.Collections.Generic;
using System.Reflection;

namespace BIG
{
    public static class EventsUtils
    {
        private static readonly Dictionary<object, List<(Type type, Delegate handler)>> _subscriptions = new();
        public static void Subscribe<T>(this T obj)
        {
            var methods = obj.GetMethodsWithAttribute<T, SubscribeAttribute>();

            if (!_subscriptions.TryGetValue(obj, out var cache))
            {
                cache = new List<(Type, Delegate)>(methods.Length);
                _subscriptions[obj] = cache;
            }

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<SubscribeAttribute>();
                var parameters = method.GetParameters();

                if (parameters.Length != 1)
                    throw new InvalidOperationException($"Method {method.Name} must have exactly one parameter.");

                var eventType = parameters[0].ParameterType;
                var delegateType = typeof(Action<>).MakeGenericType(eventType);
                var handler = Delegate.CreateDelegate(delegateType, obj, method); // Create delegate Action<T>
                SubscribeViaReflection(eventType, attr.Priority, handler);
                cache.Add((eventType, handler));
            }
        }

        public static void Unsubscribe<T>(this T obj)
        {
            if (!_subscriptions.TryGetValue(obj, out var cache))
                return;

            foreach (var (eventType, handler) in cache)
            {
                var method = typeof(Events)
                    .GetMethod(nameof(Events.Unsubscribe))!
                    .MakeGenericMethod(eventType);

                method.Invoke(null, new object[] { handler });
            }

            _subscriptions.Remove(obj);
        }

        private static void SubscribeViaReflection(Type eventType, int priority, Delegate handler)
        {
            var method = typeof(Events)
                .GetMethod(nameof(Events.Subscribe), BindingFlags.Public | BindingFlags.Static)!;

            var generic = method.MakeGenericMethod(eventType);
            generic.Invoke(null, new object[] { priority, handler });
        }
    }
}
