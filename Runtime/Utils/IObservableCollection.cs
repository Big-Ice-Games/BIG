// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace BIG
{
    public interface IObservableCollection<T>
    {
        event Action<IList<T>> OnChanged;
        void Add(T item);
        void Insert(int index, T item);
        void Clear();
        bool Contains(T item);
        int IndexOf(T item);
        void CopyTo(T[] array, int arrayIndex);
        bool Remove(T item);
        IEnumerator<T> GetEnumerator();
        void RemoveAt(int index);
        public static IObservableCollection<T> Create(IList<T> elements) => new ObservableCollection<T>(elements);
        public static IObservableCollection<T> Create(int capacity) => new ObservableCollection<T>(capacity);
        
        [Serializable]
        private class ObservableCollection<TG> : IList<TG>, IObservableCollection<TG>
        {
            private readonly IList<TG> _list;
            public event Action<IList<TG>> OnChanged = delegate { };
            internal ObservableCollection(IList<TG> initialList) => _list = initialList ?? new List<TG>();
            internal ObservableCollection(int capacity) => _list = new List<TG>(capacity);
    
            public TG this[int index]
            {
                get => _list[index];
                set
                {
                    _list[index] = value;
                    OnChanged?.Invoke(_list);
                }
            }
            
            public int Count => _list.Count;
            public bool IsReadOnly => _list.IsReadOnly;
    
            public void Add(TG item)
            {
                _list.Add(item);
                OnChanged?.Invoke(_list);
            }
    
            public void Clear()
            {
                _list.Clear();
                OnChanged?.Invoke(_list);
            }
    
            public bool Contains(TG item) => _list.Contains(item);
    
            public void CopyTo(TG[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    
            public bool Remove(TG item)
            {
                var result = _list.Remove(item);
                if (result) OnChanged?.Invoke(_list);
                return result;
            }
    
            public IEnumerator<TG> GetEnumerator() => _list.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    
            public int IndexOf(TG item) => _list.IndexOf(item);
    
            public void Insert(int index, TG item)
            {
                _list.Insert(index, item);
                OnChanged?.Invoke(_list);
            }
    
            public void RemoveAt(int index)
            {
                TG item = _list[index];
                _list.RemoveAt(index);
                OnChanged?.Invoke(_list);
            }
        }
    }
}