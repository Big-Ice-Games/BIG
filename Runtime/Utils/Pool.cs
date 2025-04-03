#region license

// Copyright (c) 2025, Bie Ice Games
// All rights reserved.

#endregion

using System;

namespace BIG
{
    public interface IPoolObjectProvider<T> where T : class, new()
    {
        T Create();
    }
    
    public class Pool<T> where T : class, new()
    {
        private class PoolNode
        {
            public T Item;
            public PoolNode Next;
        }

        private PoolNode _head; // Head of the linked list (points to the next free element)
        private readonly object _lock = new object(); // For thread safety
        private readonly IPoolObjectProvider<T> _provider;
        
        public Pool(int initialCapacity, IPoolObjectProvider<T> provider = null)
        {
            _provider = provider;
            for (int i = 0; i < initialCapacity; i++)
            {
                var node = new PoolNode { Item = Create() };
                Put(node);
            }
        }

        private T Create() => _provider != null ? _provider.Create() : new T();

        public T Get()
        {
            lock (_lock)
            {
                if (_head == null)
                {
                    if (_provider != null) return _provider.Create();
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