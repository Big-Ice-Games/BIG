using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BIG
{
    /// <summary>
    /// Values of an enum type cached once per type — enum reflection (Enum.GetValues) allocates on every call,
    /// so all enum helpers read from this cache instead.
    /// Every closed EnumCache&lt;T&gt; self-registers in <see cref="Cache"/> on first use, so <see cref="Cache.Clear"/> refreshes it.
    /// </summary>
    internal static class EnumCache<T> where T : struct, Enum
    {
        public static T[] VALUES = (T[])Enum.GetValues(typeof(T));

        static EnumCache()
        {
            Cache.Register($"EnumCache<{typeof(T).FullName}>", () => VALUES = (T[])Enum.GetValues(typeof(T)));
        }
    }

    /// <summary>
    /// Class contains extensions from IEnumerable, IList, arrays and enums.
    /// </summary>
    public static class Collections
    {
        private const byte TRUE = 1;
        private const byte FALSE = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomElement<T>(this IList<T> elements)
        {
            if (elements == null) throw new Exception("Cannot random element from null list.");
            if (elements.Count < 1) throw new Exception("Cannot random element from empty list.");

            int index = Random.MemoryFriendlyRandom(0, elements.Count);
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

            int index = Random.MemoryFriendlyRandom(0, maxElementIndex);
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
        public static void Clear<T>(this T?[]? collection) where T : class
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

            int index = Random.MemoryFriendlyRandom(0, elements.Length);
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
        /// Invoke action on each element of given dictionary without GC allocation.
        /// </summary>
        /// <param name="dict">Dictionary</param>
        /// <param name="action">Action to invoke</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Each<TK, TV>(this Dictionary<TK, TV> dict, Action<TK, TV> action)
        {
            var enumerator = dict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var kvp = enumerator.Current;
                action(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Invoke action on each element of given collection.
        /// Note: the delegate call dominates the loop cost, so a plain loop is as fast as it gets.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Generic array collection.</param>
        /// <param name="action">Generic action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArrayEach<T>(this T[] collection, Action<T> action) where T : class
        {
            for (int i = 0; i < collection.Length; i++)
            {
                action(collection[i]);
            }
        }

        /// <summary>
        /// Invoke action on each element of given 2D collection.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Generic array collection.</param>
        /// <param name="action">Generic action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Array2DEach<T>(this T[,] collection, Action<T> action)
        {
            int width = collection.GetLength(0);
            int height = collection.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    action(collection[x, y]);
                }
            }
        }

        /// <summary>
        /// Copy T* into T[] (memmove under the hood).
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Pointer to T.</param>
        /// <param name="array">Destination array</param>
        /// <param name="length">How many elements should be copied to destination array.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyToArray<T>(T* collection, T[] array, int length) where T : unmanaged
        {
            new ReadOnlySpan<T>(collection, length).CopyTo(array);
        }

        /// <summary>
        /// Init T* collection with default T values (memset under the hood).
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Pointer to T.</param>
        /// <param name="length">How many elements we want to init.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Init<T>(T* collection, int length) where T : unmanaged
        {
            new Span<T>(collection, length).Clear();
        }

        /// <summary>
        /// Invoke action on each element of given collection that passes the where statement.
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Generic array collection.</param>
        /// <param name="whereFunc">Where statement.</param>
        /// <param name="action">Generic action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArrayEach<T>(ref T[] collection, Func<T, bool> whereFunc, Action<T> action) where T : class
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (whereFunc(collection[i])) action(collection[i]);
            }
        }

        /// <summary>
        /// Remove element from the given array (allocates a new, smaller array).
        /// </summary>
        /// <typeparam name="T">Type of collection elements.</typeparam>
        /// <param name="collection">Reference to the collection of structures.</param>
        /// <param name="index">SlotIndex of the element that you want to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAt<T>(ref T[] collection, int index)
        {
            AssertIndex(collection, index);
            T[] result = new T[collection.Length - 1];

            Array.Copy(collection, 0, result, 0, index);
            Array.Copy(collection, index + 1, result, index, collection.Length - index - 1);
            collection = result;
        }

        /// <summary>
        /// Removes X random elements from list
        /// If the count to remove is bigger than list count -> it removes all the elements from the list
        /// </summary>
        public static void RemoveRandomElements<T>(
            ref List<T> list,
            int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (count <= 0)
                return;

            int remaining = list.Count;

            count = Math.Min(count, remaining);

            for (int i = 0; i < count; i++)
            {
                int index = Random.MemoryFriendlyRandom(remaining);
                list.RemoveAt(index);
                remaining--;
            }
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
            collection.AssertMinCount(1);
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
            collection.AssertMinCount(1);
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
        /// All values of the given enum type — cached, zero allocation.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ForEachEnum<T>() where T : struct, Enum
        {
            return EnumCache<T>.VALUES;
        }

        /// <summary>
        /// Get next enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="obj">Enum value.</param>
        /// <returns>Next enum value or the first one if given value was the last.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNextEnum<T>(this T obj) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            var index = Array.IndexOf(values, obj);
            if (index + 1 >= values.Length)
            {
                return values[0];
            }

            return values[index + 1];
        }

        /// <summary>
        /// Get next enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="obj">Enum value.</param>
        /// <param name="excludeValue">Value from given enum that we would like to skip.</param>
        /// <returns>Next enum value or the first one if given value was the last.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNextEnum<T>(this T obj, T excludeValue) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            var index = Array.IndexOf(values, obj);
            var securityCount = values.Length;
            do
            {
                if (index + 1 >= values.Length)
                    index = 0;
                else
                    index++;
                if (--securityCount <= 0)
                    break;
            } while (values[index].Equals(excludeValue));

            return values[index];
        }

        /// <summary>
        /// Get next enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="obj">Enum value.</param>
        /// <param name="exclude">Values from given enum that we would like to skip.</param>
        /// <returns>Next enum value or the first one if given value was the last.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNextEnum<T>(this T obj, params T[] exclude) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            var index = Array.IndexOf(values, obj);
            var securityCount = values.Length;
            do
            {
                if (index + 1 >= values.Length)
                    index = 0;
                else
                    index++;
                if (--securityCount <= 0)
                    break;
            } while (Array.IndexOf(exclude, values[index]) != -1);

            return values[index];
        }

        /// <summary>
        /// Get next enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="obj">Enum value.</param>
        /// <returns>Next enum value or the same if it was the last one.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNextEnumWithoutLoop<T>(this T obj) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            var index = Array.IndexOf(values, obj);
            if (index + 1 >= values.Length)
            {
                return values[values.Length - 1];
            }
            return values[index + 1];
        }

        /// <summary>
        /// Get previous value for given enum.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="obj">Current enum value.</param>
        /// <returns>Previous enum value or last if function was called on the first element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPreviousEnum<T>(this T obj) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            var index = Array.IndexOf(values, obj);
            if (index - 1 < 0)
            {
                return values[values.Length - 1];
            }

            return values[index - 1];
        }

        /// <summary>
        /// Get previous value for given enum.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="obj">Current enum value.</param>
        /// <returns>Previous enum value or the same if function was called on the first element.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetPreviousEnumWithoutLoop<T>(this T obj) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            var index = Array.IndexOf(values, obj);
            if (index <= 0)
            {
                return obj;
            }
            return values[index - 1];
        }

        /// <summary>
        /// Gets random enum value.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnum<T>() where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            return values[Random.Range(0, values.Length)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndexOfCurrentEnumValue<T>(this T obj) where T : struct, Enum
        {
            return Array.IndexOf(EnumCache<T>.VALUES, obj);
        }

        /// <summary>
        /// Gets random enum value. Deterministic by seed.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnumWithSeed<T>(byte seed) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            return values[Random.Range(0, values.Length, seed)];
        }

        /// <summary>
        /// Gets random enum value.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="maxIndexInclusive">Max enum element index that can be exceed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnum<T>(int maxIndexInclusive) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            if (maxIndexInclusive > values.Length)
            {
                throw new InvalidOperationException(
                  $"Inclusive index '{maxIndexInclusive}' exceed '{typeof(T).Name}' values quantity.");
            }

            return values[Random.Range(0, maxIndexInclusive + 1)];
        }

        /// <summary>
        /// Gets random enum value different from the excluded element.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="excludedElement">Element that should be excluded from randomization.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomEnum<T>(this T excludedElement) where T : struct, Enum
        {
            var values = EnumCache<T>.VALUES;
            if (values.Length <= 1)
            {
                return excludedElement;
            }

            T result = excludedElement;
            while (result.Equals(excludedElement))
            {
                result = values[Random.Range(0, values.Length)];
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
        /// Cast enum type to list of enum values. Allocates a new list — for allocation-free iteration use <see cref="ForEachEnum{T}"/>.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>List of enum values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> EnumToList<T>() where T : struct, Enum
        {
            return new List<T>(EnumCache<T>.VALUES);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> EnumToListExcluding<T>(params T[] elements) where T : struct, Enum
        {
            var result = new List<T>(EnumCache<T>.VALUES);
            result.RemoveAll(elements.Contains);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertIndex<T>(this T[] collection, int index)
        {
            if (collection == null)
            {
                throw new NullReferenceException("Collection is null.");
            }
            if (index < 0 || index >= collection.Length)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range for collection of length {collection.Length}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertMinCount<T>(this IList<T> collection, int minCount)
        {
            if (collection == null)
            {
                throw new NullReferenceException("Collection is null.");
            }
            if (collection.Count < minCount)
            {
                throw new IndexOutOfRangeException($"Collection count {collection.Count} should be at least {minCount}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByte(this bool value) => value ? TRUE : FALSE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTrue(this byte value) => value == TRUE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFalse(this byte value) => value == FALSE;

        /// <summary>
        /// Quantize value from range [-1, 1] into a byte in range [0, 200]. Values outside the range are clamped.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByte(this float value)
        {
            value = Math.Clamp(value, -1f, 1f);
            var result = (byte)((value + 1) * 100);
            if (value < 0) result += 1;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveIfExists(this Hashtable hashTable, string key)
        {
            if (hashTable.ContainsKey(key))
               hashTable.Remove(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrModify(this Hashtable hashTable, string key, object? value)
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

        /// <summary>
        /// Copy first count elements of the list into a new array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] TakeToArray<T>(this List<T> collection, int count)
        {
            if (count > collection.Count)
                throw new ArgumentOutOfRangeException(nameof(count), $"Count {count} exceeds list count {collection.Count}.");

            T[] result = new T[count];
            collection.CopyTo(0, result, 0, count);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToOneDimensionalArrayIndex(int x, int y, int arrayLength)
        {
            return x + y * arrayLength;
        }

        /// <summary>
        /// Check whether the collection contains given position.
        /// </summary>
        public static bool Contains(this IntVector2[]? collection, IntVector2 position)
        {
            if (collection == null || collection.Length < 1) return false;

            foreach (var element in collection)
            {
                if (element == position)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check whether the span contains given position.
        /// </summary>
        public static bool Contains(this Span<IntVector2> collection, IntVector2 position)
        {
            foreach (var positionInCollection in collection)
            {
                if (positionInCollection == position)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Random implementation that works for Mono.
        /// </summary>
        public static class Random
        {
            private const double PI2 = Math.PI * 2;
            // ThreadLocal ensures each thread has its own System.Random instance — System.Random is not thread-safe.
            private static readonly ThreadLocal<System.Random> TLS_RANDOM = new ThreadLocal<System.Random>(() => new System.Random());
            public static int MemoryFriendlyRandom(int value1, int value2) => TLS_RANDOM.Value!.Next(value1, value2);
            public static int MemoryFriendlyRandom(int value) => TLS_RANDOM.Value!.Next(value);
            public static byte MemoryFriendlyRandomByte() => (byte)TLS_RANDOM.Value!.Next(0, 256);

            public static int Range(int value1, int value2) => TLS_RANDOM.Value!.Next(value1, value2);

            /// <summary>
            /// Deterministic by seed: the same seed always returns the same value.
            /// </summary>
            public static int Range(int value1, int value2, byte seed) => new System.Random(seed).Next(value1, value2);

            public static int Next(int maxValue) => TLS_RANDOM.Value!.Next(maxValue);
            public static double NextDouble() => TLS_RANDOM.Value!.NextDouble();
            public static double RandomAngle() => TLS_RANDOM.Value!.NextDouble() * PI2;

            public static void RandomPositionInRadius(float x, float y, float radius, out float x2, out float y2)
            {
                var random = TLS_RANDOM.Value!;
                var angle = random.NextDouble() * PI2;
                var distance = Math.Sqrt(random.NextDouble()) * radius;
                x2 = (float)(x + distance * Math.Cos(angle));
                y2 = (float)(y + distance * Math.Sin(angle));
            }

            public static void RandomOffsetInRadius(float radius, out float x2, out float y2)
            {
                var random = TLS_RANDOM.Value!;
                var angle = random.NextDouble() * PI2;
                var distance = Math.Sqrt(random.NextDouble()) * radius;
                x2 = (float)(distance * Math.Cos(angle));
                y2 = (float)(distance * Math.Sin(angle));
            }
        }

        public static MersenneGenerator GetMersenneGenerator(uint seed) => new MersenneGenerator(seed);
        public class MersenneGenerator
        {
            private const int N = 624;
            private const int M = 397;
            private int _index = N;
            private readonly uint[] _arr = new uint[N];

            internal MersenneGenerator(uint seed)
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
                    _arr[i] = _arr[(i + M) % N] ^ (y >> 1);
                    if (y % 2 != 0)
                        _arr[i] = _arr[i] ^ 0x9908b0df;
                }
                _index = 0;
            }

            public float GetFloat()
            {
                return GetNumber() % 65536 / 65535.0f;
            }

            public int GetInt(int min, int max)
            {
                if (min >= max)
                    return max;

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
        public static float NextFloat(this Random random, int x, int y)
            => (float)(random.NextDouble() * (y - x)) + x;
    }
}
