#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

namespace BIG
{
    public struct Event<T> where T : struct
    {
        internal Event(T data)
        {
            Data = data;
            EventIdentifier = typeof(T).FullName;
        }
        public T Data { get; }
        public string EventIdentifier { get; }
    }
}