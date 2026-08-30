using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace BIG
{
    /// <summary>
    /// Serialized bytes backed by an array rented from <see cref="ArrayPool{T}"/> — zero GC pressure.
    /// Dispose returns the array to the pool, so the idiomatic usage is:
    /// <code>
    /// using var buffer = Serializer.SerializePooled(state);
    /// socket.Send(buffer.Span);
    /// </code>
    /// Treat it as move-only: dispose exactly once and do not touch <see cref="Span"/> afterwards
    /// (the array goes back to the pool and WILL be handed to someone else).
    /// </summary>
    public readonly struct PooledBuffer : IDisposable
    {
        private readonly byte[] _array;

        /// <summary> Number of valid bytes. The rented array itself can be longer. </summary>
        public readonly int Length;

        internal PooledBuffer(byte[] array, int length)
        {
            _array = array;
            Length = length;
        }

        /// <summary> The valid slice of the rented array. </summary>
        public ReadOnlySpan<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ReadOnlySpan<byte>(_array, 0, Length);
        }

        /// <summary>
        /// Raw rented array for APIs that require byte[] (e.g. older socket overloads).
        /// Use together with <see cref="Length"/> — the array is usually longer than the payload.
        /// </summary>
        public byte[] Array
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _array;
        }

        public void Dispose()
        {
            if (_array != null)
                ArrayPool<byte>.Shared.Return(_array);
        }
    }
}
