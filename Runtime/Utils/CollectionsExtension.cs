// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BIG
{
    /// <summary>
    /// Class contains extensions from IEnumerable, IList, arrays and enums.
    /// </summary>
    public static class CollectionsExtension
    {
        private const byte ITERATION_STEPS_COUNT_16 = 16;
        private const byte ITERATION_STEPS_COUNT_15 = 15;
        private const byte TRUE = 1;
        private const byte FALSE = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomElement<T>(this IList<T> elements)
        {
            if (elements == null) throw new Exception("Cannot random element from null list.");
            if (elements.Count < 1) throw new Exception("Cannot random element from empty list.");

            int index = CollectionsExtension.Random.MemoryFriendlyRandom(0, elements.Count);
            return elements[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomElement<T>(this IList<T> elements, System.Random random)
        {
            if (elements == null) throw new Exception("Cannot random element from null list.");
            if (elements.Count < 1) throw new Exception("Cannot random element from empty list.");

            int index = random.Next(0, elements.Count);
            return elements[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomElement<T>(this IEnumerable<T> elements, int maxElementIndex)
        {
            if (elements == null) throw new Exception("Cannot random element from null IEnumerable.");
            if (maxElementIndex < 1) throw new Exception("Cannot random element from empty IEnumerable.");

            int index = CollectionsExtension.Random.MemoryFriendlyRandom(0, maxElementIndex);
            int i = 0;
            foreach (var element in elements)
            {
                if (i == index)
                    return element;
                ++i;
            }

            throw new Exception($"Cannot find element index {index} in elements.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] collection) where T : class
        {
            if (collection == null) return;
            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomElement<T>(this T[] elements)
        {
            if (elements == null) throw new Exception("Cannot random element from null array.");
            if (elements.Length < 1) throw new Exception("Cannot random element from empty array.");

            int index = CollectionsExtension.Random.MemoryFriendlyRandom(0, elements.Length);
            return elements[index];
        }

        /// <summary>
        /// Invoke action on each element of given collection.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">IEnumerable collection.</param>
        /// <param name="action">Action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                if (item != null)
                    action(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Each<T>(this IEnumerable<T> collection, Func<T, bool> whereFunc, Action<T> action)
        {
            foreach (var item in collection)
            {
                if (item != null && whereFunc(item))
                    action(item);
            }
        }

        /// <summary>
        /// Invoke action on each element of given collection.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Generic array collection.</param>
        /// <param name="action">Generic action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArrayEach<T>(this T[] collection, Action<T> action) where T : class
        {
            int i;
            for (i = 0; i < (collection.Length & ~ITERATION_STEPS_COUNT_15); i += ITERATION_STEPS_COUNT_16)
            {
                action(collection[i]);
                action(collection[i + 1]);
                action(collection[i + 2]);
                action(collection[i + 3]);
                action(collection[i + 4]);
                action(collection[i + 5]);
                action(collection[i + 6]);
                action(collection[i + 7]);
                action(collection[i + 8]);
                action(collection[i + 9]);
                action(collection[i + 10]);
                action(collection[i + 11]);
                action(collection[i + 12]);
                action(collection[i + 13]);
                action(collection[i + 14]);
                action(collection[i + 15]);
            }
            for (i = (collection.Length & ~ITERATION_STEPS_COUNT_15); i < collection.Length; i++)
            {
                action(collection[i]);
            }
        }

        /// <summary>
        /// Copy T* into T[].
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Pointer to T.</param>
        /// <param name="array">Destination array</param>
        /// <param name="length">How many elements should be copied to destination array.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyToArray<T>(T* collection, T[] array, int length) where T : unmanaged
        {
            int i;
            for (i = 0; i < (length & ~ITERATION_STEPS_COUNT_15); i += ITERATION_STEPS_COUNT_16)
            {
                array[i] = collection[i];
                array[i + 1] = collection[i + 1];
                array[i + 2] = collection[i + 2];
                array[i + 3] = collection[i + 3];
                array[i + 4] = collection[i + 4];
                array[i + 5] = collection[i + 5];
                array[i + 6] = collection[i + 6];
                array[i + 7] = collection[i + 7];
                array[i + 8] = collection[i + 8];
                array[i + 9] = collection[i + 9];
                array[i + 10] = collection[i + 10];
                array[i + 11] = collection[i + 11];
                array[i + 12] = collection[i + 12];
                array[i + 13] = collection[i + 13];
                array[i + 14] = collection[i + 14];
                array[i + 15] = collection[i + 15];
            }
            for (i = (length & ~ITERATION_STEPS_COUNT_15); i < length; i++)
            {
                array[i] = collection[i];
            }
        }

        /// <summary>
        /// Init T* collection with default T values.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Pointer to T.</param>
        /// <param name="length">How many elements we want to init.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Init<T>(T* collection, int length) where T : unmanaged
        {
            int i;
            for (i = 0; i < (length & ~ITERATION_STEPS_COUNT_15); i += ITERATION_STEPS_COUNT_16)
            {
                collection[i] = new T();
                collection[i + 1] = new T();
                collection[i + 2] = new T();
                collection[i + 3] = new T();
                collection[i + 4] = new T();
                collection[i + 5] = new T();
                collection[i + 6] = new T();
                collection[i + 7] = new T();
                collection[i + 8] = new T();
                collection[i + 9] = new T();
                collection[i + 10] = new T();
                collection[i + 11] = new T();
                collection[i + 12] = new T();
                collection[i + 13] = new T();
                collection[i + 14] = new T();
                collection[i + 15] = new T();
            }
            for (i = (length & ~ITERATION_STEPS_COUNT_15); i < length; i++)
            {
                collection[i] = new T();
            }
        }

        /// <summary>
        /// Invoke action on each element of given collection.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Generic array collection.</param>
        /// <param name="whereFunc">Where statement.</param>
        /// <param name="action">Generic action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArrayEach<T>(ref T[] collection, Func<T, bool> whereFunc, Action<T> action) where T : class
        {
            int i;
            for (i = 0; i < (collection.Length & ~ITERATION_STEPS_COUNT_15); i += ITERATION_STEPS_COUNT_16)
            {
                if (whereFunc(collection[i])) action(collection[i]);
                if (whereFunc(collection[i + 1])) action(collection[i + 1]);
                if (whereFunc(collection[i + 2])) action(collection[i + 2]);
                if (whereFunc(collection[i + 3])) action(collection[i + 3]);
                if (whereFunc(collection[i + 4])) action(collection[i + 4]);
                if (whereFunc(collection[i + 5])) action(collection[i + 5]);
                if (whereFunc(collection[i + 6])) action(collection[i + 6]);
                if (whereFunc(collection[i + 7])) action(collection[i + 7]);
                if (whereFunc(collection[i + 8])) action(collection[i + 8]);
                if (whereFunc(collection[i + 9])) action(collection[i + 9]);
                if (whereFunc(collection[i + 10])) action(collection[i + 10]);
                if (whereFunc(collection[i + 11])) action(collection[i + 11]);
                if (whereFunc(collection[i + 12])) action(collection[i + 12]);
                if (whereFunc(collection[i + 13])) action(collection[i + 13]);
                if (whereFunc(collection[i + 14])) action(collection[i + 14]);
                if (whereFunc(collection[i + 15])) action(collection[i + 15]);
            }
            for (i = (collection.Length & ~ITERATION_STEPS_COUNT_15); i < collection.Length; i++)
            {
                if (whereFunc(collection[i])) action(collection[i]);
            }
        }

        /// <summary>
        /// Remove element from the given collection of structures.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Reference to the collection of structures.</param>
        /// <param name="index">SlotIndex of the element that you want to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAt<T>(ref T[] collection, int index)
        {
            Assert(collection, index);
            T[] result = new T[collection.Length - 1];

            Array.Copy(collection, 0, result, 0, index);
            Array.Copy(collection, index + 1, result, index, collection.Length - index - 1);
            collection = result;
        }

        /// <summary>
        /// Remove element from the given collection of structures.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Collection of the structures</param>
        /// <param name="index">SlotIndex of the element that you want to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAt<T>(ref List<T> collection, int index)
        {
            Assert(collection, index);
            var array = collection.ToArray();
            RemoveAt(ref array, index);
            collection = array.ToList();
        }

        /// <summary>
        /// Get next element from the given collection.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Collection of the class instances.</param>
        /// <param name="currentElement">Current element that you want to lookup from.</param>
        /// <returns>Next element from the collection after current element. If current element was the last one function will return the first object from the list.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNextElement<T>(this IList<T> collection, T currentElement)
        {
            collection.Assert(1);
            var index = collection.IndexOf(currentElement);
            if (index == -1)
            {
                throw new InvalidOperationException("Given element not found in given collection.");
            }
            ++index;
            if (index >= collection.Count)
            {
                index = 0;
            }

            return collection[index];
        }

        /// <summary>
        /// Get previous element from the given collection.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Collection of the elements.</param>
        /// <param name="currentElement">Current element that you want to lookup from.</param>
        /// <returns>Previous element from the collection after current element. If current element was the first one function will return the last object from the list.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPreviousElement<T>(this IList<T> collection, T currentElement)
        {
            collection.Assert(1);
            var index = collection.IndexOf(currentElement);
            if (index == -1)
            {
                throw new InvalidOperationException("Given element not found in given collection.");
            }
            --index;
            if (index < 0)
            {
                index = collection.Count - 1;
            }

            return collection[index];
        }

        /// <summary>
        /// Foreach implementation on enum type.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ForEachEnum<T>() where T : struct
        {
            var list = EnumToList<T>();
            foreach (T value in list)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Get next enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="obj">Enum value.</param>
        /// <returns>Next enum value or the first one if given value was the last.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNextEnum<T>(this T obj) where T : struct
        {
            try
            {
                var tmpList = EnumToList<T>();
                var index = tmpList.IndexOf(obj);
                if (index + 1 >= tmpList.Count)
                {
                    return tmpList[0];
                }

                return tmpList[index + 1];
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Get next enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="obj">Enum value.</param>
        /// <returns>Next enum value or the same if it was the last one.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNextEnumWithoutLoop<T>(this T obj) where T : struct
        {
            try
            {
                var tmpList = EnumToList<T>();
                var index = tmpList.IndexOf(obj);
                if (index + 1 >= tmpList.Count)
                {
                    return tmpList[tmpList.Count - 1];
                }
                return tmpList[index + 1];
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Get previous value for given enum.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="obj">Current enum value.</param>
        /// <returns>Previous enum value or last if function was called on the first element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPreviousEnum<T>(this T obj) where T : struct
        {
            try
            {
                var tmpList = EnumToList<T>();
                var index = tmpList.IndexOf(obj);
                if (index - 1 < 0)
                {
                    return tmpList[tmpList.Count - 1];
                }

                return tmpList[index - 1];
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Get previous value for given enum.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="obj">Current enum value.</param>
        /// <returns>Previous enum value or the same if function was called on the first element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPreviousEnumWithoutLoop<T>(this T obj) where T : struct
        {
            try
            {
                var tmpList = EnumToList<T>();
                var index = tmpList.IndexOf(obj);
                return tmpList[index - 1];
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Gets random enum value.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnum<T>() where T : struct
        {
            var tmpList = EnumToList<T>();
            return tmpList[Random.Range(0, tmpList.Count)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndexOfCurrentEnumValue<T>(this T obj) where T : struct
        {
            return EnumToList<T>().IndexOf(obj);
        }

        /// <summary>
        /// Gets random enum value.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnumWithSeed<T>(byte seed) where T : struct
        {
            var tmpList = EnumToList<T>();
            return tmpList[Random.Range(0, tmpList.Count, seed)];
        }

        /// <summary>
        /// Gets random enum value.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="maxIndexInclusive">Max enum element index that can be exceed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnum<T>(int maxIndexInclusive) where T : struct
        {
            var tmpList = EnumToList<T>();
            if (maxIndexInclusive > tmpList.Count)
            {
                throw new InvalidOperationException(
                  $"Inclusive index '{maxIndexInclusive}' exceed '{typeof(T).Name}' values quantity.");
            }

            return tmpList[Random.Range(0, maxIndexInclusive + 1)];
        }

        /// <summary>
        /// Gets random enum value.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="excludedElement">Element that should be excluded from randomization.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnum<T>(this T excludedElement) where T : struct
        {
            var tmpList = EnumToList<T>();
            int index = tmpList.IndexOf(excludedElement);

            T result = excludedElement;
            while (tmpList.IndexOf(result) == index)
            {
                result = tmpList[Random.Range(0, tmpList.Count)];
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this List<T> list)
        {
            int listCount = list.Count;
            while (listCount > 1)
            {
                listCount--;
                int k = Random.Next(listCount + 1);
                // ReSharper disable once SwapViaDeconstruction
                // (list[k], list[listCount]) = (list[listCount], list[k]); <- unreadable.
                T value = list[k];
                list[k] = list[listCount];
                list[listCount] = value;
            }
        }

        /// <summary>
        /// Cast enum type to list of enum values.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>List of enum values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> EnumToList<T>() where T : struct
        {
            try
            {
                return Enum.GetValues(typeof(T)).Cast<T>().ToList();
            }
            catch
            {
                throw new InvalidOperationException($"Type '{typeof(T).FullName}' is not an enum type.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> EnumToListExcluding<T>(T element) where T : struct
        {
            try
            {
                var result = Enum.GetValues(typeof(T)).Cast<T>().ToList();
                result.Remove(element);
                return result;
            }
            catch
            {
                throw new InvalidOperationException($"Type '{typeof(T).FullName}' is not an enum type.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Assert<T>(this T[] collection, int index)
        {
            if (collection == null)
            {
                throw new NullReferenceException("Collection is null.");
            }
            if (collection.Length < index)
            {
                throw new IndexOutOfRangeException($"Collection length {collection.Length} should be at least {index}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Assert<T>(this IList<T> collection, int index)
        {
            if (collection == null)
            {
                throw new NullReferenceException("Collection is null.");
            }
            if (collection.Count < index)
            {
                throw new IndexOutOfRangeException($"Collection length {collection.Count} should be at least {index}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByte(this bool value) => value ? TRUE : FALSE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTrue(this byte value) => value == TRUE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFalse(this byte value) => value == FALSE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByte(this float value)
        {
            var result = (byte)((value + 1) * 100);
            if (value < 0) result += 1;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrModify<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V GetIfExists<K, V>(this Dictionary<K, V> dictionary, K key) where V : class
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : default (V);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetIfExists<K, V>(this Dictionary<K, V> dictionary, K key, out V value) where V : class
        {
            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key];
                return true;
            }
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddIfNotExists<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
                return;
            dictionary.Add(key, value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveIfExists(this Hashtable hashTable, string key)
        {
            if (hashTable.ContainsKey(key))
                hashTable.Remove(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrModify(this Hashtable hashTable, string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null) return;
            if (hashTable.ContainsKey(key))
                hashTable[key] = value;
            else
                hashTable.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FirstCharToUpper(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value[0].ToString().ToUpper() + value.Substring(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(this List<T> collection, int capacity)
        {
            if (capacity > collection.Count)
                throw new Exception("Capacity exceed list capacity.");

            T[] result = new T[capacity];
            for (int i = 0; i < capacity; i++)
                result[i] = collection[i];
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToOneDimensionalArrayIndex(int x, int y, int arrayLength)
        {
            return x + y * arrayLength;
        }

        /// <summary>
        /// Random implementation that works for Mono.
        /// </summary>
        public static class Random
        {
            private const double PI2 = Math.PI * 2;
            private static System.Random _random = new System.Random();
            private static readonly System.Random PERSISTENT_RANDOM = new System.Random();
            private static MersenneGenerator _mersenneGenerator;

            public static void InitializeSeedRandom(uint seed)
            {
                _mersenneGenerator = new MersenneGenerator(seed);
            }

            public static int GetRandomIntFromSeed(int min, int max)
            {
                return _mersenneGenerator.GetInt(min, max);
            }

            public static float GetRandomFloatFromSeed()
            {
                return _mersenneGenerator.GetFloat();
            }

            public static float GetRandomFloatFromSeed(float min, float max)
            {
                return _mersenneGenerator.GetFloat(min, max);
            }

            public static int MemoryFriendlyRandom(int value1, int value2) => PERSISTENT_RANDOM.Next(value1, value2);

            public static byte MemoryFriendlyRandomByte() => (byte)PERSISTENT_RANDOM.Next(0, 256);

            public static int Range(int value1, int value2)
            {
                _random = new System.Random(Guid.NewGuid().ToByteArray()[0]);
                return _random.Next(value1, value2);
            }

            public static int Range(int value1, int value2, byte seed)
            {
                _random = new System.Random(seed);
                return _random.Next(value1, value2);
            }

            public static int Next(int maxValue) => _random.Next(maxValue);
            public static double NextDouble() => _random.NextDouble();
            public static double RandomAngle() => _random.NextDouble() * PI2;

            public static void RandomPositionInRadius(float x, float y, float radius, out float x2, out float y2)
            {
                var angle = _random.NextDouble() * PI2;
                var distance = Math.Sqrt(_random.NextDouble()) * radius;
                x2 = (float)(x + distance * Math.Cos(angle));
                y2 = (float)(y + distance * Math.Sin(angle));
            }

            public static void RandomOffsetInRadius(float radius, out float x2, out float y2)
            {
                var angle = _random.NextDouble() * PI2;
                var distance = Math.Sqrt(_random.NextDouble()) * radius;
                x2 = (float)(distance * Math.Cos(angle));
                y2 = (float)(distance * Math.Sin(angle));
            }
        }

        private class MersenneGenerator
        {
            private const int N = 624;
            private const int M = 397;
            private int _index = N;
            private readonly uint[] _arr = new uint[N];

            public MersenneGenerator(uint seed)
            {
                _arr[0] = seed;
                for (uint i = 1; i < N; i++)
                {
                    _arr[i] = 1812433253 * (_arr[i - 1] ^ (_arr[i - 1] >> 30)) + i;
                }
            }

            private uint GetNumber()
            {
                if (_index >= N)
                    Twist();

                var y = _arr[_index];

                y = y ^ (y >> 11);
                y = y ^ ((y << 7) & 2636928640);
                y = y ^ ((y << 15) & 4022730752);
                y = y ^ (y >> 18);
                _index++;

                return y;
            }

            private void Twist()
            {
                for (int i = 0; i < N; i++)
                {
                    uint y = ((_arr[i]) & 0x80000000) +
                             ((_arr[(i + 1) % N]) & 0x7fffffff);
                    _arr[i] = _arr[(i + M) % N] ^ (uint)(y >> 1);
                    if (y % 2 != 0)
                        _arr[i] = _arr[i] ^ 0x9908b0df;
                }
                _index = 0;
            }

            public float GetFloat()
            {
                return (float)(GetNumber() % 65536) / 65535.0f;
            }

            public int GetInt(int min, int max)
            {
                return (int)(GetNumber() % (max - min) + min);
            }

            public float GetFloat(float min, float max)
            {
                return GetFloat() * (max - min) + min;
            }
        }
    }

    public static class RandomExtension
    {
        public static float NextFloat(this System.Random random, int x, int y) => random.Next(x, y);
    }
}
