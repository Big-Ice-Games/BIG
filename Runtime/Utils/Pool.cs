#region license

// Copyright (c) 2025, Bie Ice Games
// All rights reserved.

#endregion

using System;

namespace BIG
{
    public class Pool<T> where T : class, new()
    {
        private class PoolNode
        {
            public T? Item;
            public PoolNode? Next;
        }

        private PoolNode? _head; // Head of the linked list (points to the next free element)
        private readonly object _lock = new object(); // For thread safety

        public Pool(int initialCapacity)
        {
            for (int i = 0; i < initialCapacity; i++)
            {
                var node = new PoolNode { Item = new T() };
                Put(node);
            }
        }

        public T? Get()
        {
            lock (_lock)
            {
                if (_head == null)
                {
                    return new T();
                }

                // Get the object from the head of the list
                PoolNode node = _head;
                _head = _head.Next;
                node.Next = null; // Clear the next pointer to avoid memory leaks
                return node.Item;
            }
        }

        public void Put(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (_lock)
            {
                PoolNode node = new PoolNode { Item = item, Next = _head };
                _head = node;
            }
        }

        private void Put(PoolNode node)
        {
            lock (_lock)
            {
                node.Next = _head;
                _head = node;
            }
        }
    }
}