#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Action = Unity.Plastic.Newtonsoft.Json.Serialization.Action;

namespace BIG
{
    internal readonly struct EventSubscriber
    {
        public EventSubscriber(int priority, Action<object> handler)
        {
            Priority = priority;
            Handler = handler;
        }
        public int Priority { get; }
        public Action<object> Handler { get; }
        public void Invoke(object @event) => Handler?.Invoke(@event);
    }
    
    public static class Events
    {
        private static readonly Dictionary<string, IList<EventSubscriber>> EVENTS_SUBSCRIBERS = new Dictionary<string, IList<EventSubscriber>>(64);

        /// <summary>
		/// Clear event subscriptions.
		/// </summary>
        public static void Clear()
        {
            EVENTS_SUBSCRIBERS.Clear();
        }

        /// <summary>
        /// GMSubscribe for T type of event.
        /// </summary>
        /// <typeparam name="T">Type of event you want to subscribe for.</typeparam>
        /// <param name="priority">Priority for subscription. Lower priority are handled first.</param>
        /// <param name="handler">Event handler that receive this event.</param>
        public static void Subscribe<T>(int priority, Action<Event<T>> handler) where T : struct
        {
            string key = typeof(T).FullName;
            var wrappedHandler = new Action<object>((e) => 
            {
                if (e is Event<T> typedEvent)
                {
                    handler(typedEvent);
                }
            });

            InternalSubscription(key, new EventSubscriber(priority, wrappedHandler));
        }

        /// <summary>
        /// Non-generic subscription used by DI module.
        /// </summary>
        /// ///
        /// <param name="eventDataTypeFullName">Pass typeof(T).FullName from Event(T) used in handler.
        /// It is done automatically in <see cref="SubscribeAttribute"/></param>
        /// <param name="priority">Priority for subscription. Lower priority are handled first.</param>
        /// <param name="handler">Handle for this event. Under the hood is Action(Event(T))</param>
        public static void Subscribe(string eventDataTypeFullName, int priority, Action<object> handler)
        {
            string key = eventDataTypeFullName;
            InternalSubscription(key, new EventSubscriber(priority, handler));
        }

        /// <summary>
        /// Subscribes a handler to the event subscribers list for a given event type.
        /// </summary>
        /// <param name="key">The full name of the event type.</param>
        /// <param name="handler">The event subscriber to be added.</param>
        private static void InternalSubscription(string key, EventSubscriber handler)
        {
            Debug.Log($"Event {key} subscrubed with priority {handler.Priority}");
            if (EVENTS_SUBSCRIBERS.TryGetValue(key, out var subscribers) == false)
            {
                subscribers = new List<EventSubscriber>(4); // If there are no subscribers for this event type, create a new list.
                EVENTS_SUBSCRIBERS[key] = subscribers;
            }

            for (int i = 0; i < subscribers.Count; i++)
            {
                if (subscribers[i].Handler == handler.Handler) return; // Already subscribe
            }

            // Sort by priority
            int insertAt = subscribers.Count;
            for (int i = 0; i < subscribers.Count; i++)
            {
                if (handler.Priority < subscribers[i].Priority)
                {
                    insertAt = i;
                    break;
                }
            }

            subscribers.Insert(insertAt, handler);
        }

        /// <summary>
        /// Unsubscribe T type of event from your event handler.
        /// </summary>
        /// <typeparam name="T">Type of event that you are no longer interested in.</typeparam>
        /// <param name="handler">Event handler that subscribed this type of event.</param>
        public static void Unsubscribe<T>(Action<object> handler)
        {
            InternalUnsubscribe(typeof(T).FullName, handler);
        }

        /// <summary>
        /// Non-generic unsubscription for given handler on given type used by DI module.
        /// </summary>
        /// <param name="eventDataTypeFullName">Pass typeof(T).FullName from Event(T) used in handler.
        /// It is done automatically in <see cref="SubscribeAttribute"/></param>
        /// <param name="handler">Handle for this event.</param>
        public static void Unsubscribe(string eventDataTypeFullName, Action<object> handler)
        {
            InternalUnsubscribe(eventDataTypeFullName, handler);
        }

        private static void InternalUnsubscribe(string key, Action<object> handler)
        {
            if (EVENTS_SUBSCRIBERS.TryGetValue(key, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Handler == handler)
                    {
                        list.RemoveAt(i);
                        Debug.Log($"Event {key} unsubscribed.");
                        if (list.Count == 0)
                            EVENTS_SUBSCRIBERS.Remove(key);
                        return;
                    }
                }
            }
        }
        
        /// <summary>
        /// Event publication function.
        /// </summary>
        /// <typeparam name="T">Type of event that we are publishing.</typeparam>
        /// <param name="e">Event as self.</param>
        public static void Publish<T>(Event<T> e) where T : struct
        {
            string key = typeof(T).FullName;

            if (!EVENTS_SUBSCRIBERS.TryGetValue(key, out var subs))
                return;
            
            for (int i = 0; i < subs.Count;)
            {
                var subscriber = subs[i];
                i++; // Increment index before potential change in the list of subscribers
                
                subscriber.Invoke(e.Data);
            }
        }
        
        public static void Raise<T>(this T eventData) where T : struct
        {
            var e = new Event<T>(eventData);
            Publish(e);
        }
        
        public static void Raise<T>(this ref T eventData) where T : struct
        {
            var e = new Event<T>(eventData);
            Publish(e);
        }
    }
}
