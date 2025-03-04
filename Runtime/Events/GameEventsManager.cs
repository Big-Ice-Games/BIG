#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace BIG.Events
{
    internal class EventSubscriber
    {
        /// <summary>
        /// Lower priority are handled first.
        /// </summary>
        public int Priority;

        public EventHandler EventHandler;

        public EventSubscriber(int priority, EventHandler eventHandler)
        {
            Priority = priority;
            EventHandler = eventHandler;
        }
    }
    public static class GameEventsManager
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
        public static void Subscribe<T>(int priority, EventHandler handler) where T : Event
        {
            string key = typeof(T).FullName;
            InternalSubscription(key, new EventSubscriber(priority, handler));
        }

        /// <summary>
        /// Non-generic subscription used by DI module.
        /// </summary>
        /// <param name="T">Event type.</param>
        /// /// <param name="priority">Priority for subscription. Lower priority are handled first.</param>
        /// <param name="handler">Handle for this event.</param>
        public static void Subscribe(Type T, int priority, EventHandler handler)
        {
            string key = T.FullName;
            InternalSubscription(key, new EventSubscriber(priority, handler));
        }

        private static void InternalSubscription(string key, EventSubscriber handler)
        {
            if (!EVENTS_SUBSCRIBERS.ContainsKey(key))
            {
                EVENTS_SUBSCRIBERS.Add(key, new List<EventSubscriber>());
            }

            if (EVENTS_SUBSCRIBERS[key].All(s => s.EventHandler != handler.EventHandler))
            {
                EVENTS_SUBSCRIBERS[key].Add(handler);

                // Sort list of subscribers by priority. 
                EVENTS_SUBSCRIBERS[key] = EVENTS_SUBSCRIBERS[key].OrderBy(s => s.Priority).ToList();
            }
        }

        /// <summary>
        /// Unsubscribe T type of event from your event handler.
        /// </summary>
        /// <typeparam name="T">Type of event that you are no longer interested in.</typeparam>
        /// <param name="handler">Event handler that subscribed this type of event.</param>
        public static void Unsubscribe<T>(EventHandler handler) where T : Event
        {
            InternalUnsubscribe(typeof(T).FullName, handler);
        }

        /// <summary>
        /// Non-generic unsubscription for given handler on given type used by DI module.
        /// </summary>
        /// <param name="T">Type of event.</param>
        /// <param name="handler">Handle for this event.</param>
        public static void Unsubscribe(Type T, EventHandler handler)
        {
            InternalUnsubscribe(T.FullName, handler);
        }

        private static void InternalUnsubscribe(string key, EventHandler handler)
        {
            if (EVENTS_SUBSCRIBERS.TryGetValue(key, out var value))
            {
                for (int i = 0; i < value.Count; i++)
                {
                    if (value[i].EventHandler == handler)
                    {
                        value.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Event publication function.
        /// </summary>
        /// <typeparam name="T">Type of event that we are publishing.</typeparam>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event as self.</param>
        public static void Publish<T>(object sender, T e) where T : Event?
        {
            string key = typeof(T).FullName;

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception($"GameEventManager: Cannot recognize {typeof(T)} assembly FullName.");
            }

            if (EVENTS_SUBSCRIBERS.ContainsKey(key))
            {
                // Get copy of the list of subscribers
                var tmp = EVENTS_SUBSCRIBERS[key].ToList();

                for (int i = 0; i < tmp.Count; i++)
                {
                    // If no one already consumed this event
                    if (!e.Consumed)
                    {
                        // If subscriber wasn't removed from the subscribers in meantime
                        if (EVENTS_SUBSCRIBERS[key].Contains(tmp[i]))
                            tmp[i]?.EventHandler?.Invoke(sender, e);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
