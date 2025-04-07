using System;
using System.Collections.Generic;

namespace BIG
{
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
        private static readonly Dictionary<Type, List<EventSubscriber>> Subscribers = new();
        public static void Clear() => Subscribers.Clear();

        public static void Subscribe<T>(int priority, Action<T> handler) where T : struct
        {
            var type = typeof(T);

            if (!Subscribers.TryGetValue(type, out var list))
            {
                list = new List<EventSubscriber>(4);
                Subscribers[type] = list;
            }

            foreach (var sub in list)
            {
                if (sub.Handler.Equals(handler))
                    return; // already subscribed
            }

            var subscriber = new EventSubscriber(priority, handler);

            int insertAt = list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                if (subscriber.Priority < list[i].Priority)
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
            if (!Subscribers.TryGetValue(type, out var list))
                return;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Handler.Equals(handler))
                {
                    list.RemoveAt(i);
                    if (list.Count == 0)
                        Subscribers.Remove(type);
                    return;
                }
            }
        }

        public static void Raise<T>(T data) where T : struct
        {
            if (!Subscribers.TryGetValue(typeof(T), out var list))
                return;

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Invoke(data);
            }
        }
    }
}
