using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace BIG
{
    /// <summary>
    /// Priority convention (used across the whole BIG ecosystem): HIGHER priority executes EARLIER. Default is 0.
    /// If you need to handle an event before the others, subscribe with a priority above 0.
    /// </summary>
    [JetBrains.Annotations.MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SubscribeAttribute : Attribute
    {
        private const int DEFAULT_PRIORITY = 0;
        public int Priority { get; }
        public SubscribeAttribute(int priority = DEFAULT_PRIORITY)
        {
            Priority = priority;
        }
    }

    internal readonly struct EventSubscriber
    {
        public readonly int Priority;
        public readonly Delegate Handler;

        public EventSubscriber(int priority, Delegate handler)
        {
            Priority = priority;
            Handler = handler;
        }

        public void Invoke<T>(T data) where T : struct
        {
            if (Handler is Action<T> action)
                action(data);
        }
    }

    public static class Events
    {
        private static readonly Dictionary<Type, List<EventSubscriber>> SUBSCRIBERS = new Dictionary<Type, List<EventSubscriber>>();

        /// <summary>
        /// Full event system STATE reset — drops every subscriber, including attribute-based subscription tracking
        /// in <see cref="EventsUtils"/> (so objects can subscribe again from scratch).
        /// This changes behavior, so it is a deliberate bootstrap/teardown step — intentionally NOT part of <see cref="Cache.Clear"/>,
        /// which must stay safe to call at any time (caches repopulate lazily, dropped subscribers would not).
        /// </summary>
        public static void Clear()
        {
            SUBSCRIBERS.Clear();
            EventsUtils.ClearSubscriptions();
        }

        /// <summary>
        /// Subscribe handler for T events. HIGHER priority executes EARLIER; default is 0.
        /// Subscribers with equal priority execute in subscription order.
        /// </summary>
        public static void Subscribe<T>(int priority, Action<T> handler) where T : struct
        {
            var type = typeof(T);

            if (!SUBSCRIBERS.TryGetValue(type, out var list))
            {
                list = new List<EventSubscriber>(4);
                SUBSCRIBERS[type] = list;
            }

            foreach (var sub in list)
            {
                if (sub.Handler.Equals(handler))
                    return; // already subscribed
            }

            var subscriber = new EventSubscriber(priority, handler);

            // List is kept sorted descending by priority, so Raise executes the highest priority first.
            int insertAt = list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                if (subscriber.Priority > list[i].Priority)
                {
                    insertAt = i;
                    break;
                }
            }

            list.Insert(insertAt, subscriber);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (!SUBSCRIBERS.TryGetValue(type, out var list))
                return;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Handler.Equals(handler))
                {
                    list.RemoveAt(i);
                    if (list.Count == 0)
                        SUBSCRIBERS.Remove(type);
                    return;
                }
            }
        }

        public static void Raise<T>(T data) where T : struct
        {
            if (!SUBSCRIBERS.TryGetValue(typeof(T), out var list))
                return;

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Invoke(data);
            }
        }

        /// <summary>
        /// Raise event, but it will be handled only by one subscriber with the highest priority.
        /// If more than one subscriber has that priority, the first subscribed wins.
        /// </summary>
        public static void RaiseOnlyToHighestPriority<T>(T data) where T : struct
        {
            if (!SUBSCRIBERS.TryGetValue(typeof(T), out var list))
                return;

            // List is kept sorted descending by priority, so the highest priority is at the front.
            list[0].Invoke(data);
        }

        /// <summary>
        /// Raise event, but it will be handled only by one subscriber with the lowest priority.
        /// If more than one subscriber has that priority, the first subscribed wins.
        /// </summary>
        public static void RaiseOnlyToLowestPriority<T>(T data) where T : struct
        {
            if (!SUBSCRIBERS.TryGetValue(typeof(T), out var list))
                return;

            // List is kept sorted descending by priority, so the lowest priority is at the end.
            int index = list.Count - 1;
            int lowest = list[index].Priority;
            while (index > 0 && list[index - 1].Priority == lowest)
                index--;

            list[index].Invoke(data);
        }
    }

    public static class EventsUtils
    {
        private static readonly Dictionary<object, List<(Type type, Delegate handler)>> SUBSCRIPTIONS = new Dictionary<object, List<(Type type, Delegate handler)>>();

        private static readonly MethodInfo SUBSCRIBE_METHOD =
            typeof(Events).GetMethod(nameof(Events.Subscribe), BindingFlags.Public | BindingFlags.Static)!;
        private static readonly MethodInfo UNSUBSCRIBE_METHOD =
            typeof(Events).GetMethod(nameof(Events.Unsubscribe), BindingFlags.Public | BindingFlags.Static)!;

        private readonly struct SubscriberMethod
        {
            public readonly MethodInfo Method;
            public readonly Type EventType;
            public readonly Type DelegateType;
            public readonly MethodInfo ClosedSubscribe;
            public readonly int Priority;

            public SubscriberMethod(MethodInfo method, Type eventType, Type delegateType, MethodInfo closedSubscribe, int priority)
            {
                Method = method;
                EventType = eventType;
                DelegateType = delegateType;
                ClosedSubscribe = closedSubscribe;
                Priority = priority;
            }
        }

        /// <summary>
        /// Reflection metadata cached per type — objects are subscribed on every enable (pooling!),
        /// while the set of [Subscribe] methods never changes per type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, SubscriberMethod[]> METHODS_CACHE = new ConcurrentDictionary<Type, SubscriberMethod[]>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> UNSUBSCRIBE_CACHE = new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        /// Clear cached reflection metadata (registered in <see cref="Cache"/>). Active subscriptions are not touched.
        /// </summary>
        public static void ClearCache()
        {
            METHODS_CACHE.Clear();
            UNSUBSCRIBE_CACHE.Clear();
        }

        /// <summary>
        /// Drop attribute-based subscription tracking. Called by <see cref="Events.Clear"/> — must stay in sync with it,
        /// otherwise idempotent Subscribe would refuse to re-subscribe objects after a state reset.
        /// </summary>
        internal static void ClearSubscriptions() => SUBSCRIPTIONS.Clear();

        private static SubscriberMethod[] GetSubscriberMethods(Type type)
        {
            if (METHODS_CACHE.TryGetValue(type, out var cached))
                return cached;

            var allMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            int count = 0;
            for (int i = 0; i < allMethods.Length; i++)
            {
                if (allMethods[i].IsDefined(typeof(SubscribeAttribute), false))
                    count++;
            }

            var result = count == 0 ? Array.Empty<SubscriberMethod>() : new SubscriberMethod[count];
            for (int i = 0, s = 0; i < allMethods.Length; i++)
            {
                var method = allMethods[i];
                if (!method.IsDefined(typeof(SubscribeAttribute), false))
                    continue;

                var attr = method.GetCustomAttribute<SubscribeAttribute>()!;
                var parameters = method.GetParameters();

                if (parameters.Length != 1)
                    throw new InvalidOperationException($"Method {type.Name}.{method.Name} marked with [Subscribe] must have exactly one parameter.");

                var eventType = parameters[0].ParameterType;
                var delegateType = typeof(Action<>).MakeGenericType(eventType);
                result[s++] = new SubscriberMethod(method, eventType, delegateType, SUBSCRIBE_METHOD.MakeGenericMethod(eventType), attr.Priority);
            }

            METHODS_CACHE[type] = result;
            return result;
        }

        /// <summary>
        /// Subscribe all [Subscribe]-decorated methods of this object. Idempotent — repeated call is a no-op until Unsubscribe.
        /// </summary>
        public static void Subscribe<T>(this T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (SUBSCRIPTIONS.ContainsKey(obj))
                return; // Already subscribed.

            var methods = GetSubscriberMethods(obj.GetType());
            if (methods.Length == 0)
                return;

            var cache = new List<(Type, Delegate)>(methods.Length);
            SUBSCRIPTIONS[obj] = cache;

            foreach (var subscriber in methods)
            {
                var handler = Delegate.CreateDelegate(subscriber.DelegateType, obj, subscriber.Method); // Create delegate Action<T>
                subscriber.ClosedSubscribe.Invoke(null, new object[] { subscriber.Priority, handler });
                cache.Add((subscriber.EventType, handler));
            }
        }

        public static void Unsubscribe<T>(this T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (!SUBSCRIPTIONS.TryGetValue(obj, out var cache))
                return;

            foreach (var (eventType, handler) in cache)
            {
                var method = UNSUBSCRIBE_CACHE.GetOrAdd(eventType, static t => UNSUBSCRIBE_METHOD.MakeGenericMethod(t));
                method.Invoke(null, new object[] { handler });
            }

            SUBSCRIPTIONS.Remove(obj);
        }
    }
}
