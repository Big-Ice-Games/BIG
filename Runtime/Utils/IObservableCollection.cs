using System;
using System.Collections;
using System.Collections.Generic;

namespace BIG
{
    public interface IObservableCollection<T>
    {
        event Action<IList<T>>? OnChanged;
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
    }

    [Serializable]
    internal class ObservableCollection<T> : IList<T>, IObservableCollection<T>
    {
        private readonly IList<T> _list;
        public event Action<IList<T>>? OnChanged;

        internal ObservableCollection(IList<T>? initialList)
        {
            _list = initialList ?? new List<T>();
        }

        internal ObservableCollection(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                Invoke();
            }
        }

        public void Invoke() => OnChanged?.Invoke(_list);

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public void Add(T item)
        {
            _list.Add(item);
            Invoke();
        }

        public void Clear()
        {
            _list.Clear();
            Invoke();
        }

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var result = _list.Remove(item);
            if (result)
            {
                Invoke();
            }

            return result;
        }

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            Invoke();
        }

        public void RemoveAt(int index)
        {
            T item = _list[index];
            _list.RemoveAt(index);
            Invoke();
        }
    }
}